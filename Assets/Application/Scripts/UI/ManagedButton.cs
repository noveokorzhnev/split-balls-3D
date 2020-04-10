using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ManagedButton : MonoBehaviour
{
    private Button mButton;
    protected Button Button
    {
        get
        {
            if (mButton == null)
            {
                mButton = GetComponent<Button>();
            }
            return mButton;
        }
    }

    protected System.Action mainClickAction;
    protected System.Action additionalClickAction;

    protected virtual void Awake()
    {
        Button.onClick.AddListener(() => 
        { 
            mainClickAction?.Invoke();
            additionalClickAction?.Invoke();
        });
    }

    public virtual void SetMainOnClickAction(System.Action action)
    {
        mainClickAction = action;
    }

    public virtual void AddSecondaryOnClickAction(System.Action action)
    {
        additionalClickAction += action;
    }
}
