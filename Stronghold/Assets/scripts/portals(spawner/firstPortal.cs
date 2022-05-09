using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class firstPortal : MonoBehaviour
{
    [SerializeField]
    Transform startEnemy;

    [SerializeField]
    Transform _startTarget;

    [SerializeField]
    GameObject[] enemyPrefabs;

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

    [System.Obsolete]
    void Start()
    {

        StartCoroutine(CheckFirstEnemy());
    }

    private void Update()
    {
        if (detroyPortal) portal.transform.position -= new Vector3 (0, 0.1f,0);

    }

    [System.Obsolete]
    private IEnumerator CheckFirstEnemy()
    {
        bool allEnemyDie = false;
        while (!allEnemyDie)
        {
            if(startEnemy.GetChildCount() <= 1)
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
        foreach(GameObject enemy in enemyPrefabs)
        {
            for(int i = 0; i < 2; i++)
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
            if (transform.GetChildCount() == 0)
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
        foreach (GameObject enemy in enemyPrefabs)
        {
            for (int i = 0; i < 3; i++)
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
            if (transform.GetChildCount() == 0)
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
        Instantiate(explosionFX, transform.position, Quaternion.identity,transform);
        detroyPortal = true;
        Destroy(portal, 1.5f);
        Destroy(firstPortalLoc, 5f);
    }
}
