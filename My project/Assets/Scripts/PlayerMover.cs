using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class PlayerMover : MonoBehaviour
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";

    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _shrinkFactor = 0.5f;
    [SerializeField] private float _shrinkDuration = 0.2f;
    [SerializeField] private float _growDuration = 1.0f;

    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    private Vector3 originalScale;
    private float originalRadius;
    private float _directionX;
    private float _directionY;
    private bool _isShrinking = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        originalScale = transform.localScale;
        originalRadius = _circleCollider.radius;

        // Set Rigidbody2D Interpolation for smooth physics
        _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        _directionX = Input.GetAxis(HORIZONTAL_AXIS);
        _directionY = Input.GetAxis(VERTICAL_AXIS);
        if (Input.GetKeyDown(KeyCode.Space) && !_isShrinking)
        {
            StartCoroutine(Shrink());
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(_speed * _directionX, _speed * _directionY);
    }

    IEnumerator Shrink()
    {
        _isShrinking = true;
        Vector3 targetScale = originalScale * _shrinkFactor;
        float targetRadius = originalRadius * _shrinkFactor;
        float elapsedTime = 0;

        // Stop movement to avoid physics issues
        _rigidbody.velocity = Vector2.zero;

        while (elapsedTime < _shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _shrinkDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            _circleCollider.radius = Mathf.Lerp(originalRadius, targetRadius, t);
            yield return null;
        }

        transform.localScale = targetScale;
        _circleCollider.radius = targetRadius;

        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));

        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        Vector3 startScale = transform.localScale;
        float startRadius = _circleCollider.radius;
        float elapsedTime = 0;

        // Stop movement to avoid physics glitches
        _rigidbody.velocity = Vector2.zero;

        while (elapsedTime < _growDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _growDuration;
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            _circleCollider.radius = Mathf.Lerp(startRadius, originalRadius, t);
            yield return null;
        }

        transform.localScale = originalScale;
        _circleCollider.radius = originalRadius;
        _isShrinking = false;
    }
}
