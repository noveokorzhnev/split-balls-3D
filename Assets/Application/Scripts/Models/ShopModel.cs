using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopModel", menuName = "Game Resources/Models/Shop")]
public class ShopModel : ScriptableObject
{
    [System.Serializable]
    public struct ShopScreen
    {
        public GameObject catalogPrefab;
        public string titleText;
        public Color titleTextColor;

        public GameObject infoPrefab;
        public string subtitleText;
        public Color subtitleTextColor;

        public int priceOfBall;
    }

    [Header("Screens")]
    [SerializeField]
    private ShopScreen[] catalogScreens;

    [Header("Rendering")]
    [SerializeField]
    private Texture ballTextureUnknown;
    [SerializeField]
    private Texture[] ballTextures;

    public ShopScreen[] CatalogScreens => catalogScreens;
    public Texture BallTextureUnknown => ballTextureUnknown;
    public Texture[] BallTextures => ballTextures;
}
