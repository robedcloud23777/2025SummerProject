using System;
using UnityEngine;

public class HitObject : MonoBehaviour
{
    IHitable ihitable;
    public float damage;
    private void OnCollisionEnter2D(Collision2D other)
    { 
        other.gameObject.GetComponent<IHitable>().Hit(damage);
    }
}

public interface IHitable
{
    public void Hit(float damage);
}
