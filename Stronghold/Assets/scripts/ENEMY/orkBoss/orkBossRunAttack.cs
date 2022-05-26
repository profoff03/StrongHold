using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orkBossRunAttack : MonoBehaviour
{
    [SerializeField]
    private BigOrkBoss _boss;

    [SerializeField]
    private float _damage;
    private void Start()
    {
        gameObject.GetComponent<DamageProperty>().Damage = _damage;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Untagged"))
        {
            _boss.isRush = false;
            _boss.canRush = false;
            _boss.dashParticle.SetActive(false);
            _boss._animator.SetBool("isRush", false);
        }
    }
}
