using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenRotation : ScriptableAnimation
{
    [Header("Details")]
    [SerializeField]
    private Vector3 fromValue;
    [SerializeField]
    private Vector3 toValue;
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
        transform.localEulerAngles = fromValue;

        var tween = transform.DOLocalRotate(toValue, time).SetEase(ease);
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
