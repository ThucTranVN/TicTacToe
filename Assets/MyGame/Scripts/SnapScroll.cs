using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class SnapScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float snapSpeed = 0.25f;
    public float scaleSelected = 1f;
    public float scaleOther = 0.85f;

    public Action<int> onSnapChanged; // callback khi ch?n item m?i

    private RectTransform content;
    private HorizontalLayoutGroup layoutGroup;

    private int itemCount;
    private float itemWidth;
    private float spacing;

    private bool isDragging = false;
    private int currentIndex = 0;

    private void Awake()
    {
        content = scrollRect.content;
        layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
    }

    private void Start()
    {
        itemCount = content.childCount;

        if (itemCount > 0)
        {
            itemWidth = ((RectTransform)content.GetChild(0)).rect.width;
            spacing = layoutGroup?.spacing ?? 0;
        }

    }

    private void Update()
    {
        if (!isDragging)
        {
            SnapToNearest();
        }

        UpdateScale();
    }

    private void SnapToNearest()
    {
        float contentPosX = -content.anchoredPosition.x;
        float totalWidth = itemWidth + spacing;
        int nearestIndex = Mathf.RoundToInt(contentPosX / totalWidth);
        nearestIndex = Mathf.Clamp(nearestIndex, 0, itemCount - 1);

        float targetX = nearestIndex * totalWidth;
        Vector2 newPos = new Vector2(-targetX, content.anchoredPosition.y);
        content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, newPos, snapSpeed);

        if (nearestIndex != currentIndex)
        {
            currentIndex = nearestIndex;
            onSnapChanged?.Invoke(currentIndex);
        }
    }

    private void UpdateScale()
    {
        float contentPosX = -content.anchoredPosition.x;
        float totalWidth = itemWidth + spacing;

        for (int i = 0; i < itemCount; i++)
        {
            float itemCenter = i * totalWidth;
            float distance = Mathf.Abs(itemCenter - contentPosX);
            float t = Mathf.Clamp01(distance / totalWidth);

            float scale = Mathf.Lerp(scaleSelected, scaleOther, t);
            content.GetChild(i).localScale = Vector3.Lerp(content.GetChild(i).localScale, Vector3.one * scale, 0.2f);
        }
    }

    public void SetIndex(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, itemCount - 1);
        float totalWidth = itemWidth + spacing;
        float targetX = currentIndex * totalWidth;

        content.DOAnchorPosX(-targetX, 0.4f).SetEase(Ease.InOutQuad);
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void OnBeginDrag() => isDragging = true;
    public void OnEndDrag() => isDragging = false;
}
