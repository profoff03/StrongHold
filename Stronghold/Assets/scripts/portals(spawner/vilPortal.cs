using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vilPortal : MonoBehaviour
{
    [SerializeField]
    Transform ghostPos;

    AudioSource[] mainAudioSourse;
    AudioSource playerAudioSource;
    [SerializeField]
    AudioClip explosionSound;
    [SerializeField]
    AudioClip rockStartSound;
    [SerializeField]
    AudioClip rockEndSound;

    Transform[] spawnerTransforms;

    [SerializeField]
    Transform startEnemy;

    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    GameObject[] enemyPrefabs;

    [SerializeField]
    GameObject bossPrefab;
    GameObject bossPref;

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

    public bool playerEnter = false;

    bool canBuildWall = false;

    internal bool roaring = false;

    [System.Obsolete]
    void Start()
    {
        //TODO
        Check();
        playerAudioSource = playerTransform.GetComponent<AudioSource>();
        mainAudioSourse = Camera.main.GetComponents<AudioSource>();

        spawnerTransforms = transform.GetComponentsInChildren<Transform>();
        StartCoroutine(CheckFirstEnemy());
        stoneWallParticles = stoneParticles.GetComponentsInChildren<ParticleSystem>();
    }

    //TODO
    private void Check()
    {
        if (PlayerPrefs.HasKey("EnemyNearPortal4"))
        {
            if (PlayerPrefs.GetInt("EnemyNearPortal4") == 1) { Destroy(startEnemy); }
        }
        if (PlayerPrefs.HasKey("DestroyPortal4"))
        {
            if (PlayerPrefs.GetInt("DestroyPortal4") == 1)
            {
                destroyPortal();
            }
        }
    }

    [System.Obsolete]
    private void Update()
    {
        //float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (detroyPortal) portal.transform.position -= new Vector3(0, 0.2f, 0);
        if (playerEnter && !wallEnable)
        {
            if (canBuildWall)
                StartCoroutine(enableWall());

            StartCoroutine(changeMusicToBattle());
            //Debug.Log(distance);
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
            if (startEnemy.GetChildCount() <= 4)
            {
                //TODO
                PlayerPrefs.SetInt("EnemyNearPortal4", 1);
                Debug.Log("allDie");
                canBuildWall = true;
                allEnemyDie = true;
                StartCoroutine(SpawnEnemyFirstWave());
            }
            yield return new WaitForSeconds(5);
        }

    }

    [System.Obsolete]
    private IEnumerator SpawnEnemyFirstWave()
    {
        int count = 0;
        foreach (GameObject enemy in enemyPrefabs)
        {
            count++;
            for (int i = 0; i < 2; i++)
            {
                int r = Random.Range(0, spawnerTransforms.Length);
                GameObject thisEnemy = Instantiate(enemy, spawnerTransforms[r].position, spawnerTransforms[r].rotation, transform);
                if (count == 2) 
                {
                    simpleGhost ghost = thisEnemy.GetComponent<simpleGhost>();
                    ghost.firstPos = ghostPos;
                    ghost.secondPos = ghostPos;
                }
                yield return new WaitForSeconds(spawnDelay);
                

            }
        }
        yield return new WaitForSeconds(4);

        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if (transform.GetChildCount() <= 2)
            {
                Debug.Log("allDie");
                allEnemyDie = true;
                StartCoroutine(SpawnEnemySecondWave());
            }
            yield return new WaitForSeconds(5);
        }

    }

    [System.Obsolete]
    private IEnumerator SpawnEnemySecondWave()
    {
        Instantiate(bossPrefab, transform.position, transform.rotation, transform);
        yield return new WaitForSeconds(4);
        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if (transform.GetChildCount() <= 2)
            {
                //TODO
                PlayerPrefs.SetInt("DestroyPortal4", 1);
                Debug.Log("allDie");
                allEnemyDie = true;
                destroyPortal();

            }
            yield return new WaitForSeconds(5);
        }
    }

    [System.Obsolete]


    void destroyPortal()
    {
        playerAudioSource.PlayOneShot(explosionSound);
        playerAudioSource.PlayOneShot(rockEndSound);
        Instantiate(explosionFX, transform.position, Quaternion.identity, transform);
        detroyPortal = true;
        foreach (ParticleSystem particle in stoneWallParticles)
        {
            particle.Play();
        }
        Destroy(portal, 1.5f);
        Destroy(firstPortalLoc, 5f);

        StartCoroutine(changeMusicToMain());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerEnter = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerEnter = false;
    }
}

