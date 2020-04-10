using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    private float mRotation;
    public float Rotation
    {
        get => mRotation;
        set
        {
            mRotation = value;
            transform.localEulerAngles = Vector3.up * value;
        }
    }
}
