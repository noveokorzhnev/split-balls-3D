using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyCollector
{
    event System.Action onKeyCollected;
}

public class KeysSystem
{
    private const string itemsType = "keys";

    private int stagesTotal;
    private bool[][] takenKeysData;

    private System.Func<int> getTakenKeysAmount;
    private System.Action<int> setTakenKeysAmount;

    private int currentStage;
    private int currentSubstage;

    public KeysSystem(StagePrefabs stagePrefabs)
    {
        stagesTotal = stagePrefabs.GetGroupsCount();

        getTakenKeysAmount = () => PersistentData.GetIntDecoded(GetType(), 0);
        setTakenKeysAmount = x => PersistentData.SetIntEncoded(GetType(), 0, x);

        UpdateTakenKeysData();
    }

    private void UpdateTakenKeysData()
    {
        takenKeysData = PersistentData.GetUnlockedItems(itemsType, stagesTotal);
    }

    public void SetAndTestCurrentStageAndSubstage(int stageIndex, int substageIndex, out bool isKeyTaken)
    {
        if (stageIndex < 0 || stageIndex >= takenKeysData.Length)
        {
            isKeyTaken = false;
            return;
        }

        currentStage = stageIndex;
        currentSubstage = substageIndex;

        var dataArray = takenKeysData[stageIndex];
        isKeyTaken = dataArray != null && substageIndex < dataArray.Length && dataArray[substageIndex];
    }

    public void AddKeyCollector(IKeyCollector collector)
    {
        collector.onKeyCollected += NoticeKeyCollected;
    }

    public void RemoveKeyCollector(IKeyCollector collector)
    {
        collector.onKeyCollected -= NoticeKeyCollected;
    }

    private void NoticeKeyCollected()
    {
        PersistentData.UnlockItem(itemsType, currentStage, currentSubstage);
        UpdateTakenKeysData();

        setTakenKeysAmount(getTakenKeysAmount() + 1);
    }

    public bool IsEnough(int amount)
    {
        return getTakenKeysAmount() >= amount;
    }

    public void Spend(int amount)
    {
        setTakenKeysAmount(getTakenKeysAmount() - amount);
    }
}
