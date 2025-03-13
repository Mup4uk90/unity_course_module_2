using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private PlayerMover player1;
    [SerializeField] private PlayerMover player2;
    [SerializeField] private TMP_Text player1ScoreText;
    [SerializeField] private TMP_Text player2ScoreText;

    void Update()
    {
        player1ScoreText.text = $"Player 1 Score: {player1.Score}";
        player2ScoreText.text = $"Player 2 Score: {player2.Score}";
    }
}
