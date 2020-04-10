using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedTextWithCurrencyId : ManagedText
{
    [SerializeField]
    private GameCurrency idCurrency;
    public GameCurrency Id => idCurrency;
}
