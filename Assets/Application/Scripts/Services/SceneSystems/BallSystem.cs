using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBallSpawner
{
    event System.Func<Vector3, float, Ball> spawnBallAtPositionWithScale;
}

public interface IBallDestroyer
{
    event System.Action onBallDestroyed;
}

public class BallSystem : INoMoreBallsTrigger
{
    private const string itemsType = "balls";

    private int ballCategoriesTotal;
    private bool[][] unlockedBallsData;

    private System.Func<int, int, GameObject> getBallPrefab;
    private System.Func<int> getSelectedBallCategory;
    private System.Func<int> getSelectedBallIndex;
    private System.Action<int> setSelectedBallCategory;
    private System.Action<int> setSelectedBallIndex;

    private GameZone gameZone;
    private BallCollector ballCollector;

    private GameObject ballPrefab;
    private int ballsTotal;

    public event System.Action onNoMoreBalls;

    public BallSystem(BallPrefabs ballPrefabs, StageSystem stageSystem)
    {
        stageSystem.AddNoMoreBallsTrigger(this);

        getBallPrefab = ballPrefabs.GetPrefab;
        ballCategoriesTotal = ballPrefabs.GetGroupsCount();

        getSelectedBallCategory = () => PersistentData.GetIntDecoded(GetType(), 0);
        getSelectedBallIndex = () => PersistentData.GetIntDecoded(GetType(), 1);
        setSelectedBallCategory = x => PersistentData.SetIntEncoded(GetType(), 0, x);
        setSelectedBallIndex = x => PersistentData.SetIntEncoded(GetType(), 1, x);

        UnlockBall(0, 0);

        gameZone = Object.FindObjectOfType<GameZone>();
        ballCollector = Object.FindObjectOfType<BallCollector>();
    }

    private void UpdateUnlockedBallsData()
    {
        unlockedBallsData = PersistentData.GetUnlockedItems(itemsType, ballCategoriesTotal);
    }

    public bool IsBallUnlocked(int categoryIndex, int ballIndex)
    {
        if (categoryIndex < 0 || categoryIndex >= unlockedBallsData.Length)
        {
            return false;
        }

        var dataArray = unlockedBallsData[categoryIndex];
        return dataArray != null && ballIndex < dataArray.Length && dataArray[ballIndex];
    }

    public void UnlockBall(int categoryIndex, int ballIndex)
    {
        PersistentData.UnlockItem(itemsType, categoryIndex, ballIndex);
        UpdateUnlockedBallsData();
    }

    public bool IsBallSelected(int categoryIndex, int ballIndex)
    {
        return 
            categoryIndex == getSelectedBallCategory() && 
            ballIndex == getSelectedBallIndex();
    }

    public void SelectBall(int categoryIndex, int ballIndex)
    {
        setSelectedBallCategory(categoryIndex);
        setSelectedBallIndex(ballIndex);
    }

    public void SpawnFirstBall()
    {
        ballPrefab = getBallPrefab(getSelectedBallCategory(), getSelectedBallIndex());

        gameZone.SpawnFirstBall(ballPrefab);
        ballsTotal = 1;
    }

    public void SpawnBallsInCollector(int amount)
    {
        ballCollector.SpawnBalls(ballPrefab, amount);
    }

    public void ClearCollector()
    {
        ballCollector.Clear();
    }

    public void AddBallSpawner(IBallSpawner spawner)
    {
        spawner.spawnBallAtPositionWithScale += SpawnBallAtPositionWithScale;
    }

    public void RemoveBallSpawner(IBallSpawner spawner)
    {
        spawner.spawnBallAtPositionWithScale -= SpawnBallAtPositionWithScale;
    }

    public void AddBallDestroyer(IBallDestroyer destroyer)
    {
        destroyer.onBallDestroyed += NoticeBallDestroyed;
    }

    public void RemoveBallDestroyer(IBallDestroyer destroyer)
    {
        destroyer.onBallDestroyed -= NoticeBallDestroyed;
    }

    private Ball SpawnBallAtPositionWithScale(Vector3 position, float scale)
    {
        var ball = gameZone.SpawnBall(ballPrefab, position, scale);

        ballsTotal++;
        return ball;
    }

    private void NoticeBallDestroyed()
    {
        ballsTotal--;
        if (ballsTotal < 1)
        {
            onNoMoreBalls?.Invoke();
        }
    }    
}
