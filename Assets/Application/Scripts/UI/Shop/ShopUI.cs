using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField]
    private ShopScroller scroller;
    [SerializeField]
    private Image scrollIndicatorImage;
    [SerializeField]
    private Transform infosTransform;

    [Header("Unlock sequence")]
    [SerializeField]
    private AnimationCurve unlockHopsTimeCurve;
    [SerializeField]
    private int unlockHops;

    private Color scrollIndicatorDisabledColor;
    private Image[] scrollIndicators;
    private GameObject currentCatalogObject;
    private GameObject currentInfoObject;

    private ShopModel model;
    private BallSystem ballSystem;
    private GameCurrencySystem gameCurrencySystem;
    private MonetizationSystem monetization;

    public void Initialize(ShopModel model, BallSystem ballSystem, GameCurrencySystem gameCurrencySystem, MonetizationSystem monetization)
    {
        this.model = model;
        this.ballSystem = ballSystem;
        this.gameCurrencySystem = gameCurrencySystem;
        this.monetization = monetization;

        int screensCount = model.CatalogScreens.Length;

        scrollIndicatorDisabledColor = scrollIndicatorImage.color;        
        scrollIndicators = new Image[screensCount];
        scrollIndicators[0] = scrollIndicatorImage;
        for (int i = 1; i < screensCount; i++)
        {
            scrollIndicators[i] = Instantiate(scrollIndicatorImage.gameObject, scrollIndicatorImage.transform.parent).GetComponent<Image>();
        }

        scroller.Initialize(screensCount, DisplayCategory);
    }

    private void DisplayCategory(int categoryIndex)
    {
        var screen = model.CatalogScreens[categoryIndex];

        DestroyImmediate(currentCatalogObject);
        currentCatalogObject = Instantiate(screen.catalogPrefab, scroller.ScrolledTransform);

        DestroyImmediate(currentInfoObject);
        currentInfoObject = Instantiate(screen.infoPrefab, infosTransform);        

        for (int i = 0; i < scrollIndicators.Length; i++)
        {
            scrollIndicators[i].color = (i == categoryIndex) ? screen.titleTextColor : scrollIndicatorDisabledColor;
        }

        var shopTexts = GetComponentsInChildren<ShopText>(true);
        foreach (var shopText in shopTexts)
        {
            switch (shopText.Id)
            {
                case ShopText.TextPlacement.Title:
                    shopText.SetText(screen.titleText);
                    shopText.SetColor(screen.titleTextColor);
                    break;
                case ShopText.TextPlacement.Subtitle:
                    shopText.SetText(screen.subtitleText);
                    shopText.SetColor(screen.subtitleTextColor);
                    break;
                case ShopText.TextPlacement.UnlockPrice:
                    shopText.SetText(screen.priceOfBall.ToString());
                    break;
            }
        }

        var itemButtons = GetComponentsInChildren<ShopItemButton>(true);
        var lockedItemButtons = itemButtons.Where(x => !ballSystem.IsBallUnlocked(categoryIndex, x.Index)).ToArray();

        foreach (var itemButton in itemButtons)
        {
            bool isUnlocked = ballSystem.IsBallUnlocked(categoryIndex, itemButton.Index);

            itemButton.IsHighlighted = false;
            itemButton.IsSelected = ballSystem.IsBallSelected(categoryIndex, itemButton.Index);
            itemButton.ItemTexture = isUnlocked ? model.BallTextures[itemButton.Index] : model.BallTextureUnknown;

            itemButton.SetMainOnClickAction(() =>
            {
                if (isUnlocked)
                {
                    ballSystem.SelectBall(categoryIndex, itemButton.Index);
                    foreach (var itemButtonRe in itemButtons)
                    {
                        itemButtonRe.IsSelected = ballSystem.IsBallSelected(categoryIndex, itemButtonRe.Index);
                    }
                }
            });
        }

        var unlockButtons = infosTransform.GetComponentsInChildren<ShopUnlockButton>(true);
        foreach (var unlockButton in unlockButtons)
        {
            unlockButton.SetMainOnClickAction(() =>
            {
                if (gameCurrencySystem.IsEnough(GameCurrency.Coins, screen.priceOfBall))
                {
                    StartCoroutine(UnlockRandomBall(lockedItemButtons, (ballIndex) =>
                    {
                        gameCurrencySystem.Spend(GameCurrency.Coins, screen.priceOfBall);

                        ballSystem.UnlockBall(categoryIndex, ballIndex);
                        ballSystem.SelectBall(categoryIndex, ballIndex);
                        DisplayCategory(categoryIndex);
                    }));
                }
                else
                {
                    var notEnoughCoins = FindObjectOfType<NotEnoughCoinsOverlay>();
                    notEnoughCoins.SetOnShowCallback(overlayObject =>
                    {
                        monetization.ConnectAllVideoRewardTexts(overlayObject.transform);
                        monetization.ConnectAllVideoRewardButtons(overlayObject.transform, (_) => notEnoughCoins.Hide());
                    });
                    notEnoughCoins.Show();
                }
            });
        }

        monetization.ConnectAllVideoRewardTexts(transform);
        monetization.ConnectAllVideoRewardButtons(transform, (rewardType) =>
        {
            if (rewardType == VideoRewardType.UnlockBall)
            {
                StartCoroutine(UnlockRandomBall(lockedItemButtons, (ballIndex) =>
                {
                    ballSystem.UnlockBall(categoryIndex, ballIndex);
                    ballSystem.SelectBall(categoryIndex, ballIndex);
                    DisplayCategory(categoryIndex);
                }));
            }
        });
    }

    private IEnumerator UnlockRandomBall(ShopItemButton[] lockedItemsArray, System.Action<int> onComplete)
    {
        int lockedItemsTotal = lockedItemsArray.Length;

        // Cannot unlock when nothing's locked.
        // Don't trigger callback, since there was nothing to spend coins on.
        if (lockedItemsTotal == 0)
        {
            yield break;
        }

        // Immediately unlock item if there is only one.
        if (lockedItemsTotal == 1)
        {
            onComplete(lockedItemsArray[0].Index);
            yield break;
        }

        // Disable all buttons while animation is in progress.
        var allButtons = GetComponentsInChildren<Button>();
        foreach (var button in allButtons)
        {
            button.interactable = false;
        }

        // Animate hops of highlight using the animation curve.
        int highlightedindex = Random.Range(0, lockedItemsTotal);        
        for (int i = 0; i < unlockHops; i++)
        {
            int newHighlightedIndex = Random.Range(0, lockedItemsTotal - 1);
            if (newHighlightedIndex >= highlightedindex)
            {
                newHighlightedIndex++;
            }

            lockedItemsArray[highlightedindex].IsHighlighted = false;
            highlightedindex = newHighlightedIndex;
            lockedItemsArray[highlightedindex].IsHighlighted = true;

            yield return new WaitForSecondsRealtime(unlockHopsTimeCurve.Evaluate((float)i / unlockHops));
        }

        lockedItemsArray[highlightedindex].IsHighlighted = false;

        // Enable back all buttons, since animation is finished.
        foreach (var button in allButtons)
        {
            button.interactable = true;
        }

        onComplete(lockedItemsArray[highlightedindex].Index);
    }
}
