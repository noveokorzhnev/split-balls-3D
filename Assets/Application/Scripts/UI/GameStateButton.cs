using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateButton : ManagedButton
{
    [SerializeField]
    private GameState stateToAdvance;

    public void RegisterAdvanceAction(System.Action<GameState> advanceToStateAction)
    {
        SetMainOnClickAction(() => advanceToStateAction(stateToAdvance));
    }
}
