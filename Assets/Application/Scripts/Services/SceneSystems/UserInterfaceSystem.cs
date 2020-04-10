using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserInterfaceSystem
{
    private ScreensOperator screens;
    private GameStateButton[] gameStateButtons;
    private Dictionary<GameCurrency, CurrencyText[]> currencyTexts;
    private Dictionary<GameCurrency, BufferCurrencyText[]> bufferCurrencyTexts;

    public UserInterfaceSystem()
    {
        screens = Object.FindObjectOfType<ScreensOperator>();
        gameStateButtons = Resources.FindObjectsOfTypeAll<GameStateButton>();

        currencyTexts = Resources.FindObjectsOfTypeAll<CurrencyText>()
            .GroupBy(text => text.Id)
            .ToDictionary(group => group.Key, group => group.ToArray());

        bufferCurrencyTexts = Resources.FindObjectsOfTypeAll<BufferCurrencyText>()
            .GroupBy(text => text.Id)
            .ToDictionary(group => group.Key, group => group.ToArray());
    }    

    public void OnEnterState(GameState state)
    {
        screens.ShowScreenForState(state);
    }

    public void RegisterAdvanceToStateAction(System.Action<GameState> advanceToStateAction)
    {
        foreach (var button in gameStateButtons)
        {
            button.RegisterAdvanceAction(advanceToStateAction);
        }
    }

    public void DisplayCurrency(GameCurrency currency, int value)
    {
        if (currencyTexts.ContainsKey(currency))
        {
            foreach (var text in currencyTexts[currency])
            {
                text.SetText(value.ToString());
            }
        }
    }

    public void DisplayBufferCurrency(GameCurrency currency, int value)
    {
        if (bufferCurrencyTexts.ContainsKey(currency))
        {
            foreach (var text in bufferCurrencyTexts[currency])
            {
                text.SetText(value.ToString());
            }
        }
    }
}
