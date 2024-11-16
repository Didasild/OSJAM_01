using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Cell cellOver;
    public int initialHealthPoints = 3;
    private int healthPoints;
    public TMP_Text healthPointText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Convertit la position de la souris en coordonn�es du monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Effectue un raycast � la position de la souris
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Affiche le nom de l'objet d�tect�
            //Debug.Log("Cellule d�tect�e : " + hit.collider.gameObject.name);

            // Optionnel : Appelle une m�thode sur le script Cell attach�
            cellOver = hit.collider.GetComponent<Cell>();
        }
        else
        {
            //Debug.Log("Aucune cellule d�tect�e");
        }

        if (cellOver == null || GameManager.Instance.currentGameState != GameState.InGame)
        {
            return;
        }
        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                if (cellOver.currentState == CellState.Cover)
                {
                    ChangeCellState(CellState.Reveal);
                }
                if (cellOver.currentType == CellType.Mine)
                {
                    DecreaseHealth(1);
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                ChangeCellState(CellState.Flag);
            }
            else if (cellOver.currentState == CellState.Flag)
            {
                ChangeCellState(CellState.Cover);
            }
        }
    }

    private void ChangeCellState(CellState newState)
    {
        if (newState == CellState.Reveal)
        {
            cellOver.ChangeState(CellState.Reveal);
        }
        if (newState == CellState.Flag)
        {
            cellOver.ChangeState(CellState.Flag);
        }
        if (newState == CellState.Cover)
        {
            cellOver.ChangeState(CellState.Cover);
        }
    }

    public void ResetHealtPoint()
    {
        healthPoints = initialHealthPoints;
        healthPointText.text = healthPoints.ToString();
    }

    public void DecreaseHealth(int damage)
    {
        healthPoints -= damage;
        healthPointText.text = healthPoints.ToString();
        if (healthPoints <= 0)
        {
            GameManager.Instance.ChangeGameState(GameState.Loose);
        }
    }

    public void IncreaseHealth(int heal)
    {
        healthPoints += heal;
    }

}
