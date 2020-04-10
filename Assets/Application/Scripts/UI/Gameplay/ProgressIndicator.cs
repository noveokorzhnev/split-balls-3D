using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject groupNormalLevels;
    [SerializeField]
    private GameObject groupBossLevel;

    [SerializeField]
    private TMP_Text normalLevelsGroupNumber;
    [SerializeField]
    private Image normalLevelsPassedIndicatorsMask;

    public void Show(bool isBoss)
    {
        groupBossLevel.SetActive(isBoss);
        groupNormalLevels.SetActive(!isBoss);
    }

    public void SetLevelsGroupNumber(int number)
    {
        normalLevelsGroupNumber.text = number.ToString();
    }

    public void SetLevelsPassedFraction(float fraction)
    {
        normalLevelsPassedIndicatorsMask.fillAmount = fraction;
    }
}
