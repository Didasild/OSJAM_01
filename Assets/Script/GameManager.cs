using System.Collections;
using UnityEngine;
using TMPro;

#region ENUMS
public enum GameState
{
    None,
    InGame,
    Loose,
}
#endregion

public class GameManager : MonoBehaviour
{
    #region PARAMETERS
    [Header("INFORMATIONS STATE")]
    [NaughtyAttributes.ReadOnly]
    public GameState currentGameState;

    [Header("FLOOR ELEMENTS")]
    public TMP_Text floorLevelText;
    [NaughtyAttributes.ReadOnly]
    public int currentFloorLevel;
    [NaughtyAttributes.ReadOnly]
    public int floorLoop = 0;
    [NaughtyAttributes.ReadOnly]
    public RoomSettings currentRoomSettings;

    [Header("MANAGER REFERENCES")]
    public DungeonManager dungeonManager;
    public GridManager gridManager;

    [Header("REFERENCES")]
    public GameObject endScreenUI;
    public Player player;

    //[Header("OBSERVATIONS")]
    //Private Variables
    //private int pourcentageUpdate = 0;
    //private int pourcentageOfMine = 0;

    //[Header("DIFFICULTY")]
    //public int pourcentageOfMineIncrement = 5;

    //Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private CellVisualManager _CellVisualManager;
    public static CellVisualManager CellVisualManager => _instance._CellVisualManager;
    #endregion


    #region INIT
    private void Awake()
    {
        _instance = this;
    }

    public void Start()
    {
        ChangeGameState(GameState.InGame);
    }
    #endregion

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
        if (dungeonManager.floorSettingsList.Length > 1)
        {
            //Génère un floor et une première room
            dungeonManager.currentFloorSetting = dungeonManager.floorSettingsList[0];
            dungeonManager.GenerateFloor(dungeonManager.currentFloorSetting.GetFloorSize());

            //Reset les data
            player.ResetHealtPoint();
            player.ResetClickCounter();
        }
        else
        {
            Debug.LogError("Pas de floor dans la floor setting list du Dungeon Manager");
        }
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

    #region ROOM AND FLOOR MANAGEMENT

    public void ChangeRoom(RoomData roomData)
    {
        currentRoomSettings = roomData.roomSettings;
        if (roomData.currentRoomState != roomState.Undiscover)
        {
            gridManager.LoadGridFromString(roomData.roomSavedString, currentRoomSettings.GetRoomSizeFromString(roomData.roomSavedString));
        }
        else if (currentRoomSettings.proceduralRoom == true)
        {
            gridManager.GenerateGrid(currentRoomSettings.GetRoomSize(), currentRoomSettings.roomPourcentageOfMine);
        }
        else
        {
            gridManager.LoadGridFromString(currentRoomSettings.roomLoadString, currentRoomSettings.GetRoomSizeFromString(currentRoomSettings.roomLoadString));
            roomData.ChangeRoomSate(roomState.Started);
        }
    }

    public void ChangeFloorLevel()
    {
        //Update le numéro du floor
        currentFloorLevel += 1;
        floorLevelText.text = currentFloorLevel.ToString();

        // Calculer l'index du floor actuel dans la liste
        int floorIndex = currentFloorLevel % dungeonManager.floorSettingsList.Length;

        //Récupère le floor suivant dans la liste
        dungeonManager.currentFloorSetting = dungeonManager.floorSettingsList[floorIndex];

        //Génère un floor et la room de départ
        dungeonManager.GenerateFloor(dungeonManager.currentFloorSetting.GetFloorSize());

        /// A REDEFINIR AU BESOIN
        // Vérifier si on recommence une boucle
        //if (floorIndex == 0 && currentFloorLevel > 0)
        //{
        //    IncreaseLoopDiffficulty(pourcentageOfMineIncrement);
        //}
        ////Détermine le pourcentage de mine
        //pourcentageOfMine = currentRoomSettings.roomPourcentageOfMine + pourcentageUpdate;

    }
    /// DIFFICULTY 
    //public int IncreaseLoopDiffficulty(int pourcentageOfMineIncrement)
    //{
    //    //Update le nombre de loop
    //    floorLoop += 1;
    //    pourcentageUpdate += pourcentageOfMineIncrement;
    //    return pourcentageUpdate;
    //}
    #endregion
}
