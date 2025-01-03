using System.Collections;
using UnityEngine;
using TMPro;

#region ENUMS
public enum GameState
{
    None,
    InGame,
    Lose,
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
    public int floorLoop;
    [NaughtyAttributes.ReadOnly]
    public RoomSettings currentRoomSettings;

    [Header("MANAGER REFERENCES")]
    public DungeonManager dungeonManager;
    public GridManager gridManager;

    [Header("REFERENCES")]
    public GameObject endScreenUI;
    public Player player;

    //Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private CellVisualManager _CellVisualManager;
    public static CellVisualManager CellVisualManager => _instance._CellVisualManager;

    [SerializeField] private RoomVisualManager _RoomVisualManager;
    public static RoomVisualManager RoomVisualManager => _instance._RoomVisualManager;
    #endregion PARAMETERS

    #region INIT
    private void Awake()
    {
        _instance = this;
    }

    public void Start()
    {
        ChangeGameState(GameState.InGame);
    }
    #endregion INIT

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

            case GameState.Lose:
                LoseState();
                break;
        }
    }

    private void LoseState()
    {
        endScreenUI.SetActive(true);
        currentFloorLevel = 0;
    }

    private void InGameState()
    {
        if (dungeonManager.floorSettingsList.Length > 1)
        {
            //Génère un floor et une première room
            dungeonManager.currentFloorSetting = dungeonManager.floorSettingsList[0];
            dungeonManager.GenerateFloor(dungeonManager.currentFloorSetting.GetFloorSize());

            //Reset les data
            player.ResetHealthPoint();
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
    #endregion GAME STATE

    #region ROOM AND FLOOR MANAGEMENT

    public void ChangeRoom(RoomData roomData)
    {
        currentRoomSettings = roomData.roomSettings;
        if (roomData.currentRoomState != RoomState.FogOfWar)
        {
            gridManager.LoadRoomFromString(roomData.roomSavedString, currentRoomSettings.GetRoomSizeFromString(roomData.roomSavedString));
        }
        else if (currentRoomSettings.proceduralRoom)
        {
            gridManager.GenerateGrid(currentRoomSettings.GetRoomSize());
            roomData.ChangeRoomSate(RoomState.Started);
        }
        else
        {
            gridManager.LoadRoomFromString(currentRoomSettings.roomIDString, currentRoomSettings.GetRoomSizeFromString(currentRoomSettings.roomIDString));
            roomData.ChangeRoomSate(RoomState.Started);
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

        // A REDEFINIR AU BESOIN
        // Vérifier si on recommence une boucle
        //if (floorIndex == 0 && currentFloorLevel > 0)
        //{
        //    IncreaseLoopDiffficulty(pourcentageOfMineIncrement);
        //}
        ////Détermine le pourcentage de mine
        //pourcentageOfMine = currentRoomSettings.roomPourcentageOfMine + pourcentageUpdate;

    }
    #endregion ROOM AND FLOOR MANAGEMENT
}
