using UnityEngine;
using System.Collections.Generic;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject collectiblePrefab;  // Assign in Unity
    [SerializeField] private Transform[] spawners;  // 8 spawner locations
    [SerializeField] private Color player1Color = Color.red;
    [SerializeField] private Color player2Color = Color.blue;

    private GameObject circle1, circle2;

    void Start()
    {
        SpawnCircles();
    }

    void SpawnCircles()
    {
        // Pick 2 different random spawners
        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < spawners.Length; i++) availableIndexes.Add(i);
        int index1 = availableIndexes[Random.Range(0, availableIndexes.Count)];
        availableIndexes.Remove(index1);
        int index2 = availableIndexes[Random.Range(0, availableIndexes.Count)];

        // Spawn Player 1’s Circle
        circle1 = Instantiate(collectiblePrefab, spawners[index1].position, Quaternion.identity);
        var col1 = circle1.GetComponent<Collectible>();
        col1.PlayerID = 1;
        col1.OnCollected += RespawnCircles;
        circle1.GetComponent<SpriteRenderer>().color = player1Color;

        // Spawn Player 2’s Circle
        circle2 = Instantiate(collectiblePrefab, spawners[index2].position, Quaternion.identity);
        var col2 = circle2.GetComponent<Collectible>();
        col2.PlayerID = 2;
        col2.OnCollected += RespawnCircles;
        circle2.GetComponent<SpriteRenderer>().color = player2Color;
    }

    void RespawnCircles()
    {
        if (circle1) Destroy(circle1);
        if (circle2) Destroy(circle2);
        SpawnCircles();
    }
}
