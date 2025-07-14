using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public TileState state = TileState.Unknown;

    [SerializeField] private GameObject oImage;
    [SerializeField] private GameObject xImage;
    [SerializeField] private GameObject highlightImage;
    [SerializeField] private SpriteRenderer highlightSpriteRdr;

    private void Awake()
    {
        highlightSpriteRdr = highlightImage.GetComponent<SpriteRenderer>();
    }


    public void SetState(TileState newState)
    {
        state = newState;
        if (oImage != null)
        {
            oImage.SetActive(state == TileState.O);
        }

        if (xImage != null)
        {
            xImage.SetActive(state == TileState.X);
        }
    }
    public void Highlight()
    {
        if (highlightImage != null)
        {
            SetAnimationHightlight();
        }
    }
    private void SetAnimationHightlight()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(highlightSpriteRdr.DOFade(1, 1));
        sequence.Append(highlightSpriteRdr.DOFade(0, 1));
    }

    private void OnMouseDown()
    {
        BoardController.Instance.OnTileClicked(this);
    }
}
