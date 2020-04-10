using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollector : MonoBehaviour
{
    [SerializeField]
    private Transform minSpawnerPoint;
    [SerializeField]
    private Transform maxSpawnerPoint;
    [SerializeField]
    private Transform ballsParent;

    private GameObject prefab;
    private int ballsLeftToSpawn;

    private void Update()
    {
        while (ballsLeftToSpawn > 0)
        {
            SpawnBall();
            ballsLeftToSpawn--;
        }
    }

    private void SpawnBall()
    {
        Vector3 position = Vector3.Lerp(minSpawnerPoint.position, maxSpawnerPoint.position, Random.value);

        GameObject instance = Instantiate(prefab, ballsParent);
        instance.transform.position = position;
    }

    public void SpawnBalls(GameObject prefab, int amount)
    {
        this.prefab = prefab;
        ballsLeftToSpawn = amount;
    }

    public void Clear()
    {
        ballsLeftToSpawn = 0;
        foreach (Transform ball in ballsParent)
        {
            Destroy(ball.gameObject);
        }
    }
}
