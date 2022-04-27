using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playParticleSound : MonoBehaviour
{
    ParticleSystem _particleSistem;
    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _particleSistem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
      
        
    }
}
