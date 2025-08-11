using System;
using UnityEngine;

public class TestHitObject : MonoBehaviour, IHitable
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Hit(float damage)
    {
        spriteRenderer.color = Color.yellow;
    }
}
