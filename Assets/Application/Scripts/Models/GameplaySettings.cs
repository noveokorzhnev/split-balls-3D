using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Game Resources/Settings/Gameplay")]
public class GameplaySettings : ScriptableObject
{
    [Header("Controls")]   
    [Tooltip("Conversion from horizontal pull value to rotation value")]
    [SerializeField]
    private float pullToRotationRate;
    [Tooltip("Conversion from vertical pull value to wind force value")]
    [SerializeField]
    private float pullToWindForceRate;
    [Tooltip("In local euler angles")]
    [SerializeField]
    private float maxCameraRigRotation;

    [Header("Physics")]
    [Tooltip("Constant force of gravity")]
    [SerializeField]
    private float gravity;
    [Tooltip("Max horizontal force applied to dynamic rigidbodies")]
    [SerializeField]
    private float maxSideForce;

    [Header("View")]
    [SerializeField]
    private Material localWindZoneMaterial;
    [SerializeField]
    private Gradient localWindZoneGradient;
    
    public float MaxCameraRigRotation => maxCameraRigRotation;
    public float PullToRotationRate => pullToRotationRate;
    public float PullToWindForceRate => pullToWindForceRate;
    public float Gravity => gravity;
    public float MaxSideForce => maxSideForce;

    public void ApplyLocalWindZoneColor(float windForce01)
    {
        localWindZoneMaterial.color = localWindZoneGradient.Evaluate(windForce01);
    }
}
