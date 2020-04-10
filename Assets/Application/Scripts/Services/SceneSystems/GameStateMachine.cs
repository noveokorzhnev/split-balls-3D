using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum GameState
{
    StartScreen,
    BallShop,
    Gameplay,
    NoMoreBalls,
    GetExtraBall,
    NextSubstage,
    StageFinished,
    BallsSold,
    Chests,
    PreStage
}

public class GameStateMachine : StateMachine<GameState>
{
    public GameStateMachine(
        IGameplaySystem gameplayProxy, 
        UserInterfaceSystem ui, 
        ControlSystem control, 
        StageSystem stage, 
        BallSystem ball, 
        KeysSystem keys,
        GameCurrencySystem currencies,
        MonetizationSystem monetization) : base()
    {
        // Iterate through all states and link UI (and other capable systems) to every state events.
        var allStates = (GameState[])System.Enum.GetValues(typeof(GameState));
        foreach (var state in allStates)
        {
            AddCallbackOnEnter(state, () => ui.OnEnterState(state));
        }

        // Define gameplay initialization callbacks.
        System.Action initFirstSubstageAction = stage.CreateFirstSubstage;
        System.Action initCurrentSubstageAction = stage.CreateCurrentSubstage;

        initFirstSubstageAction += ball.SpawnFirstBall;
        initCurrentSubstageAction += ball.SpawnFirstBall;

        // Link gameplay initialization systems to the correct callbacks.
        AddCallbackOnEnter(GameState.StartScreen, initFirstSubstageAction);
        AddCallbackOnEnter(GameState.PreStage, initFirstSubstageAction);
        AddCallbackOnEnter(GameState.Gameplay, control.Reset);

        AddCallbackOnTransition(GameState.NoMoreBalls, GameState.Gameplay, initCurrentSubstageAction);
        AddCallbackOnTransition(GameState.GetExtraBall, GameState.Gameplay, ball.SpawnFirstBall);
        AddCallbackOnTransition(GameState.NextSubstage, GameState.Gameplay, initCurrentSubstageAction);

        // Link currency gain system to the correct callbacks.
        AddCallbackOnEnter(GameState.StageFinished, () => 
        {
            int coinsAmount = monetization.GetCoinsForVictory();
            currencies.SetBuffer(GameCurrency.Coins, coinsAmount);
            ball.SpawnBallsInCollector(coinsAmount);
        });
        AddCallbackOnExit(GameState.BallsSold, () => 
        { 
            currencies.AddFromBuffer(GameCurrency.Coins);
            ball.ClearCollector();
        });

        // Link resolutions to the "instant pass" states.
        AddCallbackOnEnter(GameState.GetExtraBall, () => Advance(GameState.Gameplay));
        AddCallbackOnEnter(GameState.NextSubstage, () => 
        {
            bool didPassLastSubstage;
            stage.PassCurrentSubstage(out didPassLastSubstage);

            if (didPassLastSubstage)
            {
                Advance(GameState.StageFinished);
            }
            else
            {
                Advance(GameState.Gameplay);
            }
        });
        AddCallbackOnEnter(GameState.Chests, () => 
        {
            if (!keys.IsEnough(3))
            {
                Advance(GameState.PreStage);
            }
        });
        AddCallbackOnExit(GameState.Chests, () =>
        {
            if (keys.IsEnough(3))
            {
                keys.Spend(3);
            }
        });
        AddCallbackOnEnter(GameState.PreStage, () => Advance(GameState.Gameplay));

        // Link gameplay proxy to the gameplay state callbacks.
        AddCallbackOnEnter(GameState.Gameplay, gameplayProxy.OnGameplayStarted);
        AddCallbackOnExit(GameState.Gameplay, gameplayProxy.OnGameplayStopped);        

        // Link the state machine operations to UI (and other capable systems).
        ui.RegisterAdvanceToStateAction(Advance);
        control.AddCallbackOnBeginPull(() => Advance(GameState.Gameplay));
        stage.SetSubstageFailedAction(() => Advance(GameState.NoMoreBalls));
        stage.SetNextSubstageAction(() => Advance(GameState.NextSubstage));
        monetization.SetAdvanceToStateCallbackForVideoRewardButton(rewardType => 
        {
            switch (rewardType)
            {
                case VideoRewardType.ExtraBall:
                    Advance(GameState.GetExtraBall);
                    break;
                case VideoRewardType.SellBalls:
                    Advance(GameState.BallsSold);
                    break;
            }
        });

        // Register every possible transition.
        AddTransition(GameState.StartScreen, GameState.Gameplay);
        AddTransition(GameState.StartScreen, GameState.BallShop);

        AddTransition(GameState.BallShop, GameState.StartScreen);

        AddTransition(GameState.Gameplay, GameState.NoMoreBalls);
        AddTransition(GameState.Gameplay, GameState.NextSubstage);

        AddTransition(GameState.NoMoreBalls, GameState.GetExtraBall);
        AddTransition(GameState.NoMoreBalls, GameState.Gameplay);
        AddTransition(GameState.GetExtraBall, GameState.Gameplay);

        AddTransition(GameState.NextSubstage, GameState.Gameplay);
        AddTransition(GameState.NextSubstage, GameState.StageFinished);

        AddTransition(GameState.StageFinished, GameState.BallsSold);

        AddTransition(GameState.BallsSold, GameState.Chests);

        AddTransition(GameState.Chests, GameState.PreStage);

        AddTransition(GameState.PreStage, GameState.Gameplay);

        // Finally, enter the initial state...
        Enter(GameState.StartScreen);
    }
}
