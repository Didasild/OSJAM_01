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
    //Public Variables
    [Header("INFORMATIONS STATE")]
    [NaughtyAttributes.ReadOnly]
    public GameState currentGameState;

    [Header("FLOOR ELEMENTS")]
    public TMP_Text floorLevelText;
    public FloorSettings[] floorSettingsList;
    [NaughtyAttributes.ReadOnly]
    public int currentFloorLevel;
    [NaughtyAttributes.ReadOnly]
    public int floorLoop = 0;
    [NaughtyAttributes.ReadOnly]
    public FloorSettings currentFloorSettings;

    [Header("DIFFICULTY")]
    public int pourcentageOfMineIncrement = 3;

    [Header("REFERENCES")]
    public GameObject endScreenUI;
    public GridManager gridManager;
    public Player player;
    public GameObject mainCamera;

    //[Header("OBSERVATIONS")]
    //Private Variables
    private int pourcentageUpdate = 0;
    private int pourcentageOfMine = 0;

    //Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private CellVisualManager _CellVisualManager;
    public static CellVisualManager CellVisualManager => _instance._CellVisualManager;


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
        if (currentFloorSettings.proceduralGrid == true)
        {
            gridManager.GenerateGrid(currentFloorSettings.GetGridSize(), currentFloorSettings.floorPourcentageOfMine);
        }
        else
        {
            gridManager.LoadGridFromString(currentFloorSettings.savedGridString, currentFloorSettings.GetGridSize()) ;
        }
        player.ResetHealtPoint();
        player.ResetClickCounter();
    }

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
    #endregion

    #region FLOOR GENERATION
    public void ChangeFloorLevel()
    {
        //Update le numéro du floor
        currentFloorLevel += 1;
        floorLevelText.text = currentFloorLevel.ToString();

        // Calculer l'index du floor actuel dans la liste
        int floorIndex = currentFloorLevel % floorSettingsList.Length;

        //Récupère le floor suivant dans la liste
        currentFloorSettings = floorSettingsList[floorIndex];

        // Vérifier si on recommence une boucle
        if (floorIndex == 0 && currentFloorLevel > 0)
        {
            IncreaseLoopDiffficulty(pourcentageOfMineIncrement);
        }
        //Détermine le pourcentage de mine
        pourcentageOfMine = currentFloorSettings.floorPourcentageOfMine + pourcentageUpdate;

        //Check si c'est une grille procédurale ou généré
        if (currentFloorSettings.proceduralGrid == true)
        {
            //Génère une grille aléatoire avec les Settings récupérés
            gridManager.GenerateGrid(currentFloorSettings.GetGridSize(), pourcentageOfMine);
        }

    }

    public int IncreaseLoopDiffficulty(int pourcentageOfMineIncrement)
    {
        //Update le nombre de loop
        floorLoop += 1;
        pourcentageUpdate += pourcentageOfMineIncrement;
        return pourcentageUpdate;
    }
    #endregion

}
