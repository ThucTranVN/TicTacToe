using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public TileState state = TileState.Unknown;

    [SerializeField] private GameObject oImage;
    [SerializeField] private GameObject xImage;

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

    private void OnMouseDown()
    {
        BoardController.Instance.OnTileClicked(this);
    }
}
