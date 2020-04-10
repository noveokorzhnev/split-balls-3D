using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensOperator : MonoBehaviour
{
    [System.Serializable]
    public struct StateScreen
    {
        public GameState state;
        public GameObject @object;
    }

    [SerializeField]
    private StateScreen[] screens;

    public void ShowScreenForState(GameState state)
    {
        foreach (var screen in screens)
        {
            if (screen.@object != null)
            {
                screen.@object.SetActive(screen.state == state);
            }
        }
    }
}
