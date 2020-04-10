using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum VideoRewardType
{
    Coins,
    ExtraBall,
    UnlockBall,
    SellBalls
}

public class MonetizationSystem
{
    private AdsPlaceholderOverlay adsOverlay;

    private System.Func<GameCurrency, int> getVideoRewardCurrencyAmount;
    private System.Func<GameCurrency, int> getVideoRewardMultiplyCurrencyBufferModifier;

    private System.Action<GameCurrency> addVideoRewardCurrency;
    private System.Action<GameCurrency> multiplyCurrencyBuffer;
    private System.Func<(int, int)> getRangeOfCoinsForVictory;

    private System.Action<VideoRewardType> advanceToStateCallback;

    public MonetizationSystem(MonetizationSettings monetizationSettings, GameCurrencySystem currencySystem)
    {
        adsOverlay = Object.FindObjectOfType<AdsPlaceholderOverlay>();

        getVideoRewardCurrencyAmount = (currency) =>
        {
            switch (currency)
            {
                default:
                case GameCurrency.Coins:
                    return monetizationSettings.AdsVideoRewardCoins;
            }
        };
        getVideoRewardMultiplyCurrencyBufferModifier = (currency) =>
        {
            switch (currency)
            {
                default:
                case GameCurrency.Coins:
                    return monetizationSettings.AdsVideoMultiplyCoinsModifier;
            }
        };

        addVideoRewardCurrency = (currency) => currencySystem.Add(currency, getVideoRewardCurrencyAmount(currency));
        multiplyCurrencyBuffer = (currency) => currencySystem.MultiplyBuffer(currency, getVideoRewardMultiplyCurrencyBufferModifier(currency));
        getRangeOfCoinsForVictory = () => monetizationSettings.RangeOfCoinsForVictory;

        // Connect all persistent scene elements.
        ConnectAllVideoRewardTexts(null);
        ConnectAllVideoRewardButtons(null);
    }

    public int GetCoinsForVictory()
    {
        var range = getRangeOfCoinsForVictory();
        return Random.Range(range.Item1, range.Item2);
    }

    public void ConnectAllVideoRewardTexts(Transform parent)
    {
        var allTexts = parent != null ?
            parent.GetComponentsInChildren<ManagedTextWithCurrencyId>(true) :
            Resources.FindObjectsOfTypeAll<ManagedTextWithCurrencyId>();

        foreach (var text in allTexts)
        {
            if (text is VideoRewardCurrencyText)
            {
                text.SetText(getVideoRewardCurrencyAmount(text.Id).ToString());
            }
            else if (text is VideoRewardMultiplyCoinsText)
            {
                text.SetText("X" + getVideoRewardMultiplyCurrencyBufferModifier(text.Id));
            }
        }
    }

    public void ConnectAllVideoRewardButtons(Transform parent, System.Action<VideoRewardType> onCloseCallback = null)
    {        
        var allButtons = parent != null ? 
            parent.GetComponentsInChildren<VideoRewardButton>(true) : 
            Resources.FindObjectsOfTypeAll<VideoRewardButton>(); 

        foreach (var button in allButtons)
        {
            button.SetMainOnClickAction(() =>
            {
                adsOverlay.SetOnHideCallback(() =>
                {
                    switch (button.Type)
                    {
                        case VideoRewardType.Coins:
                            addVideoRewardCurrency(GameCurrency.Coins);
                            break;
                        case VideoRewardType.SellBalls:
                            multiplyCurrencyBuffer(GameCurrency.Coins);
                            break;
                    }

                    onCloseCallback?.Invoke(button.Type);
                    advanceToStateCallback?.Invoke(button.Type);
                });

                adsOverlay.Show();
            });
        }
    }

    public void SetAdvanceToStateCallbackForVideoRewardButton(System.Action<VideoRewardType> advanceToStateCallback)
    {
        this.advanceToStateCallback = advanceToStateCallback;
    }
}
