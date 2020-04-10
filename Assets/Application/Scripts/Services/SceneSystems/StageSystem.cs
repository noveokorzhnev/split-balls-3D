using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public interface IFinishTrigger
{
    event System.Action onFinishTriggered;
}

public interface INoMoreBallsTrigger
{
    event System.Action onNoMoreBalls;
}

public class StageSystem
{  
    private System.Func<int, GameObject[]> getStagePrefabs;
    private System.Func<int> getStageIndex;
    private System.Action incrementStageIndex;
    private System.Func<int, int, bool> checkIfKeyIsTaken;

    private GameZone gameZone;
    private GameObject[] stagePrefabs;
    private int stageIndex;
    private int substageIndex;
    private bool wasFinishTriggered;

    private bool allStagesComplete;

    public event System.Action onFinishCurrentSubstage;
    public event System.Action onNextSubstage;
    public event System.Action onSubstageFailed;

    public StageSystem(StagePrefabs stagePrefabs, KeysSystem keysSystem)
    {
        getStagePrefabs = stagePrefabs.GetGroup;
        getStageIndex = () => PersistentData.GetIntDecoded(GetType(), 0);
        incrementStageIndex = () => PersistentData.SetIntEncoded(GetType(), 0, getStageIndex() + 1);

        checkIfKeyIsTaken = (stage, substage) =>
        {
            bool isKeyTaken;
            keysSystem.SetAndTestCurrentStageAndSubstage(stage, substage, out isKeyTaken);
            return isKeyTaken;
        };

        gameZone = Object.FindObjectOfType<GameZone>();
    }        

    public void CreateFirstSubstage()
    {
        stageIndex = getStageIndex();
        substageIndex = 0;
        stagePrefabs = getStagePrefabs(stageIndex);

        while (stagePrefabs == null)
        {
            Debug.Log("ALL STAGES COMPLETE. REPEATING THE LAST STAGE.");
            allStagesComplete = true;
            stagePrefabs = getStagePrefabs(--stageIndex);
        }

        CreateCurrentSubstage();
    }

    public void CreateCurrentSubstage()
    {
        gameZone.CreateStage(stagePrefabs[substageIndex]);
        wasFinishTriggered = false;

        if (checkIfKeyIsTaken(stageIndex, substageIndex))
        {
            var key = Object.FindObjectOfType<Key>();
            if (key != null)
            {
                Object.Destroy(key.gameObject);
            }
        }
    }

    public void PassCurrentSubstage(out bool isLastSubstagePassed)
    {
        if (allStagesComplete)
        {
            isLastSubstagePassed = true;
            return;
        }

        substageIndex++;
        isLastSubstagePassed = substageIndex == stagePrefabs.Length;

        if (isLastSubstagePassed)
        {
            incrementStageIndex();
        }
    }

    public void GetCurrentInfo(out int stageIndex, out int substageIndex, out int totalSubstages)
    {
        stageIndex = this.stageIndex;
        substageIndex = this.substageIndex;
        totalSubstages = stagePrefabs.Length;
    }

    public void AddFinishTrigger(IFinishTrigger finishTrigger)
    {
        finishTrigger.onFinishTriggered += NoticeFinishTriggered;
    }

    public void RemoveFinishTrigger(IFinishTrigger finishTrigger)
    {
        finishTrigger.onFinishTriggered -= NoticeFinishTriggered;
    }

    public void AddNoMoreBallsTrigger(INoMoreBallsTrigger noMoreBallsTrigger)
    {
        noMoreBallsTrigger.onNoMoreBalls += NoticeNoMoreBalls;
    }

    public void RemoveNoMoreBallsTrigger(INoMoreBallsTrigger noMoreBallsTrigger)
    {
        noMoreBallsTrigger.onNoMoreBalls -= NoticeNoMoreBalls;
    }

    public void SetNextSubstageAction(System.Action action)
    {
        onNextSubstage = action;
    }

    public void SetSubstageFailedAction(System.Action action)
    {
        onSubstageFailed = action;
    }

    private void NoticeFinishTriggered()
    {
        wasFinishTriggered = true;
    }

    private void NoticeNoMoreBalls()
    {
        if (wasFinishTriggered)
        {
            onFinishCurrentSubstage?.Invoke();

            DOTween.Sequence().InsertCallback(2f, () => onNextSubstage?.Invoke());            
        }
        else
        {
            onSubstageFailed?.Invoke();
        }
    }
}
