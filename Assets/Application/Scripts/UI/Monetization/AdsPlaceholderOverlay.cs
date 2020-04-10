using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsPlaceholderOverlay : UserInterfaceOverlay
{
    public override void Show()
    {
        base.Show();

        /*
         * In general, this should be managed by a specific script,
         * to make sure that game logics is not damaged by setting timeScale or pausing AudioListener.
         */
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public override void Hide()
    {
        base.Hide();

        /*
         * In general, this should be managed by a specific script,
         * to make sure that game logics is not damaged by setting timeScale or pausing AudioListener.
         */
        Time.timeScale = 1;
        AudioListener.pause = false;
    }  
}
