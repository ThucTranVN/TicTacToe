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
        if (!DataManager.HasInstance)
        {
            Debug.LogWarning("DataManager chưa được khởi tạo.");
            return;
        }

        switch (boardType)
        {
            case BoardType.Size3x3:
                boardImage.sprite = DataManager.Instance.GlobalConfig.spriteSize3x3;
                break;
            case BoardType.Size6x6:
                boardImage.sprite = DataManager.Instance.GlobalConfig.spriteSize6x6;
                break;
            case BoardType.Size9x9:
                boardImage.sprite = DataManager.Instance.GlobalConfig.spriteSize9x9;
                break;
            case BoardType.Size11x11:
                boardImage.sprite = DataManager.Instance.GlobalConfig.spriteSize11x11;
                break;
            default:
                Debug.LogWarning($"Không tìm thấy sprite cho boardType: {boardType}");
                break;
        }
    }

    public void UpdateScale(float normalizedScrollPos)
    {
        if (!DataManager.HasInstance) return;

        float distance = Mathf.Abs(normalizedScrollPos - NormalizedPosition);
        float targetScale = Mathf.Clamp(
            1f - distance * DataManager.Instance.GlobalConfig.scaleFactor,
            DataManager.Instance.GlobalConfig.scaleMin,
            DataManager.Instance.GlobalConfig.scaleSelected
        );
        AnimateScale(targetScale);
    }

    public void SetSelected(bool isSelected)
    {
        if (!DataManager.HasInstance) return;

        float targetScale;
        if (isSelected)
        {
            targetScale = DataManager.Instance.GlobalConfig.scaleSelected;
        }
        else
        {
            targetScale = DataManager.Instance.GlobalConfig.scaleNormal;
        }

        AnimateScale(targetScale);

        if (isSelected)
        {
            boardImage.color = DataManager.Instance.GlobalConfig.imageColorSelected;
            boardLabel.color = DataManager.Instance.GlobalConfig.labelColorSelected;
        }
        else
        {
            boardImage.color = DataManager.Instance.GlobalConfig.imageColorUnselected;
            boardLabel.color = DataManager.Instance.GlobalConfig.labelColorUnselected;
        }
    }


    private void AnimateScale(float targetScale)
    {
        if (!DataManager.HasInstance) return;

        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
        }

        scaleTween = transform.DOScale(Vector3.one * targetScale, DataManager.Instance.GlobalConfig.scaleTweenDuration).SetEase(Ease.OutQuad);
    }
}
