using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ShopScroller : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Scroll System")]
    [SerializeField]
    private RectTransform scrolledTransformCenter;
    [SerializeField]
    private RectTransform touchRef;
    [SerializeField]
    private float xInterval;
    [SerializeField]
    private float xThreshold;
    [SerializeField]
    private GameObject groupInfo;

    [Header("Auto-Focus Settings")]
    [SerializeField]
    private float normalInertiaDecaySpeed;
    [SerializeField]
    private float minInertia;
    [SerializeField]
    private float releaseFocusTime;
    [SerializeField]
    private AnimationCurve releaseFocusEase;
    
    private const int inertiaBufferSize = 15;
    private Queue<float> manualOffsetsBuffer = new Queue<float>(inertiaBufferSize);

    private float currentOffset;
    private float initialDragOffset;
    private float normalInertia;

    private int displayedItemIndex;
    private int itemsTotal;
    private System.Action<int> onDisplayItem;

    public Transform ScrolledTransform => scrolledTransformCenter;

    public void Initialize(int itemsTotal, System.Action<int> onDisplayItem)
    {
        this.itemsTotal = itemsTotal;
        this.onDisplayItem = onDisplayItem;

        DisplayItem();
    }

    private void DisplayItem()
    {
        onDisplayItem?.Invoke(displayedItemIndex);
    }

    private void PreviousItem()
    {
        displayedItemIndex--;
        if (displayedItemIndex < 0)
        {
            displayedItemIndex = itemsTotal - 1;
        }

        DisplayItem();
    }

    private void NextItem()
    {
        displayedItemIndex++;
        if (displayedItemIndex >= itemsTotal)
        {
            displayedItemIndex = 0;
        }

        DisplayItem();
    }

    private void Update()
    {
        if (Mathf.Abs(normalInertia) > minInertia)
        {
            currentOffset += normalInertia;
            if (normalInertia > 0)
            {
                normalInertia -= normalInertiaDecaySpeed * Time.unscaledDeltaTime;
            }
            else
            {
                normalInertia += normalInertiaDecaySpeed * Time.unscaledDeltaTime;
            }
            UpdateLayout();

            if (Mathf.Abs(normalInertia) <= minInertia)
            {
                normalInertia = 0;
                EndDragAutoFocus();
            }
        }
    }

    private void UpdateLayout()
    {
        if (currentOffset < -xInterval / 2)
        {
            currentOffset += xInterval;
            initialDragOffset -= xInterval;
            NextItem();
        }

        if (currentOffset > xInterval / 2)
        {
            currentOffset -= xInterval;
            initialDragOffset += xInterval;
            PreviousItem();
        }

        scrolledTransformCenter.anchoredPosition = new Vector2(currentOffset, scrolledTransformCenter.anchoredPosition.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        groupInfo.SetActive(false);
        normalInertia = 0;
        DOTween.Kill(this, true);

        touchRef.position = Input.mousePosition;
        initialDragOffset = touchRef.anchoredPosition.x - currentOffset;

        manualOffsetsBuffer.Clear();
        manualOffsetsBuffer.Enqueue(touchRef.anchoredPosition.x);
    }

    public void OnDrag(PointerEventData eventData)
    {
        touchRef.position = Input.mousePosition;
        currentOffset = touchRef.anchoredPosition.x - initialDragOffset;

        while (manualOffsetsBuffer.Count + 1 > inertiaBufferSize)
        {
            manualOffsetsBuffer.Dequeue();
        }
        manualOffsetsBuffer.Enqueue(touchRef.anchoredPosition.x);

        UpdateLayout();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentOffset < -xThreshold)
        {
            NextItem();
            currentOffset = xInterval / 2;
        }
        else if (currentOffset > xThreshold)
        {
            PreviousItem();
            currentOffset = -xInterval / 2;
        }

        normalInertia = 0;
        EndDragAutoFocus();
    }

    private void EndDragAutoFocus()
    {
        groupInfo.SetActive(true);
        DOTween.To(() => currentOffset, x => { currentOffset = x; UpdateLayout(); }, 0, releaseFocusTime).SetEase(releaseFocusEase).SetUpdate(true).SetId(this);
    }
}
