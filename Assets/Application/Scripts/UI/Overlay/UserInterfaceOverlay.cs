using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterfaceOverlay : MonoBehaviour
{
    [SerializeField]
    protected GameObject overlayPrefab;

    protected GameObject overlayObject;

    protected System.Action<GameObject> onShowCallBack;
    protected System.Action onHideCallback;

    public virtual void SetOnShowCallback(System.Action<GameObject> callback)
    {
        onShowCallBack = callback;
    }

    public virtual void SetOnHideCallback(System.Action callback)
    {
        onHideCallback = callback;
    }

    public virtual void Show()
    {
        // Don't show if already showing.
        if (overlayObject)
        {
            return;
        }

        overlayObject = Instantiate(overlayPrefab, transform);
        onShowCallBack?.Invoke(overlayObject);

        var closeButton = overlayObject.GetComponentInChildren<HideOverlayButton>();
        if (closeButton != null)
        {
            closeButton.SetMainOnClickAction(Hide);
        }
    }

    public virtual void Hide()
    {
        Destroy(overlayObject);
        onHideCallback?.Invoke();
    }
}
