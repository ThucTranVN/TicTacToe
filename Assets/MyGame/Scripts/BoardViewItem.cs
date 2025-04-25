using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardViewItem : MonoBehaviour
{
    public Image boardImage;
    public TextMeshProUGUI boardLabel;
    private BoardType boardType;

    public void Init(BoardType type)
    {
        boardType = type;
        string path = $"Images/Boards/{type}";
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null)
        {
            boardImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Không tìm th?y sprite: {path}");
        }
        boardLabel.text = type.ToString();
    }

    public void SetSelected(bool isSelected)
    {
        // Scale to highlight
        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;

        // Optional: change color or outline if needed
        boardImage.color = isSelected ? Color.white : new Color(1f, 1f, 1f, 0.6f);
        boardLabel.color = isSelected ? Color.yellow : Color.gray;
    }
}
