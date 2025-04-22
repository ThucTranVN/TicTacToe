using UnityEngine;

public class BoardController : BaseManager<BoardController>
{
    public GameObject tilePrefab;
    public int width;
    public int height;
    public int borderSize;
    private Tile[,] tiles;
    public void InitBoard(BoardType boardType)
    {
        SetupBoardType(boardType);
        SetupCameraPosition();
    }

    private void SetupBoardType(BoardType boardType)
    {
        string sizePart = boardType.ToString().Replace("Size", "");
        string[] size = sizePart.Split('x');
        width = int.Parse(size[0]);
        height = int.Parse(size[1]);
        SetupTile(width, height);
    }

    private void SetupTile(int width, int height)
    {
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
                tileObject.name = $"Tile({x},{y})";
                Tile tile = tileObject.GetComponent<Tile>();
                tile.xIndex = x;
                tile.yIndex = y;
                tiles[x, y] = tile;
            }
        }
    }
    private void SetupCameraPosition()
    {
        Vector2 cameraPosition = new((float)(width - 1) / 2f, (float)(height - 1) / 2f);
        Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticleSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;
        Camera.main.orthographicSize = (verticleSize > horizontalSize) ? verticleSize : horizontalSize;
    }

}
