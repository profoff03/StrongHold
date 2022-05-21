using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class secondPortal : MonoBehaviour
{
    [SerializeField]
    Transform startEnemy;

    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    GameObject[] enemyPrefabs;

    [SerializeField]
    GameObject bossPrefab;

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

        StartCoroutine(CheckFirstEnemy());
        stoneWallParticles = stoneParticles.GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (detroyPortal) portal.transform.position -= new Vector3(0, 0.1f, 0);
        if (distance <= 90f && !wallEnable)
        {
            StartCoroutine(enableWall());
            //Debug.Log(distance);
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
                GameObject thisEnemy = Instantiate(enemy, transform.position, transform.rotation, transform);
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
        Instantiate(bossPrefab, transform.position, transform.rotation, transform);
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
    }
}
