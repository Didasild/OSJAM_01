using UnityEngine;

public class Cell : MonoBehaviour
{
    public Cellstate currentState;
    public Celltype currentType;
    public GameObject cellCover;
    public GameObject cellEmpty;
    public GameObject cellMine;

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
        currentState = Cellstate.Cover;
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
