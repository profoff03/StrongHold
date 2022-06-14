using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buffSpawner : MonoBehaviour
{
    #region TakeToBeatTheGhosts
    [SerializeField]
    private float _minRespwanTime;
    [SerializeField]
    private float _maxRespwanTime;
    private PlayerControll _playerControll;
    private bool _isSpawned = true;
    private bool _isTouch = false;
    public GameObject Thing;
    private GameObject _thing;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _thing = Instantiate(Thing, transform.position + new Vector3(0,5f,0), transform.rotation, transform);
        _playerControll = GameObject.Find("Player").GetComponent<PlayerControll>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isSpawned)
        {
            if (_isTouch)
            {
                Destroy(_thing);
                _playerControll.killGhostEffectTrue();
                _isTouch = false;
                _isSpawned = false;
                StartCoroutine(spawnThing());
            }
                
        }
    }
    private IEnumerator spawnThing()
    {
        yield return new WaitForSeconds(Random.Range(_minRespwanTime, _maxRespwanTime));
        _thing = Instantiate(Thing, transform.position + new Vector3(0, 5f, 0) , transform.rotation, transform);
        _isSpawned = true;   
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isSpawned)
        {
            if (other.CompareTag("Player") && _playerControll.canTakeThing) _isTouch = true;
        }
    }
}
