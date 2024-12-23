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
        debugVisual.color = debugBaseColor;
        debugVisual.color = Color.red;
        // Lance une coroutine pour effectuer le fade-out
        StartCoroutine(FadeOutCoroutine());
    }
    private IEnumerator FadeOutCoroutine()
    {
        // Attendre 1 seconde avant de commencer le fade-out
        yield return new WaitForSeconds(1f);

        float fadeDuration = 0.5f; // Durée du fade-out
        Color startColor = debugVisual.color;
        Color endColor = debugBaseColor;

        float elapsedTime = 0f;

        // Transition de la couleur actuelle vers la couleur de base
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            debugVisual.color = Color.Lerp(startColor, endColor, t);
            yield return null; // Attendre le prochain frame
        }

        // S'assurer que la couleur finale est bien définie
        debugVisual.color = debugBaseColor;
    }
    #endregion VISUAL FUNCTIONS


}
