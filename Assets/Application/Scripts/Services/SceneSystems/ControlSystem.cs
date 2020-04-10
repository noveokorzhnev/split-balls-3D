using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSystem
{
    private GameplaySettings settings;
    private TouchControl touchControl;
    private CameraRig cameraRig;

    private float initialRotation;
    private float initialWindForce;
    private float clampedWindForce;

    public ControlSystem(GameplaySettings gameplaySettings)
    {
        settings = gameplaySettings;
        Physics.gravity = new Vector3(0, -settings.Gravity, 0);

        touchControl = Object.FindObjectOfType<TouchControl>();
        touchControl.onBeginPull += HandleBeginPull;
        touchControl.onPullHorizontal += HandlePullHorizontal;
        touchControl.onPullVertical += HandlePullVertical;

        cameraRig = Object.FindObjectOfType<CameraRig>();
        LocalWindZone.getWindForce += () => clampedWindForce;
    }

    public void Reset()
    {
        cameraRig.Rotation = 0;
        initialWindForce = 0;
        clampedWindForce = 0;
        Physics.gravity = new Vector3(0, -settings.Gravity, 0);
        touchControl.OnBeginDrag(null);
    }

    public void AddCallbackOnBeginPull(System.Action callback)
    {
        touchControl.onBeginPull += callback;
    }

    private void HandleBeginPull()
    {
        initialRotation = cameraRig.Rotation;
    }

    private void HandlePullHorizontal(float pullValue)
    {
        float rotationValue = initialRotation + pullValue * settings.PullToRotationRate;
        float clampedRotationValue = Mathf.Clamp(rotationValue, -settings.MaxCameraRigRotation, settings.MaxCameraRigRotation);
        float sideForce = settings.MaxSideForce * (clampedRotationValue / settings.MaxCameraRigRotation);

        cameraRig.Rotation = clampedRotationValue;
        Physics.gravity = new Vector3(sideForce, -settings.Gravity, 0);
    }

    private void HandlePullVertical(float pullValue)
    {
        float windForce = initialWindForce + pullValue * settings.PullToWindForceRate;
        clampedWindForce = Mathf.Clamp01(windForce);

        settings.ApplyLocalWindZoneColor(clampedWindForce);
    }
}
