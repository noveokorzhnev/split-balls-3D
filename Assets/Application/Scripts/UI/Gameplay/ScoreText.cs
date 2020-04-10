using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreText : ManagedText
{
    private int mValue;
    public int Value
    {
        get => mValue;
        set
        {
            mValue = value;
            SetText(value.ToString());

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.15f, 0.3f);
        }
    }
}
