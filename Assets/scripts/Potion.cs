using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public ParticleSystem[] explosionEffects; // Particle effect array to store multiple effects
    public PotionType potionType;
    private bool isTouchable = true;
    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;
    public Potion(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
    public bool IsTouchable
    {
        get { return isTouchable; }
        set { isTouchable = value; }
    }
    void Awake()
    {
        explosionEffects = GetComponentsInChildren<ParticleSystem>();
      
    }
    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;

    }
    public void Explode()
    {
        // Objeyi görünmez yap
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Patlama efektlerini tetikle
        foreach (var effect in explosionEffects)
        {
            effect.Play();
        }
    }

    // MoveToTarget
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }

    // MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.2f;
        Vector2 startPosition = transform.position;
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;
            transform.position = Vector2.Lerp(startPosition, _targetPos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = _targetPos;
        isMoving = false;
    }

}

public enum PotionType
{
    
    Red,
    Blue,
    Purple,
    Green,
    White,
    TNT,
    ColorBomb
}