using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShadowPlayerController : MonoBehaviour
{
    private List<Vector2> Path;
    private int currentIndex = 0;
    private Image imageReference;
    private float moveSpeed;
    private TrailRenderer[] trailRenderer;
    private int destroyIndex;
    private RectTransform rectReference;
    public void InitializeShadow(List<Vector2Int> P, float S, int L , Vector4 Color)
    {
        //Initialize Path
        Path = new List<Vector2>();
        foreach(Vector2Int pos in P)
        {
            Path.Add(Grid.Instance.GridArray[pos.x, pos.y].GetComponent<Cell>().Position);
        }

        //Position and Scale the rect
        rectReference = this.GetComponent<RectTransform>();
        rectReference.sizeDelta = new Vector2(S, S);
        rectReference.anchoredPosition = Path[0];
     
        //Get the trail and delete all old points
        trailRenderer = this.GetComponents<TrailRenderer>();
        trailRenderer[0].Clear();

        
        //Create a gradient with the same color as the current background and set it to the trail renderer
        //Color Keys
        GradientColorKey[] gradientColor = new GradientColorKey[1];
        gradientColor[0].color = Color;
        gradientColor[0].time = 0;
        
        //Alpha Keys
        GradientAlphaKey[] gradientAlpha = new GradientAlphaKey[1];
        gradientAlpha[0].alpha = 1;
        gradientAlpha[0].time = 0;
        
        //Set the new gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(gradientColor, gradientAlpha);
        trailRenderer[0].colorGradient = gradient;

        currentIndex = 1;
        moveSpeed = 0.25f;

        //Index at which the shadow destroys
        destroyIndex = (Path.Count - 1) - Mathf.FloorToInt(L / 3);

        if(destroyIndex < 4)
        {
            destroyIndex = 4;
        }

        DOTween.defaultEaseType = Ease.Linear;

        this.gameObject.SetActive(true);

        StartCoroutine("MoveShadow");
    }
    IEnumerator MoveShadow()
    {
        //Move the shadow on the path
        while(currentIndex <= destroyIndex)
        {
            rectReference.DOAnchorPos(Path[currentIndex], moveSpeed);
            currentIndex++;
            yield return new WaitForSeconds(moveSpeed);
        }

        Invoke("DestroyShadow", moveSpeed * 2);
        yield return null;


    }

    void DestroyShadow()
    {
        CancelInvoke();
        //Deactivate the object and enable player movement
        EventManager.enableControls.Invoke();
        this.gameObject.SetActive(false);
    }
}
