using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoRewardButton : ManagedButton
{   
    [SerializeField]
    private VideoRewardType type;
    public VideoRewardType Type => type;  

}
