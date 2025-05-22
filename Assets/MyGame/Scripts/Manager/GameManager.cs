using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    private GameMode currentGameMode = GameMode.Unknown;

    public  GameMode CurrenGameMode => currentGameMode;
    private void Start()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowNotify<NotifyGameLoading>();

        }
    }

    public void StartGame(GameMode gameMode, BoardType boardType)
    {
        currentGameMode = gameMode;

        Debug.Log($"[GameManager] StartGame with mode: {currentGameMode}, board: {boardType}");

        if (BoardController.HasInstance)
        {
            BoardController.Instance.InitBoard(boardType);
        }
    }

    public void RestartGame()
    {

    }

    public void PauseGame()
    {

    }

    public void ResumeGame()
    {

    }

    public void QuitGame()
    {

    }

    private void OnApplicationQuit()
    {
        
    }

    private void OnApplicationPause(bool pause)
    {
        
    }
}
