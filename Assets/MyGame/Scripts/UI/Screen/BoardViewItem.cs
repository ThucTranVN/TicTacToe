using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoardViewItem : MonoBehaviour
{
    public Image boardImage;
    public TextMeshProUGUI boardLabel;
    private BoardType boardType;

    public float NormalizedPosition { get; set; }

    private Tween scaleTween;

    public void Init(BoardType type)
    {
        boardType = type;
        SetImage();
        boardLabel.text = type.ToString();
    }

    private void SetImage()
    {
        GlobalConfig globalConfig = DataManager.Instance.GlobalConfig;
        if (globalConfig == null)
        {
            Debug.Log("GlobalConfig chưa được gán trong DataManager.");
            return;
        }

        switch (boardType)
        {
            case BoardType.Size3x3:
                boardImage.sprite = globalConfig.spriteSize3x3;
                break;
            case BoardType.Size6x6:
                boardImage.sprite = globalConfig.spriteSize6x6;
                break;
            case BoardType.Size9x9:
                boardImage.sprite = globalConfig.spriteSize9x9;
                break;
            case BoardType.Size11x11:
                boardImage.sprite = globalConfig.spriteSize11x11;
                break;
            default:
                Debug.LogWarning($"Không tìm thấy sprite cho boardType: {boardType}");
                break;
        }
    }

    public void UpdateScale(float normalizedScrollPos)
    {
        float distance = Mathf.Abs(normalizedScrollPos - NormalizedPosition);
        float targetScale = Mathf.Clamp(1f - distance * 2f, 0.8f, 1.1f);
        AnimateScale(targetScale);
    }

    public void SetSelected(bool isSelected)
    {
        float targetScale = isSelected ? 1.1f : 1f;
        AnimateScale(targetScale);

        boardImage.color = isSelected ? Color.white : new Color(1f, 1f, 1f, 0.6f);
        boardLabel.color = isSelected ? Color.yellow : Color.gray;
    }

    private void AnimateScale(float targetScale)
    {
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
        }

        scaleTween = transform.DOScale(Vector3.one * targetScale, 0.3f)
            .SetEase(Ease.OutQuad);
    }
}
