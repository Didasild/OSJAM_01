using System.Collections;
using UnityEngine;
using TMPro;

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
    public int floorLevel;
    public TMP_Text floorLevelText;
    

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

    public void LooseState() 
    {
        endScreen.SetActive (true);    
    }

    public void InGameState()
    {
        grid.GenerateGrid();
        player.ResetHealtPoint();
        player.ResetClickCounter();
    }

    public void IncrementFloorLevel()
    {
        floorLevel += 1;
        floorLevelText.text = floorLevel.ToString();
    }

    public void RestartGame()
    {
        StartCoroutine(CO_RestartGame());
        IEnumerator CO_RestartGame()
        {
            yield return new WaitForEndOfFrame();

            ChangeGameState(GameState.InGame);
            floorLevel = 0;
            floorLevelText.text = floorLevel.ToString();
            endScreen.SetActive(false);
        }
    }

}
