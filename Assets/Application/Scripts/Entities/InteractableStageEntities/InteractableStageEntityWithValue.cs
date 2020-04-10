using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class InteractableStageEntityWithValue : InteractableStageEntity
{
    [SerializeField]
    protected int value;


    private TMP_Text label;
    private const string FORMAT_VALUE = "%VALUE%";
    protected virtual string LabelFormat => FORMAT_VALUE;

    protected virtual void OnValueSet()
    {
        if (label)
        {
            label.text = LabelFormat.Replace(FORMAT_VALUE, value.ToString());
        }
    }

    protected override void Awake()
    {
        base.Awake();

        label = transform.Find("Attachments/Display").GetComponentInChildren<TMP_Text>();

        OnValueSet();
    }
}
