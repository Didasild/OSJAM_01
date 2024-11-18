using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Cell cellOver;
    [Header("HEALTH")]
    public int initialHealthPoints = 3;
    public TMP_Text healthPointText;
    private int healthPoints;
    [Header("SWORD")]
    public int initialSwordCounter = 0;
    public TMP_Text swordCounterText;
    private int swordCounter;

    [NaughtyAttributes.ReadOnly]
    public int clicCounter;

    // Update is called once per frame
    void Update()
    {
        // Convertit la position de la souris en coordonnées du monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Effectue un raycast à la position de la souris
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Optionnel : Appelle une méthode sur le script Cell attaché
            cellOver = hit.collider.GetComponent<Cell>();
        }
        else
        {
            //Debug.Log("Aucune cellule détectée");
        }

        if (cellOver == null || GameManager.Instance.currentGameState != GameState.InGame)
        {
            return;
        }
        #region CLIC GAUCHE
        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                if (clicCounter == 0)
                {
                    cellOver.ChangeType(CellType.Empty);
                    cellOver.RemoveNeighborsMine();
                    GameManager.Instance.grid.SetItemsType(CellType.Stair, 1);
                    GameManager.Instance.grid.SetItemsType(CellType.Potion, GameManager.Instance.currentFloorSettings.GetNumberOfPotion());
                    GameManager.Instance.grid.SetItemsType(CellType.Sword, GameManager.Instance.currentFloorSettings.GetNumberOfSword());
                    IncreaseClickCount();
                }
                else
                {
                    cellOver.ChangeState(CellState.Reveal);
                    IncreaseClickCount();
                }
            }
            else if (cellOver.currentState == CellState.Sword)
            {
                if (cellOver.currentType == CellType.Mine)
                {
                    cellOver.DestroyCellType();
                }
                else
                {
                    cellOver.ChangeState(CellState.Reveal);
                }
            }
            else if (cellOver.currentType == CellType.Stair)
            {
                GameManager.Instance.ChangeFloorLevel();
                ResetClickCounter();
            }
            else if (cellOver.currentType == CellType.Potion)
            {
                IncreaseHealth(1);
                cellOver.ChangeType(CellType.Empty);
                cellOver.UpdateRegardingNeighbors();
            }
            else if (cellOver.currentType == CellType.Sword)
            {
                IncreaseSwordCounter();
                cellOver.ChangeType(CellType.Empty);
                cellOver.UpdateRegardingNeighbors();
            }
        }
        #endregion

        #region CLIC DROIT
        // Clique sur le bouton droit
        if (Input.GetMouseButtonDown(1))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                cellOver.ChangeState(CellState.Flag);
            }
            else if (cellOver.currentState == CellState.Flag && swordCounter >= 1)
            {
                cellOver.ChangeState(CellState.Sword);
                DecreaseSwordCounter();
            }
            else if (cellOver.currentState == CellState.Flag && swordCounter == 0)
            {
                cellOver.ChangeState(CellState.Cover);
            }
            else if (cellOver.currentState == CellState.Sword)
            {
                cellOver.ChangeState(CellState.Cover);
                IncreaseSwordCounter();
            }
        }

        #endregion

        #region CLIC MILIEU
        // Clique du milieu
        if (Input.GetMouseButtonDown(2))
        {
            if (cellOver.currentType == CellType.Hint && cellOver.currentState == CellState.Reveal)
            {
                int neighborsFlagged = 0;
                int neighborsMine = 0;
                int mineExploded = 0;

                //Récupère le nombre de drapeaux et de mines autour
                neighborsFlagged = cellOver.GetNeighborsState(CellState.Flag);
                neighborsMine = cellOver.GetNeighborsType(CellType.Mine);

                //Reveal les case couverte autour
                if (cellOver.currentType == CellType.Hint && neighborsFlagged == neighborsMine)
                {
                    foreach (Cell neighborsCell in cellOver.neighborsCellList)
                    {
                        if (neighborsCell.currentState == CellState.Cover && neighborsCell.currentType == CellType.Mine)
                        {
                            mineExploded += 1;
                        }
                        if (neighborsCell.currentState == CellState.Cover)
                        {
                            neighborsCell.ChangeState(CellState.Reveal);
                        }
                    }
                    if (mineExploded >= 1)
                    {
                        foreach (Cell neighborsCell in cellOver.neighborsCellList)
                        {
                            if (neighborsCell.currentState == CellState.Flag && neighborsCell.currentType != CellType.Mine)
                            {
                                neighborsCell.ChangeState(CellState.Reveal);
                            }
                        }
                    }
                }
            }

        }
        #endregion
    }

    #region HEALTH
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
        healthPointText.text = healthPoints.ToString();
    }
    #endregion

    #region SWORD
    public void ResetSwordCounter()
    {
        swordCounter = initialSwordCounter;
        swordCounterText.text = swordCounter.ToString();
    }

    public void DecreaseSwordCounter(int swordDecrease = 1)
    {
        if (swordCounter == 0)
        {
            return;
        }
        swordCounter -= swordDecrease;
        swordCounterText.text = swordCounter.ToString();
    }

    public void IncreaseSwordCounter(int swordIncrease = 1)
    {
        swordCounter += swordIncrease;
        swordCounterText.text = swordCounter.ToString();
    }

    #endregion

    public void ResetClickCounter()
    {
        clicCounter = 0;
    }

    public void IncreaseClickCount()
    {
        clicCounter += 1;
    }

}
