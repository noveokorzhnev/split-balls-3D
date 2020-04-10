using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSystem
{
    public ShopSystem(ShopModel shopModel, BallSystem ballSystem, GameCurrencySystem gameCurrencySystem, MonetizationSystem monetization)
    {
        var shopInterface = Object.FindObjectOfType<ScreensOperator>().GetComponentInChildren<ShopUI>(true);

        shopInterface.Initialize(shopModel, ballSystem, gameCurrencySystem, monetization);
    }
}
