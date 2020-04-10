using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameZone : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform ballsGroup;
    [SerializeField]
    private Collider finishTrigger;
    [SerializeField]
    private Collider destroyTrigger;

    private GameObject stageObject;

    public Collider FinishTrigger => finishTrigger;
    public Collider DestroyTrigger => destroyTrigger;

    public void CreateStage(GameObject stagePrefab)
    {
        Destroy(stageObject);
        stageObject = Instantiate(stagePrefab, transform);
    }

    public void SpawnFirstBall(GameObject prefab)
    {
        foreach (Transform ballTransform in ballsGroup)
        {
            Destroy(ballTransform.gameObject);
        }

        GameObject instance = Instantiate(prefab, ballsGroup);
        instance.transform.position = spawnPoint.position;
    }

    public Ball SpawnBall(GameObject prefab, Vector3 position, float scale)
    {
        GameObject instance = Instantiate(prefab, ballsGroup);
        instance.transform.position = position;
        instance.transform.localScale = Vector3.one * scale;

        return instance.GetComponentInChildren<Ball>();
    }
}
