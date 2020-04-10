using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenLocalPositionX : ScriptableAnimation
{
    [Header("Details")]
    [SerializeField]
    private float fromValue;
    [SerializeField]
    private float toValue;
    [SerializeField]
    private float time;
    [SerializeField]
    private Ease ease = Ease.Linear;
    [SerializeField]
    private bool loop;
    [SerializeField]
    private LoopType loopType;

    public override void Play()
    {
        transform.localPosition = new Vector3(fromValue, transform.localPosition.y, transform.localPosition.z);

        var tween = transform.DOLocalMoveX(toValue, time).SetEase(ease);
        if (loop)
        {
            tween.SetLoops(-1, loopType);
        }
    }

    public override void Stop()
    {
        transform.DOKill();
    }
}
