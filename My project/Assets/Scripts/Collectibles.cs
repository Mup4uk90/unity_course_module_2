using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int PlayerID;  // 1 = Player1, 2 = Player2
    public System.Action OnCollected;  // Event callback

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();

        if (player != null && player.PlayerID == PlayerID) // Correct player?
        {
            player.AddPoint();
            OnCollected?.Invoke();  // Notify spawner to respawn
            Destroy(gameObject);
        }
    }
}
