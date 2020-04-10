using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 beginDragScreenPosition;

    public event System.Action onBeginPull;
    public event System.Action<float> onPullHorizontal;
    public event System.Action<float> onPullVertical;

    public bool IsDragging { get; set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginDragScreenPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        onBeginPull?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var currentScreenPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

        onPullHorizontal?.Invoke(currentScreenPosition.x - beginDragScreenPosition.x);
        onPullVertical?.Invoke(currentScreenPosition.y - beginDragScreenPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
