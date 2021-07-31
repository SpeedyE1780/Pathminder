using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    #region P U B L I C   V A R I A B L E S
    public List<Transform> Path;
    public Transform CurrentCell;
    public bool Moved; // Indicates the player did his first move
    #endregion

    #region P R I V A T E   V A R I A B L E S
    int currentIndex; //Used to go backwards on the player's path
    float respawnSpeed;
    bool canMove; // Indicates that the player is Respawning
    Image imageReference;
    int tapsCount;
    bool cellRevealed;
    Tween currentTween;
    #endregion

    #region A W A K E || O N  E N A B L E || O N  D I S A B L E

    private void Awake()
    {
        imageReference = this.GetComponent<Image>();
    }

    public void OnEnable()
    {
        EventManager.respawn += DelayRespawn;
        EventManager.enableControls += EnableMove;
    }

    public void OnDisable()
    {
        EventManager.respawn -= DelayRespawn;
        EventManager.enableControls -= EnableMove;
    }

    #endregion

    #region F U N C T I O N S || C O R O U T I N E S

    //I N I T I A L I Z E || M O V E  P L A Y E R
    //--------------------------------------------


    //Initialize the player variables and position
    public void InitializePlayer()
    {
        //Sets the current cell to the start cell and position the player there
        CurrentCell = Grid.Instance.StartCell;
        imageReference.rectTransform.anchoredPosition = CurrentCell.GetComponent<Image>().rectTransform.anchoredPosition;
        imageReference.DOFade(1, 0.5f);

        //Resets the player's path
        Path = new List<Transform>();
        currentIndex = -1;

        respawnSpeed = 0.1f;
        canMove = false;
        Moved = false;
        cellRevealed = false;

        tapsCount = 0;

    }

    void EnableMove()
    {
        canMove = true;
    }

    //Move the player
    public void MovePlayer(Transform TargetCell)
    {
        if(canMove && !DOTween.IsTweening(imageReference.rectTransform))
        {
            //If the TargetCell is a neighbor of the current cell we can move to it
            if (CurrentCell.GetComponent<Cell>().Neighbors.Contains(TargetCell.GetComponent<Cell>()))
            {
                if(cellRevealed)
                {
                    foreach (Cell cell in CurrentCell.GetComponent<Cell>().Neighbors)
                    {
                        cell.GetComponent<Image>().DOFade(1, 0.5f);
                        cellRevealed = true;
                    }

                }
                //Disable the current cell and add it to the player's path
                TargetCell.gameObject.SetActive(false);
                Path.Add(TargetCell);

                //Increment the index
                currentIndex++;

                //If there's no enemy behind the cell the player moves to it
                if (!TargetCell.GetComponent<Cell>().Enemy)
                {
                    //Move the player to the cell
                    CurrentCell = TargetCell;
                    currentTween = imageReference.rectTransform.DOAnchorPos(CurrentCell.GetComponent<Image>().rectTransform.anchoredPosition, 0.5f);

                    //Play Move Sound
                    SoundManager.Instance.MoveSound.time = 0.8f;
                    SoundManager.Instance.MoveSound.Play();

                    //Player made his first move
                    Moved = true;

                    //Increment the consecutive taps amount
                    tapsCount++;

                    if(tapsCount == 3)
                    {
                        tapsCount = 0;
                        foreach(Cell cell in CurrentCell.GetComponent<Cell>().Neighbors)
                        {
                            cell.GetComponent<Image>().DOFade(0.3f, 0.5f);
                            cellRevealed = true;
                        }
                    }

                    //Check if player have reached the end
                    if (CurrentCell.GetComponent<Cell>().Neighbors.Contains(Grid.Instance.EndCell.GetComponent<Cell>()))
                    {

                        //Add a tween to the end cell after going to the target cell
                        currentTween.OnComplete(() =>
                        {
                            imageReference.rectTransform.DOAnchorPos(Grid.Instance.EndCell.GetComponent<Image>().rectTransform.anchoredPosition, 0.5f);
                        });

                        canMove = false;

                        EventManager.nextLevel.Invoke();
                    }

                    //Makes sure the enemy didn't reach a dead end
                    else if (!CurrentCell.GetComponent<Cell>().Check())
                    {
                        canMove = false;
                        EventManager.respawn.Invoke();
                    }
                }

                //Player have collided with the enemy and respawns
                else
                {
                    EventManager.respawn.Invoke();
                }
            }
        }
    }

    //-------------------------------------------------------------------



    //R E S P A W N
    //-------------------------------
    void DelayRespawn()
    {
        canMove = false;
        Moved = false;
        Invoke("StartRespawn", 1);
    }

    //Start the respawn coroutine
    void StartRespawn()
    {
        StartCoroutine("Respawn");
        DOTween.defaultEaseType = Ease.Linear;
    }

    //Goes backward inside the player's path and moves him one step back and hides the cell until he reaches the start
    IEnumerator Respawn()
    { 
        //Move player back on his path
        while(currentIndex > 0)
        {
            if (cellRevealed)
            {
                foreach (Cell cell in CurrentCell.GetComponent<Cell>().Neighbors)
                {
                    cell.GetComponent<Image>().DOFade(1, 0.5f);
                    cellRevealed = true;
                }

            }

            //Tween the player to the previous cell
            imageReference.rectTransform.DOAnchorPos(Path[currentIndex - 1].GetComponent<Image>().rectTransform.anchoredPosition, respawnSpeed);
            yield return new WaitForSeconds(respawnSpeed);

            //Activate the cell again and move the player to it
            Path[currentIndex].gameObject.SetActive(true);
            currentIndex--;
        }

        //Places the player back to the starting cell 
        //Tween the player to the start cell
        imageReference.rectTransform.DOAnchorPos(Grid.Instance.StartCell.GetComponent<Image>().rectTransform.anchoredPosition, respawnSpeed);
        yield return new WaitForSeconds(respawnSpeed);

        //Set the cell active
        Path[currentIndex].gameObject.SetActive(true);
        //resets the path
        Path = new List<Transform>();
        //set back the current cell to the starting cell
        CurrentCell = Grid.Instance.StartCell;
        tapsCount = 0;
        cellRevealed = false;
        currentIndex = -1;
        DOTween.defaultEaseType = Ease.InOutQuad;
        EventManager.enableControls.Invoke();
        yield return null;

    }

    //----------------------------------------------------------------------

    #endregion
}
