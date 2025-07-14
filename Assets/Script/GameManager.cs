using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

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
    #region FIELDS
    [Header("INFORMATIONS STATE")]
    [ReadOnly] public GameState currentGameState;
    public Chapters currentChapter;
    public List<ChapterSettings> chaptersList = new List<ChapterSettings>();
    [ReadOnly] public ChapterSettings currentChapterSettings;

    [Header("FLOOR ELEMENTS")]
    public TMP_Text floorLevelText;
    [ReadOnly] public int currentFloorLevel;
    [ReadOnly] public RoomSettings currentRoomSettings;

    [Header("MANAGER REFERENCES")]
    [SerializeField] private FloorManager _floorManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private VisualManager _visualManager;
    [SerializeField] private Player _player;
    [SerializeField] private Dialog _dialog;

    [Header("UI REFERENCES")]
    public GameObject endScreenUI;

    //GETTER
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public static VisualManager visualManager => _instance._visualManager;
    public Player Player => _player;
    public Dialog Dialog => _dialog;
    public FloorManager FloorManager => _floorManager;
    public GridManager GridManager => _gridManager;
    #endregion FIELDS

    #region INIT
    private void Awake()
    {
        _instance = this;
        
        visualManager.Init(this);
        _player.Init(this);
        _dialog.Init(this);
        _floorManager.Init();
        _gridManager.Init();
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
        FloorManager.floorSettingsList = new FloorSettings[] { };
        FloorManager.floorSettingsList = currentChapterSettings.floorSettings;
        if (FloorManager.floorSettingsList.Length >= 1)
        {
            //Génère un floor et une première room
            FloorManager.currentFloorSetting = FloorManager.floorSettingsList[0];
            if (FloorManager.currentFloorSetting.proceduralFloor)
            {
                FloorManager.GenerateProceduralFloor(FloorManager.currentFloorSetting);
            }
            else
            {
                FloorManager.LoadFloor(FloorManager.currentFloorSetting);
            }
            
            //Reset les data
            _player.Health.ResetHealthPoint();
            _player.ResetClickCounter();
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
    //A DÉPLACER DANS FLOOR MANAGER
    public void ChangeRoom(RoomData roomData)
    {
        currentRoomSettings = roomData.initRoomSettings;
        if (roomData.currentRoomState != RoomState.FogOfWar)
        {
            GridManager.LoadRoomFromString(roomData.roomSavedString, currentRoomSettings.GetRoomSizeFromString(roomData.roomSavedString), currentRoomSettings.haveProceduralCells);
        }
        else if (currentRoomSettings.proceduralRoom)
        {
            GridManager.GenerateGrid(currentRoomSettings.GetRoomSize());
            roomData.ChangeRoomSate(RoomState.StartedLock);
        }
        else
        {
            GridManager.LoadRoomFromString(currentRoomSettings.roomIDString, currentRoomSettings.GetRoomSizeFromString(currentRoomSettings.roomIDString), currentRoomSettings.haveProceduralCells);
            roomData.ChangeRoomSate(RoomState.StartedLock);
        }
    }

    public void ChangeFloorLevel()
    {
        //Update le numéro du floor
        currentFloorLevel += 1;
        floorLevelText.text = currentFloorLevel.ToString();

        // Calculer l'index du floor actuel dans la liste
        int floorIndex = currentFloorLevel % FloorManager.floorSettingsList.Length;

        //Récupère le floor suivant dans la liste
        FloorManager.currentFloorSetting = FloorManager.floorSettingsList[floorIndex];

        //Génère un floor et la room de départ
        if (FloorManager.currentFloorSetting.proceduralFloor)
        {
            FloorManager.GenerateProceduralFloor(FloorManager.currentFloorSetting);
        }
        else
        {
            FloorManager.LoadFloor(FloorManager.currentFloorSetting);
        }
    }
    #endregion ROOM AND FLOOR MANAGEMENT
}
