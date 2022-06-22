using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForShieldScript : MonoBehaviour
{
    internal float health = 100f;
    internal float Damage;
    internal float dmgg;
    internal bool lowDamage = false;
    internal bool highDamage = false;
    internal Collider Other = null;
    internal bool Touch = false;

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
        health = 15;
        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;

        if (health == 0) { highDamage = true; } else { highDamage = false; }
        if (health == 15) { lowDamage = true; } else { lowDamage = false; }
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
