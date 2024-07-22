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
        color.a = 1f; // Ba�lang�� alfa de�eri (tamamen opak)
        image.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / duration)); // Alfa de�erini yava��a azalt
            image.color = color;
            yield return null;
        }

        // Son olarak alfa de�erini tamamen transparan yap
        color.a = 0f;
        image.color = color;
    }
}
