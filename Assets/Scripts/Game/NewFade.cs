using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class NewFade : MonoBehaviour
{
    public RectTransform Buttons;
    public Image Background;
    
    public void FadeIn()
    {
        this.gameObject.SetActive(true);
        DOTween.defaultTimeScaleIndependent = true;

        Background.DOFade(0.75f, 0.25f).OnComplete(() => {
            Buttons.DOScale(1, 0.25f);
            DOTween.defaultTimeScaleIndependent = false;
        });
    }

    public void FadeOut()
    {
        Buttons.DOScale(0, 0.25f).OnComplete(() =>
        {
            Background.DOFade(0, 0.75f);
        });

        Invoke("Deactivate", 0.5f);
    }

    void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
