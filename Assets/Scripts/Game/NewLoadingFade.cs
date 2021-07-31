using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewLoadingFade : MonoBehaviour
{
    public void FadeIn()
    {
        this.gameObject.SetActive(true);

        this.GetComponent<Image>().DOFade(1, 0.5f);

        Invoke("FadeOut", 1);

    }

    public void FadeOut()
    {
        this.GetComponent<Image>().DOFade(0, 0.5f);

        Invoke("Deactivate", 0.5f);
    }

    void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
