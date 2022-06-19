using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strongholdShield : MonoBehaviour
{
    [SerializeField]
    GameObject firstEffect;
    ParticleSystem[] firstEffectParticle;
    [SerializeField]
    GameObject secondEffect;
    ParticleSystem[] secondEffectParticle;
    void Start()
    {
        firstEffectParticle = firstEffect.GetComponentsInChildren<ParticleSystem>();
        secondEffectParticle = secondEffect.GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(enableWall());
    }

    private IEnumerator enableWall()
    {
        yield return new WaitForSeconds(2.8f);
        foreach (ParticleSystem particle in firstEffectParticle)
        {
            particle.Pause();
        }
        foreach (ParticleSystem particle in secondEffectParticle)
        {
            particle.Pause();
        }
    }
    internal void disableFirstWall()
    {
        foreach (ParticleSystem particle in firstEffectParticle)
        {
            particle.Play();
        }
        
    }

    internal void disableSecondtWall()
    {
        foreach (ParticleSystem particle in firstEffectParticle)
        {
            particle.Play();
        }

    }

    internal void disable() => Destroy(gameObject);
}
