using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopText : ManagedText
{
    [System.Serializable]
    public enum TextPlacement
    {
        Undefined,
        Title,
        Subtitle,
        UnlockPrice
    }

    [SerializeField]
    private TextPlacement idPlacement;
    public TextPlacement Id => idPlacement;
}
