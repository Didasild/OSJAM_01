using System.Collections;
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
    public bool isSelected;

    [Header("DEBUG")] 
    public SpriteRenderer cellStateVisual;
    public SpriteRenderer cellTypeVisual;
    public SpriteRenderer debugVisual;
    public Color debugBaseColor;
    public Color debugHighlightColor;
    
    private bool isFadingOut = false;
    private float fadeStartTime;
    private const float fadeDuration = 1f;
    private const float delayBeforeFade = 0.5f;
    
    public Vector2Int _cellPosition; // La position dans la grille
    
    //PRIVATE
    private CellVisualManager _cellVisualManager;
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
    public void Initialize(CellVisualManager cellVisualManager)
    {
        gameObject.name = $"Cell ({cellState}, {cellType})";
        _cellVisualManager = cellVisualManager;
        UpdateCellVisual();
        foreach (Transform child in transform)
        {
            child.gameObject.hideFlags = HideFlags.HideInHierarchy; // Rend l'objet non sélectionnable
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
        Debug.Log($"Selected: {isSelected} :" + gameObject.name);
    }
    #endregion

    #region VISUAL FUNCTIONS
    public void UpdateCellVisual()
    {
        switch (cellState)
        {
            case CellState.Cover:
                cellStateVisual.sprite = _cellVisualManager.coverSprite;
                break;
            case CellState.Reveal:
                cellStateVisual.sprite = _cellVisualManager.revealSprite;
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
                cellTypeVisual.sprite = _cellVisualManager.mineIconSprite;
                break;
            case CellType.Gate:
                cellTypeVisual.sprite = _cellVisualManager.stairType;
                break;
            case CellType.Item:
                UpdateItemVisual(itemType);
                break;
        }
        //Debug.Log($"Sprite mis à jour pour l'état : {cellState}");
    }

    public void UpdateItemVisual(ItemTypeEnum itemType)
    {
        switch (itemType)
        {
            case ItemTypeEnum.None:
                cellTypeVisual.sprite = null;
                break;
            case ItemTypeEnum.Potion:
                cellTypeVisual.sprite = _cellVisualManager.potionSprite;
                break;
            case ItemTypeEnum.Sword:
                cellTypeVisual.sprite = _cellVisualManager.swordSprite;
                break;
            case ItemTypeEnum.Coin:
                cellTypeVisual.sprite = null;
                Debug.LogWarning("Pas de visuel pour le coin");
                break;
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
        if (!isFadingOut) return;

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
