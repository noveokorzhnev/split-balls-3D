using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonetizationSettings", menuName = "Game Resources/Settings/Monetization")]
public class MonetizationSettings : ScriptableObject
{
    [SerializeField]
    private int adsVideoRewardCoins;
    [SerializeField]
    private int adsVideoMultiplyCoinsModifier;

    [SerializeField]
    private int minCoinsForVictory;
    [SerializeField]
    private int maxCoinsForVictory;

    public int AdsVideoRewardCoins => adsVideoRewardCoins;
    public int AdsVideoMultiplyCoinsModifier => adsVideoMultiplyCoinsModifier;
    public (int, int) RangeOfCoinsForVictory => (minCoinsForVictory, maxCoinsForVictory);
}
