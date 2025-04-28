using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardViewItem : MonoBehaviour
{
    public Image boardImage;
    public TextMeshProUGUI boardLabel;
    private BoardType boardType;

    
    public GlobalConfig globalConfig;

    public void Init(BoardType type)
    {
        boardType = type;
        SetImage(); 
        boardLabel.text = type.ToString();
    }

    private void SetImage()
    {
        switch (boardType)
        {
            case BoardType.Size3x3:
                boardImage.sprite = globalConfig.boardSprites[0]; 
                break;
            case BoardType.Size6x6:
                boardImage.sprite = globalConfig.boardSprites[1];  
                break;
            case BoardType.Size9x9:
                boardImage.sprite = globalConfig.boardSprites[2];  
                break;
            case BoardType.Size11x11:
                boardImage.sprite = globalConfig.boardSprites[3];
                break;
            default:
                Debug.LogWarning($"Không tìm thấy sprite cho boardType: {boardType}");
                break;
        }
    }

    public void SetSelected(bool isSelected)
    {
        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
        boardImage.color = isSelected ? Color.white : new Color(1f, 1f, 1f, 0.6f);
        boardLabel.color = isSelected ? Color.yellow : Color.gray;
    }
}
