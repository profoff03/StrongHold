using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForShieldScript : MonoBehaviour
{
    internal float maxHealth = 50f;
    internal float health;
    internal float Damage;
    internal float dmgg;
    internal bool lowDamage = false;
    internal bool highDamage = false;
    internal Collider Other = null;
    internal bool Touch = false;

    private void Start()
    {
        health = maxHealth;
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            Touch = true;
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
        }
    }

    private void TakeDamage(float? dmg)
    {
        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;
            
        if (health == 0) 
        { 
            highDamage = true;
            lowDamage = false;
        }
        else
        {
            highDamage = false;
            lowDamage = true;
        }
    }

    internal void healShield() => health = maxHealth;

}
