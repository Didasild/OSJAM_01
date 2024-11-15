using UnityEngine;

public class Player : MonoBehaviour
{
    private Cell cellOver;
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
            Debug.Log("Cellule d�tect�e : " + hit.collider.gameObject.name);

            // Optionnel : Appelle une m�thode sur le script Cell attach�
            cellOver = hit.collider.GetComponent<Cell>();
            if (cellOver != null)
            {
                Debug.Log("�tat actuel de la cellule : " + cellOver.currentState);
            }
        }
        else
        {
            Debug.Log("Aucune cellule d�tect�e");
        }

        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            if (hit.collider !=null)
            {
                if (cellOver.currentState == Cell.Cellstate.Cover)
                {
                    ChangeCellState();
                }
            }
            else
            {
                Debug.Log("Aucune Cellule a cliquer");
            }
        }
    }

    private void ChangeCellState()
    {
        cellOver.ChangeState(Cell.Cellstate.Reveal);
    }

}
