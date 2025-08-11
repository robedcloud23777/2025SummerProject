using System;
using UnityEngine;

public class HitObject : MonoBehaviour
{
    IHitable ihitable;
    public float damage;
    private void OnCollisionEnter2D(Collision2D other)
    {
        ihitable  = other.gameObject.GetComponent<IHitable>();
        Debug.Log(other.gameObject.name);
        ihitable.Hit(damage);
    }
}

public interface IHitable
{
    public void Hit(float damage);
}
