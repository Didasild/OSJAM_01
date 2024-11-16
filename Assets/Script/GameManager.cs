using UnityEngine;

public enum GameState
{
    InGame,
    Loose,
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public GameState currentGameState;
    public GameObject endScreen;
    public Grid grid;
    public Player player;
    

    private void Awake()
    {
        _instance = this;
    }

    public void Start()
    {
        ChangeGameState(GameState.InGame);
    }

    public void ChangeGameState (GameState gameState)
    {
        currentGameState = gameState;

        switch (gameState)
        {
            //case GameState.Start:
            //    StartState();
            //    break;

            case GameState.InGame:
                InGameState();
                break;

            case GameState.Loose:
                LooseState();
                break;
        }
    }

    public void StartState()
    {

    }

    public void LooseState() 
    {
        endScreen.SetActive (true);
    
    }

    public void InGameState()
    {
        grid.GenerateGrid();
        player.ResetHealtPoint();
    }

    public void RestartGame()
    {
        ChangeGameState (GameState.InGame);
        endScreen.SetActive (false);
    }

}
