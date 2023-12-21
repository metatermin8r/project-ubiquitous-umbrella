using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyHealth health;
    //public EnemyMovement movement; For use with AI later
    //public EnemyPainResponse painResponse; Example of how to integrate animations into damage

    private void Start()
    {
        //health.OnTakeDamage += painResponse.HandlePain;
        health.OnDeath += Die;
    }

    private void Die(Vector3 position, GameObject gameObject)
    {
        //movement.StopMoving(); With example above, would be used to halt Nav Mesh agent
        //painResponse.HandleDeath(); Would handle any death animations or the like
        Destroy(gameObject);
    }
}
