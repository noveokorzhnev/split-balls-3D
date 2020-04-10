using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ManagedText : MonoBehaviour
{
    private TMP_Text mText;
    protected TMP_Text Text
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

    public virtual void SetText(string text)
    {
        Text.text = text;
    }

    public virtual void SetColor(Color color)
    {
        Text.color = color;
    }
}
