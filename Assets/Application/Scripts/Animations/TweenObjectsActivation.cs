using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenObjectsActivation : ScriptableAnimation
{
    [System.Serializable]
    public struct Frame
    {
        public GameObject[] activated;
        public GameObject[] deactivated;
    }

    [Header("Details")]
    [SerializeField]
    private Frame[] frames;
    [SerializeField]
    private float frameTime;
    [SerializeField]
    private Ease ease = Ease.Linear;
    [SerializeField]
    private bool loop;
    [SerializeField]
    private LoopType loopType;

    private void UseFrame(Frame frame)
    {
        foreach (var @object in frame.activated)
        {
            @object.SetActive(true);
        }
        foreach (var @object in frame.deactivated)
        {
            @object.SetActive(false);
        }
    }

    public override void Play()
    {
        if (frames.Length > 0)
        {
            var sq = DOTween.Sequence().SetId(this);

            foreach (var frame in frames)
            {
                sq.AppendCallback(() => UseFrame(frame));
                sq.AppendInterval(frameTime);
            }

            sq.SetEase(ease);
            if (loop)
            {
                sq.SetLoops(-1, loopType);
            }
        }
    }

    public override void Stop()
    {
        if (frames.Length > 0)
        {
            DOTween.Kill(this);
        }
    }    
}
