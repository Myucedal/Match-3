using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInImage : MonoBehaviour
{
    public Image image;
    public float duration = 4f;

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = image.color;
        color.a = 1f; // Baþlangýç alfa deðeri (tamamen opak)
        image.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / duration)); // Alfa deðerini yavaþça azalt
            image.color = color;
            yield return null;
        }

        // Son olarak alfa deðerini tamamen transparan yap
        color.a = 0f;
        image.color = color;
    }
}
