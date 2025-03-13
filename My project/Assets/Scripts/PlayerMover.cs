using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // Required for UI updates

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private string _horizontalAxis = "Horizontal";
    [SerializeField] private string _verticalAxis = "Vertical";
    [SerializeField] private KeyCode _shrinkKey = KeyCode.LeftShift;

    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _shrinkFactor = 0.5f;
    [SerializeField] private float _shrinkDuration = 0.2f;
    [SerializeField] private float _growDuration = 1.0f;
    [SerializeField] private float _shrinkTimeLimit = 5.0f;  // MAX SHRINK TIME

    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    private Vector3 _originalScale;
    private float _originalRadius;
    private bool _isShrinking = false;
    private Vector2 _movementInput;
    private float _shrinkTimeRemaining;

    public int PlayerID = 1;  // Set in Unity Inspector (Player 1 = 1, Player 2 = 2)
    public int Score { get; private set; }  // Exposed for UI

    public void AddPoint()
    {
        Score += 1;
    }

    public float ShrinkTimeRemaining => _shrinkTimeRemaining;  // UI can read this

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _originalScale = transform.localScale;
        _originalRadius = _circleCollider.radius;
        _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        _shrinkTimeRemaining = _shrinkTimeLimit; // Set initial shrink time
    }

    void Update()
    {
        // Read movement input
        _movementInput.x = Input.GetAxis(_horizontalAxis);
        _movementInput.y = Input.GetAxis(_verticalAxis);

        // Shrinking logic
        if (Input.GetKeyDown(_shrinkKey) && !_isShrinking && _shrinkTimeRemaining > 0)
        {
            StartCoroutine(Shrink());
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _movementInput * _speed;
    }

    IEnumerator Shrink()
    {
        _isShrinking = true;
        Vector3 targetScale = _originalScale * _shrinkFactor;
        float targetRadius = _originalRadius * _shrinkFactor;
        float elapsedTime = 0;

        _rigidbody.velocity = Vector2.zero; // Stop movement during shrinking

        while (elapsedTime < _shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _shrinkDuration;
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
            _circleCollider.radius = Mathf.Lerp(_originalRadius, targetRadius, t);
            yield return null;
        }

        transform.localScale = targetScale;
        _circleCollider.radius = targetRadius;

        // Countdown shrink duration while holding the key
        while (Input.GetKey(_shrinkKey) && _shrinkTimeRemaining > 0)
        {
            _shrinkTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        // Prevent going below zero
        _shrinkTimeRemaining = Mathf.Max(0, _shrinkTimeRemaining);

        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        Vector3 startScale = transform.localScale;
        float startRadius = _circleCollider.radius;
        float elapsedTime = 0;

        _rigidbody.velocity = Vector2.zero;

        while (elapsedTime < _growDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _growDuration;
            transform.localScale = Vector3.Lerp(startScale, _originalScale, t);
            _circleCollider.radius = Mathf.Lerp(startRadius, _originalRadius, t);
            yield return null;
        }

        transform.localScale = _originalScale;
        _circleCollider.radius = _originalRadius;
        _isShrinking = false;

        // **Reset shrink time after fully growing**
        _shrinkTimeRemaining = _shrinkTimeLimit;
    }

    // Reset shrink timer (call when collecting a booster)
    public void ResetShrinkTimer()
    {
        _shrinkTimeRemaining = _shrinkTimeLimit;
    }
}
