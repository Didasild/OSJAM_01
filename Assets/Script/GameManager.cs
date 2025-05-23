using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

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
    public Chapters currentChapter;
    public List<ChapterSettings> chaptersList = new List<ChapterSettings>();
    [NaughtyAttributes.ReadOnly]
    public ChapterSettings currentChapterSettings;

    [Header("FLOOR ELEMENTS")]
    public TMP_Text floorLevelText;
    [NaughtyAttributes.ReadOnly]
    public int currentFloorLevel;
    [NaughtyAttributes.ReadOnly]
    public RoomSettings currentRoomSettings;

    [Header("MANAGER REFERENCES")]
    public FloorManager floorManager;
    public GridManager gridManager;

    [Header("REFERENCES")]
    public GameObject endScreenUI;
    public Player player;

    //Singleton
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [FormerlySerializedAs("_CellVisualManager")] [SerializeField] private VisualManager _visualManager;
    public static VisualManager visualManager => _instance._visualManager;
    
    
    #endregion PARAMETERS

    #region INIT
    private void Awake()
    {
        _instance = this;
        
        //roomVisualManager.Init();
        visualManager.Init();
        
        player.Init();
        floorManager.Init();
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
        //Assigne le current chapter settings en fonction du chapter selectionné
        currentChapterSettings = chaptersList.Find(setting => setting.chapter == currentChapter);
        
        //Donne au dungeon les floor correspondant au chapter
        floorManager.floorSettingsList = new FloorSettings[] { };
        floorManager.floorSettingsList = currentChapterSettings.floorSettings;
        if (floorManager.floorSettingsList.Length >= 1)
        {
            //Génère un floor et une première room
            floorManager.currentFloorSetting = floorManager.floorSettingsList[0];
            if (floorManager.currentFloorSetting.proceduralFloor)
            {
                floorManager.GenerateProceduralFloor(floorManager.currentFloorSetting);
            }
            else
            {
                floorManager.LoadFloor(floorManager.currentFloorSetting);
            }
            
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
        currentRoomSettings = roomData.initRoomSettings;
        if (roomData.currentRoomState != RoomState.FogOfWar)
        {
            gridManager.LoadRoomFromString(roomData.roomSavedString, currentRoomSettings.GetRoomSizeFromString(roomData.roomSavedString), currentRoomSettings.haveProceduralCells);
        }
        else if (currentRoomSettings.proceduralRoom)
        {
            gridManager.GenerateGrid(currentRoomSettings.GetRoomSize());
            roomData.ChangeRoomSate(RoomState.Started);
        }
        else
        {
            gridManager.LoadRoomFromString(currentRoomSettings.roomIDString, currentRoomSettings.GetRoomSizeFromString(currentRoomSettings.roomIDString), currentRoomSettings.haveProceduralCells);
            roomData.ChangeRoomSate(RoomState.Started);
        }
    }

    public void ChangeFloorLevel()
    {
        //Update le numéro du floor
        currentFloorLevel += 1;
        floorLevelText.text = currentFloorLevel.ToString();

        // Calculer l'index du floor actuel dans la liste
        int floorIndex = currentFloorLevel % floorManager.floorSettingsList.Length;

        //Récupère le floor suivant dans la liste
        floorManager.currentFloorSetting = floorManager.floorSettingsList[floorIndex];

        //Génère un floor et la room de départ
        if (floorManager.currentFloorSetting.proceduralFloor)
        {
            floorManager.GenerateProceduralFloor(floorManager.currentFloorSetting);
        }
        else
        {
            floorManager.LoadFloor(floorManager.currentFloorSetting);
        }

    }
    #endregion ROOM AND FLOOR MANAGEMENT
}
