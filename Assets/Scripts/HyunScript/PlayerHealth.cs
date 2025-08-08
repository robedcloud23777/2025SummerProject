using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float hp = 100f;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log($"[TakeDamage] {gameObject.name} took {damage} damage, remaining HP: {hp}");
        if (hp <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"[TakeDamage] {gameObject.name} died.");
        
    }
}

