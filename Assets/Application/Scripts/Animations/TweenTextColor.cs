using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class TweenTextColor : ScriptableAnimation
{
    private TMP_Text mText;
    private TMP_Text Text
    {
        get
        {
            if (mText == null)
            {
                mText = GetComponent<TMP_Text>();
            }
            return mText;
        }
    }

    [Header("Details")]
    [SerializeField]
    private float timeToNextColor;
    [SerializeField]
    private Color[] colors;

    public override void Play()
    {
        var sq = DOTween.Sequence();
        foreach (var color in colors)
        {
            sq.AppendCallback(() => Text.color = color);
            sq.AppendInterval(timeToNextColor);
        }

        sq.SetLoops(-1, LoopType.Restart);
        sq.SetId(Text);
    }

    public override void Stop()
    {
        Text.DOKill();
    }

}
