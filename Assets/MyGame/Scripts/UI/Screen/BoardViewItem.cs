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
        GlobalConfig config = DataManager.Instance.GlobalConfig;
        if (config == null)
        {
            Debug.Log("GlobalConfig chưa được gán trong DataManager.");
            return;
        }

        switch (boardType)
        {
            case BoardType.Size3x3:
                boardImage.sprite = config.spriteSize3x3;
                break;
            case BoardType.Size6x6:
                boardImage.sprite = config.spriteSize6x6;
                break;
            case BoardType.Size9x9:
                boardImage.sprite = config.spriteSize9x9;
                break;
            case BoardType.Size11x11:
                boardImage.sprite = config.spriteSize11x11;
                break;
            default:
                Debug.LogWarning($"Không tìm thấy sprite cho boardType: {boardType}");
                break;
        }
    }

    public void UpdateScale(float normalizedScrollPos)
    {
        GlobalConfig config = DataManager.Instance.GlobalConfig;
        float distance = Mathf.Abs(normalizedScrollPos - NormalizedPosition);
        float targetScale = Mathf.Clamp(1f - distance * config.scaleFactor, config.scaleMin, config.scaleSelected);
        AnimateScale(targetScale);
    }

    public void SetSelected(bool isSelected)
    {
        GlobalConfig config = DataManager.Instance.GlobalConfig;
        float targetScale = isSelected ? config.scaleSelected : config.scaleNormal;
        AnimateScale(targetScale);

        boardImage.color = isSelected ? config.imageColorSelected : config.imageColorUnselected;
        boardLabel.color = isSelected ? config.labelColorSelected : config.labelColorUnselected;
    }

    private void AnimateScale(float targetScale)
    {
        GlobalConfig config = DataManager.Instance.GlobalConfig;

        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
        }

        scaleTween = transform.DOScale(Vector3.one * targetScale, config.scaleTweenDuration)
            .SetEase(Ease.OutQuad);
    }
}
