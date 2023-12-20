using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private int currentHealth; //Serialized for debuging
    public int CurrentHealth { get => currentHealth; private set => currentHealth = value; }

    public int MaxHealth { get => maxHealth; private set => maxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int Damage)
    {
        int damageTaken = Mathf.Clamp(Damage, 0, currentHealth);

        currentHealth -= damageTaken;

        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if (currentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(transform.position, gameObject);
        }
    }
}
