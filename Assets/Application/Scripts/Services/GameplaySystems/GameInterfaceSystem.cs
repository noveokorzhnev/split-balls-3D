using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInterfaceSystem : IGameplaySystem
{
    private StageSystem stageSystem;

    private ScoreText[] scoreTexts;
    private ProgressIndicator progressIndicator;

    private System.Action finishAction;

    public GameInterfaceSystem(StageSystem stageSystem, GameInteractionsSystem interactionsSystem)
    {
        this.stageSystem = stageSystem;

        interactionsSystem.onFinishTriggered += NoticeFinishTriggered;
        stageSystem.onFinishCurrentSubstage += () => finishAction?.Invoke();
    }

    public void OnGameplayStarted()
    {
        scoreTexts = Object.FindObjectsOfType<ScoreText>();
        progressIndicator = Object.FindObjectOfType<ProgressIndicator>();

        foreach (var scoreText in scoreTexts)
        {
            scoreText.Value = 0;
        }

        int currentStage;
        int currentSubstage;
        int totalSubstages;

        stageSystem.GetCurrentInfo(out currentStage, out currentSubstage, out totalSubstages);

        bool isBossLevel = currentStage % 2 == 1;
        progressIndicator.Show(isBossLevel);

        if (!isBossLevel)
        {
            progressIndicator.SetLevelsGroupNumber(1 + currentStage / 2);
            progressIndicator.SetLevelsPassedFraction((float)currentSubstage / totalSubstages);

            finishAction = () => progressIndicator.SetLevelsPassedFraction((float)(currentSubstage + 1) / totalSubstages);
        }
    }

    public void OnGameplayStopped()
    {

    }

    private void NoticeFinishTriggered()
    {
        foreach (var scoreText in scoreTexts)
        {
            scoreText.Value++;
        }
    }
}
