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
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [Header("INFORMATIONS STATE")]
    public GameState currentGameState;
    [Header("FLOOR ELEMENTS")]
    public int currentFloorLevel;
    public TMP_Text floorLevelText;
    public FloorSettings[] floorSettingsList;
    [NaughtyAttributes.ReadOnly]
    public FloorSettings currentFloorSettings;
    [Header("REFERENCES")]
    public GameObject endScreenUI;
    public Grid grid;
    public Player player;
    public GameObject mainCamera;



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
        endScreenUI.SetActive(true);
        currentFloorLevel = 0;
    }

    public void InGameState()
    {
        currentFloorSettings = floorSettingsList[currentFloorLevel % floorSettingsList.Length];
        grid.GenerateGrid(currentFloorSettings.GetGridSize(),currentFloorSettings.floorPourcentageOfMine);
        player.ResetHealtPoint();
        player.ResetClickCounter();
    }
    #endregion

    #region FLOOR GENERATION
    public void ChangeFloorLevel()
    {
        //Update le numéro du floor
        currentFloorLevel += 1;
        floorLevelText.text = currentFloorLevel.ToString();

        //Récupère le floor suivant dans la liste
        currentFloorSettings = floorSettingsList[currentFloorLevel % floorSettingsList.Length];

        //Check si c'est une grille procédurale ou généré
        if (currentFloorSettings.proceduralGrid == true)
        {
            //Génère une grille aléatoire avec les Settings récupérés
            grid.GenerateGrid(currentFloorSettings.GetGridSize(), currentFloorSettings.floorPourcentageOfMine);
        }
    }
    #endregion
    public void RestartGame()
    {
        StartCoroutine(CO_RestartGame());
        IEnumerator CO_RestartGame()
        {
            yield return new WaitForEndOfFrame();

            ChangeGameState(GameState.InGame);
            currentFloorLevel = 0;
            floorLevelText.text = currentFloorLevel.ToString();
            endScreenUI.SetActive(false);
        }
    }

}
