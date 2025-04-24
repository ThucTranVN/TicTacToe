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
        string path = $"Images/Boards/{type}"; // L?y ?nh t? Resources/Images/Boards/{BoardType}
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null)
        {
            boardImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Không tìm th?y sprite: {path}");
        }
        boardLabel.text = type.ToString(); // Hi?n th? tên lo?i b?ng
    }
}