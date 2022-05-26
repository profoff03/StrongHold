using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class secondPortal : MonoBehaviour
{
    AudioSource[] mainAudioSourse;

    Transform[] spawnerTransforms;

    [SerializeField]
    Transform trigger;

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

    internal bool roaring = false;

    [System.Obsolete]
    void Start()
    {
        mainAudioSourse = Camera.main.GetComponents<AudioSource>();

        spawnerTransforms = transform.GetComponentsInChildren<Transform>();
        StartCoroutine(CheckFirstEnemy());
        stoneWallParticles = stoneParticles.GetComponentsInChildren<ParticleSystem>();
    }

    [System.Obsolete]
    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        float trigDistance = Vector3.Distance(playerTransform.position, trigger.position);

        if (detroyPortal) portal.transform.position -= new Vector3(0, 0.2f, 0);
        if ((trigDistance <=50f) && !wallEnable)
        {
            StartCoroutine(enableWall());
            StartCoroutine(changeMusicToBattle());
            //Debug.Log(distance);
        }

        if (roaring)
        {
            roaring = false;
            StartCoroutine(SpawnBossEnemyFirstWave());
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
            if (startEnemy.GetChildCount() <= 1)
            {
                Debug.Log("allDie");
                allEnemyDie = true;
                StartCoroutine(SpawnEnemyFirstWave());
            }
            yield return new WaitForSeconds(5);
        }

    }

    [System.Obsolete]
    private IEnumerator SpawnEnemyFirstWave()
    {
        foreach (GameObject enemy in enemyPrefabs)
        {
            for (int i = 0; i < 2; i++)
            {
                int r = Random.Range(0,spawnerTransforms.Length);
                GameObject thisEnemy = Instantiate(enemy, spawnerTransforms[r].position, spawnerTransforms[r].rotation, transform);
                //Animator anim = thisEnemy.GetComponent<Animator>();



                //anim.SetBool("isRunForward", true);
                yield return new WaitForSeconds(spawnDelay);
                //anim.SetBool("isRunForward", false);

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
        bossPref = Instantiate(bossPrefab, transform.position, transform.rotation, transform);
        yield return new WaitForSeconds(4);
        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if (transform.GetChildCount() <=2)
            {
                Debug.Log("allDie");
                allEnemyDie = true;

                destroyPortal();

            }
            yield return new WaitForSeconds(5);
        }
    }

    [System.Obsolete]
    private IEnumerator SpawnBossEnemyFirstWave()
    {
        foreach (GameObject enemy in enemyPrefabs)
        {
            for (int i = 0; i < 2; i++)
            {
                int r = Random.Range(1, spawnerTransforms.Length);
                GameObject thisEnemy = Instantiate(enemy, spawnerTransforms[r].position, spawnerTransforms[r].rotation, transform);
                //Animator anim = thisEnemy.GetComponent<Animator>();



                //anim.SetBool("isRunForward", true);
                yield return new WaitForSeconds(spawnDelay);
                //anim.SetBool("isRunForward", false);

            }
        }
        yield return new WaitForSeconds(4);

        bool allEnemyDie = false;
        BigOrkBoss orkBoss = bossPref.GetComponent<BigOrkBoss>();
        while (!allEnemyDie)
        {
            if (transform.GetChildCount() <= 3)
            {
                Debug.Log("allDie");
                allEnemyDie = true;
                orkBoss.allDie = allEnemyDie;
                orkBoss.isFirstState = false;
                orkBoss.canRush = true;
                orkBoss.atkDelay = orkBoss.curAtkDelay;
            }
            yield return new WaitForSeconds(5);
        }

    }


    void destroyPortal()
    {
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
}
