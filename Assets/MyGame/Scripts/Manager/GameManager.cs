public class GameManager : BaseManager<GameManager>
{
    private void Start()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowNotify<NotifyGameLoading>();

        }
    }

    public void StartGame(GameMode gameMode, BoardType boardType)
    {
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
