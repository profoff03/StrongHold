using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class firstPortal : MonoBehaviour
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
    Transform startEnemy;

    [SerializeField]
    HUDBarScript hud;

    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    GameObject[] enemyPrefabs;

    [SerializeField]
    GameObject stoneParticles;

    ParticleSystem[] stoneWallParticles;

    [SerializeField]
    GameObject firstPortalLoc;

    [SerializeField]
    GameObject portal;
    
    [SerializeField]
    GameObject explosionFX;

    [SerializeField]
    float enemyStartForce;

    [SerializeField]
    float spawnDelay;

    bool detroyPortal = false;
    bool wallEnable = false;

    [System.Obsolete]
    void Start()
    {
        playerAudioSource = playerTransform.GetComponent<AudioSource>();
        mainAudioSourse = Camera.main.GetComponents<AudioSource>();
        StartCoroutine(CheckFirstEnemy());
        stoneWallParticles = stoneParticles.GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (detroyPortal) portal.transform.position -= new Vector3 (0, 0.1f,0);
        if (distance <= 150f && !wallEnable)
        {
            StartCoroutine(enableWall());
            StartCoroutine(changeMusicToBattle());
            //Debug.Log(distance);
        }

    }
    private IEnumerator enableWall()
    {
        playerAudioSource.PlayOneShot(rockStartSound);
        wallEnable = true;
        stoneParticles.SetActive(true);
        foreach (ParticleSystem particle in stoneWallParticles)
        {
            particle.Play();
        }
        yield return new WaitForSeconds(2.8f);
        foreach (ParticleSystem particle in stoneWallParticles)
        {
            particle.Pause();
        }
    }

    [System.Obsolete]
    private IEnumerator CheckFirstEnemy()
    {
        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if(startEnemy.GetChildCount() <= 1)
            {
                allEnemyDie = true;
                StartCoroutine(SpawnEnemyFirstWave());
            }
            yield return new WaitForSeconds(5);
        }
        
    }

    [System.Obsolete]
    private IEnumerator SpawnEnemyFirstWave()
    {
        foreach(GameObject enemy in enemyPrefabs)
        {
            for(int i = 0; i < 2; i++)
            {
                Instantiate(enemy, transform.position, transform.rotation, transform);
                yield return new WaitForSeconds(spawnDelay);
                
            }
        }
        yield return new WaitForSeconds(4);
        
        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if (transform.GetChildCount() == 0)
            {
                allEnemyDie = true;
                StartCoroutine(SpawnEnemySecondWave());
            }
            yield return new WaitForSeconds(5);
        }

    }

    [System.Obsolete]
    private IEnumerator SpawnEnemySecondWave()
    {
        int k = 1;
        foreach (GameObject enemy in enemyPrefabs)
        {
            for (int i = 0; i < k; i++)
            {
                Instantiate(enemy, transform.position, transform.rotation, transform);
                yield return new WaitForSeconds(spawnDelay);
            }
            k++;
        }
        yield return new WaitForSeconds(4);
        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if (transform.GetChildCount() == 0)
            {
                Debug.Log("allDie");
                allEnemyDie = true;
                
                destroyPortal();

            }
            yield return new WaitForSeconds(5);
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

    void destroyPortal()
    {
        Instantiate(explosionFX, transform.position, Quaternion.identity,transform);
        playerAudioSource.PlayOneShot(explosionSound);
        playerAudioSource.PlayOneShot(rockEndSound);
        detroyPortal = true;
        foreach (ParticleSystem particle in stoneWallParticles)
        {
            particle.Play();
        }
        //Destroy(portal, 1.3f);
        Destroy(firstPortalLoc, 2f);
        hud.StartHeal();
        StartCoroutine(changeMusicToMain());
    }
}
