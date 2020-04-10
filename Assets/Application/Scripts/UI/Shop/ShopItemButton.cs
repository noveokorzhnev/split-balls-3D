using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopItemButton : ManagedButton
{
    [SerializeField]
    private int itemIndex;
    [SerializeField]
    private Color randomHighlightColor;
    [SerializeField]
    private GameObject selectedFrame;
    [SerializeField]
    private RawImage itemImage;

    private Graphic TargetGraphic => Button.targetGraphic;

    public int Index => itemIndex;

    private bool mIsHighlighted;
    public bool IsHighlighted
    {
        get => mIsHighlighted;
        set
        {
            mIsHighlighted = value;
            TargetGraphic.color = value ? randomHighlightColor : Color.white;
        }
    }

    private bool mIsSelected;
    public bool IsSelected
    {
        get => mIsSelected;
        set
        {
            mIsSelected = value;
            selectedFrame.SetActive(value);
        }
    }

    public Texture ItemTexture
    {
        set
        {
            itemImage.texture = value;
        }
    }
}
