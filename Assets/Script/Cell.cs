using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("CELL SETTINGS")]

    [Header("CELL INFORMATIONS")]
    public Cellstate currentState;
    public Celltype currentType;
    public List<Cell> neighborsCellList = new List<Cell>(); //Liste des voisins de la cellule
    [NaughtyAttributes.ReadOnly]
    public Vector2Int _cellPosition;

    [Header("CELL VISUALS")]
    public GameObject cellCover;
    public GameObject cellEmpty;
    public GameObject cellMine;
    public TextMeshProUGUI number;



    public enum Cellstate
    {
        Cover,
        Cliked,
        Reveal
    }

    public enum Celltype
    {
        ToDefine,
        Empty,
        Mine
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Initialize(Grid grid, Vector2Int cellPosition)
    {
        _cellPosition = cellPosition;
        ChangeState(currentState);
        neighborsCellList = grid.GetNeighbors(cellPosition);
    }

    public void ChangeState(Cellstate newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case Cellstate.Cover:
            CoverState(); 
            break;

            case Cellstate.Cliked:
            ClickedState(); 
            break;

            case Cellstate.Reveal:
            RevealState();
            break;
        }
    }

    public void ChangeType(Celltype newType)
    {

        currentType = newType;

        switch (currentType)
        {
            case Celltype.ToDefine:
            ToDefineType(); 
            break;

            case Celltype.Empty:
            EmptyType(); 
            break;

            case Celltype.Mine:
            MineType();
            break;
        }


    }

    private void CoverState()
    {
        Debug.Log("switch to Cover State");
    }

    private void RevealState()
    {
        Debug.Log("switch to Reveal State");
        cellCover.SetActive(false);
    }

    private void ClickedState()
    {
        Debug.Log("switch to Clicked State");
    }

    private void ToDefineType()
    {

    }

    private void EmptyType()
    {
        cellEmpty.SetActive(true);
        cellMine.SetActive(false);
    }

    private void MineType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
