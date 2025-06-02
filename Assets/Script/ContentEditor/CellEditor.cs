using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class CellEditor : MonoBehaviour
{
    #region VARIABLES
    [Header("CELL PARAMETERS")]
    public CellState cellState;
    public CellType cellType;
    public ItemTypeEnum itemType;
    public bool proceduralCell;
    
    [Header("NPC PARAMETERS")]
    public NPCSettings npcSettings;
    [FormerlySerializedAs("dialogSequenceOverride")] public DialogPull dialogPullOverride;
    
    [Header("DEBUG / SETUP")]
    public bool showSetupElements = false;
    [ShowIf("showSetupElements")] public SpriteRenderer cellStateVisual;
    [ShowIf("showSetupElements")] public SpriteRenderer cellTypeVisual;
    [ShowIf("showSetupElements")] public SpriteRenderer debugVisual;
    [ShowIf("showSetupElements")] public int hintNumber = 0;
    [ShowIf("showSetupElements")] public TMP_Text debugHintText;
    [ShowIf("showSetupElements")] public Color debugBaseColor;
    [ShowIf("showSetupElements")] public Color debugHighlightColor;
    [ShowIf("showSetupElements")] public List<CellEditor> neighborsCellList = new List<CellEditor>(); 
    [ShowIf("showSetupElements")] public bool isSelected;
    
    private bool isFadingOut = false;
    private float fadeStartTime;
    private const float fadeDuration = 1f;
    private const float delayBeforeFade = 0.5f;
    
    public Vector2Int cellPosition; // La position dans la grille
    
    //PRIVATE
    private VisualEditorManager _visualManager;
    #endregion VARIABLES

    #region "ON" FUNCTIONS
    private void OnValidate()
    {
        gameObject.name = $"Cell ({cellState}, {cellType})";
        UpdateCellVisual();
    }
    private void OnEnable()
    {
        // S'abonne à l'événement de changement de sélection
        Selection.selectionChanged += UpdateSelectionState;
    }
    private void OnDisable()
    {
        // Se désabonne de l'événement de changement de sélection
        Selection.selectionChanged -= UpdateSelectionState;
    }
    #endregion "ON" FUNCTIONS

    #region INIT
    public void Initialize(VisualEditorManager visualManager)
    {
        gameObject.name = $"Cell ({cellState}, {cellType})";
        _visualManager = visualManager;
        UpdateCellVisual();
        HideAllDescendants(transform);
    }

    private void HideAllDescendants(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.hideFlags = HideFlags.HideInHierarchy; // Rend l'objet non sélectionnable
            HideAllDescendants(child); // Appel récursif pour les enfants de cet enfant
        }
    }
    #endregion
    
    #region LOGIC FUNCTIONS
    private void UpdateSelectionState()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        isSelected = false;
        foreach (GameObject selected in selectedObjects)
        {
            // Vérifie si l'objet n'est pas nul et correspond au GameObject actuel
            if (selected != null && selected == gameObject)
            {
                isSelected = true;
                break; // Sort de la boucle dès que l'objet est trouvé
            }
        }
    }
    #endregion

    #region VISUAL FUNCTIONS
    public void UpdateCellVisual()
    {
        if (cellStateVisual == null || _visualManager == null)
        {
            return;
        }
        if (proceduralCell)
        {
            cellState = CellState.Cover;
            cellType = CellType.Empty;
            itemType = ItemTypeEnum.None;
            UpdateRandomVisual();
        }
        switch (cellState)
        {
            case CellState.Cover:
                cellStateVisual.sprite = _visualManager.coverSprite;
                break;
            case CellState.Reveal:
                cellStateVisual.sprite = _visualManager.revealSprite;
                break;
            case CellState.Flag:
                cellStateVisual.sprite = _visualManager.flagSprite;
                break;
            case CellState.PlantedSword:
                cellStateVisual.sprite = _visualManager.plantedSwordSprite;
                break;
        }

        switch (cellType)
        {
            case CellType.None:
                cellStateVisual.sprite = null;
                cellTypeVisual.sprite = null;
                break;
            case CellType.Empty:
                cellTypeVisual.sprite = null;
                break;
            case CellType.Mine:
                cellTypeVisual.sprite = _visualManager.mineIconSprite;
                break;
            case CellType.Gate:
                cellTypeVisual.sprite = _visualManager.stairSprite;
                break;
            case CellType.Hint:

                break;
            case CellType.Item:
                UpdateItemVisual(itemType);
                break;
            case CellType.Npc:
                cellTypeVisual.sprite = _visualManager.npcSprite;
                break;
        }

        if (!proceduralCell)
        {
            UpdateHintText();
        }
        //Debug.Log($"Sprite mis à jour pour l'état : {cellState}");
    }

    private void UpdateItemVisual(ItemTypeEnum itemType)
    {
        if (cellTypeVisual == null || _visualManager == null)
        {
            return;
        }
        switch (itemType)
        {
            case ItemTypeEnum.None:
                cellTypeVisual.sprite = null;
                break;
            case ItemTypeEnum.Potion:
                cellTypeVisual.sprite = _visualManager.potionSprite;
                break;
            case ItemTypeEnum.Sword:
                cellTypeVisual.sprite = _visualManager.swordSprite;
                break;
            case ItemTypeEnum.Coin:
                cellTypeVisual.sprite = null;
                Debug.LogWarning("Pas de visuel pour le coin");
                break;
        }
    }

    private void UpdateRandomVisual()
    {
        debugHintText.text = "?";
    }

    private void UpdateHintText()
    {
        if (cellType != CellType.Hint)
        {
            debugHintText.text = "";
            return;
        }

        hintNumber = 0;
        foreach (CellEditor cellEditor in neighborsCellList)
        {
            if (cellEditor.cellType  == CellType.Mine)
            {
                hintNumber++;
            }
        }

        if (hintNumber == 0)
        {
            debugHintText.text = "";
            cellType = CellType.Empty;
        }
        else
        {
            debugHintText.text = hintNumber.ToString();
        }
    }

    public void HighlightCell()
    {
        // Change la couleur immédiatement
        debugVisual.color = debugHighlightColor;

        // Démarre le processus de fade-out après 1 seconde
        fadeStartTime = (float)EditorApplication.timeSinceStartup + delayBeforeFade;
        isFadingOut = true;

        // Déclenche l'update du fade
        EditorApplication.update += FadeOutUpdate;
    }

    private void FadeOutUpdate()
    {
        if (!isFadingOut || debugVisual == null) 
        {
            // Arrête l'animation si l'objet n'existe plus
            isFadingOut = false;
            EditorApplication.update -= FadeOutUpdate;
            return;
        }
        
        // Si 1 seconde s'est écoulée
        if ((float)EditorApplication.timeSinceStartup > fadeStartTime)
        {
            float elapsedTime = (float)EditorApplication.timeSinceStartup - fadeStartTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Lerp entre la couleur rouge et la couleur de base
            debugVisual.color = Color.Lerp(debugHighlightColor, debugBaseColor, t);

            // Quand l'animation est terminée, nettoie l'update et réinitialise la couleur
            if (t >= 1f)
            {
                isFadingOut = false;
                EditorApplication.update -= FadeOutUpdate;
                debugVisual.color = debugBaseColor;
            }
        }
    }
    #endregion VISUAL FUNCTIONS


}
