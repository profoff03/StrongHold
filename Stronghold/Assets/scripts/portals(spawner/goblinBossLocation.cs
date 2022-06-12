using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goblinBossLocation : MonoBehaviour
{
    AudioSource[] mainAudioSourse;
    AudioSource playerAudioSource;
    [SerializeField]
    AudioClip explosionSound;
    [SerializeField]
    AudioClip rockStartSound;
    [SerializeField]
    AudioClip rockEndSound;
    
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    GameObject spawnWall;
    ParticleSystem[] spawnWallParticles;
    [SerializeField]
    GameObject villageWall;
    ParticleSystem[] villageWallParticles;
    [SerializeField]
    GameObject castleWall;
    ParticleSystem[] castleWallParticles;
    [SerializeField]
    GameObject brigeWall;
    ParticleSystem[] brigeWallParticles;

    bool playerEnter = false;
    bool enemyEnter = false;
    bool wallEnable;
    
    void Start()
    {
        playerAudioSource = playerTransform.GetComponent<AudioSource>();
        mainAudioSourse = Camera.main.GetComponents<AudioSource>();

        spawnWallParticles = spawnWall.GetComponentsInChildren<ParticleSystem>();
        villageWallParticles = villageWall.GetComponentsInChildren<ParticleSystem>();
        castleWallParticles = castleWall.GetComponentsInChildren<ParticleSystem>();
        brigeWallParticles = brigeWall.GetComponentsInChildren<ParticleSystem>();
    }

    
    void Update()
    {
        if (playerEnter && enemyEnter && !wallEnable) 
        {
            StartCoroutine(enableWall());
            StartCoroutine(changeMusicToBattle());
        } 
    }

    internal void disableBrigeWall() 
    {
        playerAudioSource.PlayOneShot(rockEndSound);
        foreach (ParticleSystem particle in brigeWallParticles)
        {
            particle.Play();
        }
        Destroy(brigeWall, 1.5f);
        StartCoroutine(changeMusicToMain());
    }
    internal void disableSpawnWall()
    {
        playerAudioSource.PlayOneShot(rockEndSound);
        foreach (ParticleSystem particle in spawnWallParticles)
        {
            particle.Play();
        }
        Destroy(spawnWall, 1.5f);
    }
    internal void disablecastleWall()
    {
        foreach (ParticleSystem particle in castleWallParticles)
        {
            particle.Play();
        }
        Destroy(castleWall, 1.5f);
    }
    internal void disablevillageWall()
    {
        foreach (ParticleSystem particle in brigeWallParticles)
        {
            particle.Play();
        }
        Destroy(villageWall, 1.5f);
    }

    private IEnumerator enableWall()
    {
        playerAudioSource.PlayOneShot(rockStartSound);
        wallEnable = true;
        spawnWall.SetActive(true);
        villageWall.SetActive(true);
        castleWall.SetActive(true);
        brigeWall.SetActive(true);
        foreach (ParticleSystem particle in spawnWallParticles)
        {
            particle.Play();
        }
        foreach (ParticleSystem particle in villageWallParticles)
        {
            particle.Play();
        }
        foreach (ParticleSystem particle in castleWallParticles)
        {
            particle.Play();
        }
        foreach (ParticleSystem particle in brigeWallParticles)
        {
            particle.Play();
        }
        yield return new WaitForSeconds(2.8f);
        foreach (ParticleSystem particle in spawnWallParticles)
        {
            particle.Pause();
        }
        foreach (ParticleSystem particle in villageWallParticles)
        {
            particle.Pause();
        }
        foreach (ParticleSystem particle in castleWallParticles)
        {
            particle.Pause();
        }
        foreach (ParticleSystem particle in brigeWallParticles)
        {
            particle.Pause();
        }
    }

    private IEnumerator changeMusicToBattle()
    {
        while (mainAudioSourse[0].volume > 0)
        {
            mainAudioSourse[0].volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        if (mainAudioSourse[0].volume == 0)
        {
            while (mainAudioSourse[1].volume <= 0.7)
            {
                mainAudioSourse[1].volume += 0.1f;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator changeMusicToMain()
    {
        while (mainAudioSourse[1].volume > 0)
        {
            mainAudioSourse[1].volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        if (mainAudioSourse[1].volume == 0)
        {
            while (mainAudioSourse[0].volume <= 0.7)
            {
                mainAudioSourse[0].volume += 0.1f;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!wallEnable)
        {
            if (other.gameObject.CompareTag("Player")) playerEnter = true;
            if (other.gameObject.CompareTag("Enemy")) enemyEnter = true;
        }
        
    }
}
