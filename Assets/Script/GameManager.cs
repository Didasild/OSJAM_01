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
    public FloorSettings[] floorSettingsList;
    [NaughtyAttributes.ReadOnly]
    public FloorSettings currentFloorSettings;

    private void Awake()
    {
        _instance = this;
    }

    public void Start()
    {
        ChangeGameState(GameState.InGame);
    }

    #region GAME STATE
    public void ChangeGameState(GameState gameState)
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
        endScreen.SetActive(true);
    }

    public void InGameState()
    {
        currentFloorSettings = floorSettingsList[floorLevel % floorSettingsList.Length];
        grid.GenerateGrid(currentFloorSettings.GetGridSize(),currentFloorSettings.floorPourcentageOfMine);
        player.ResetHealtPoint();
        player.ResetClickCounter();
    }
    #endregion

    #region FLOOR GENERATION
    public void ChangeFloorLevel()
    {
        floorLevel += 1;
                currentFloorSettings = floorSettingsList[floorLevel % floorSettingsList.Length];
        grid.GenerateGrid(currentFloorSettings.GetGridSize(),currentFloorSettings.floorPourcentageOfMine);
        floorLevelText.text = floorLevel.ToString();
    }
    #endregion
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
