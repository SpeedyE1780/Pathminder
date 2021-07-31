using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PursuerController : MonoBehaviour
{
    public PlayerController PlayerReference;
    int currentIndex;
    float moveSpeed;
    Image imageReference;

    private void Awake()
    {
        imageReference = this.GetComponent<Image>();
    }


    //Spawn the pursuer enemy
    public void Spawn(PlayerController PC)
    {

        this.gameObject.SetActive(true);

        currentIndex = 0;

        //Position the pursuer on the start cell
        imageReference.rectTransform.anchoredPosition = Grid.Instance.StartCell.GetComponent<Image>().rectTransform.anchoredPosition;

        imageReference.DOFade(1.0f, 0.5f);

        PlayerReference = PC;

        moveSpeed = 0.75f;
        //Calls function to start moving the enemy
        StartCoroutine("MoveCoroutine");
    }


    //Move the enemy
    IEnumerator MoveCoroutine()
    {
        //Checks if the player and pursuer are colliding else move the enemy
        while (PlayerReference.Path[currentIndex] != PlayerReference.CurrentCell)
        {
            yield return new WaitForSeconds(moveSpeed);

            //Increment the index and move the pursuer to the next cell
            DOTween.defaultEaseType = Ease.Linear;
            imageReference.rectTransform.DOAnchorPos(PlayerReference.Path[currentIndex].GetComponent<Image>().rectTransform.anchoredPosition, moveSpeed);
            currentIndex++;
        }

        yield return new WaitForSeconds(moveSpeed);
        //Respawn
        EventManager.respawn.Invoke();
    }


    //Stop the coroutine and deactivate the enemy
    public void Destroy()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }
}
