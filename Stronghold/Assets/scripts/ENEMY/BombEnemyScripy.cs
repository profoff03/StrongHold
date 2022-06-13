using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombEnemyScripy : MonoBehaviour
{
    private bool isStartDoing = true;

    private bool isChangeDistanation = false;

    private float rotationSide;

    private AudioSource playerAudioSource;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] explosionSound;
    [SerializeField]
    private AudioClip[] loughSound;
    [SerializeField]
    private AudioClip fuseStartSound;
    [SerializeField]
    private AudioClip fuseSound;


    private GameObject _target;
    private bool _isSees;
    private Animator _animator;
    private NavMeshAgent _agent;
    [SerializeField]
    private ParticleSystem _particleSystem;
    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private float explosionDistance;
    [SerializeField]
    private float bombDamage;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float explosionTime;
    private float RotationSpeed;

    private Rigidbody _rb;
    private bool can = true;
    private Vector3 _force;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _isSees = false;
        _target = GameObject.Find("Player");
        RotationSpeed = _agent.angularSpeed / 1.3f;
        StartCoroutine(startDoing());
        playerAudioSource = _target.GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {        
        transform.position += _force;

        float DistanceToPlayer = Vector3.Distance(_agent.transform.position, _target.transform.position);
        //  Debug.Log(DistanceToPlayer);
        if (!isStartDoing)
        {
            if (DistanceToPlayer < viewDistance && !_isSees)
            {
                _isSees = true;
                audioSource.PlayOneShot(loughSound[Random.Range(0, loughSound.Length)]);
                audioSource.PlayOneShot(fuseStartSound);
                audioSource.PlayOneShot(fuseSound);
                StartCoroutine(explosionTimer());
                
            }
            else if (DistanceToPlayer > viewDistance) _isSees = false;

            if (_isSees)
            {
                _animator.SetBool("isRunForward", true);
                _agent.SetDestination(_target.transform.position);
                _rb.AddForce(transform.forward * moveSpeed * Time.deltaTime * 100000);
                RotateToTarget();
            }

            if (DistanceToPlayer < explosionDistance)
            {
                explosion();
            }
            else StartCoroutine(changeDistanation());
        }
        else _rb.AddForce(transform.forward * moveSpeed * Time.deltaTime * 80000);
    }
    
    private void explosion()
    {
        if (can)
        {
            moveSpeed = 0;
            Instantiate(_particleSystem, transform.position, Quaternion.identity);
            playerAudioSource.PlayOneShot(explosionSound[Random.Range(0,explosionSound.Length)]);
            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 10f;
            sphereCollider.center = new Vector3(0, 5f, 4f);
            sphereCollider.tag = "punchHit";
            sphereCollider.gameObject.AddComponent<DamageProperty>();
            sphereCollider.GetComponent<DamageProperty>().Damage = bombDamage;

            Destroy(gameObject, 0.02f);
            can = false;
        }
    }

    private IEnumerator explosionTimer()
    {
        yield return new WaitForSeconds(explosionTime);
        audioSource.PlayOneShot(fuseSound);
        yield return new WaitForSeconds(explosionTime);
        explosion();
    }


    private IEnumerator startDoing()
    {
        _animator.SetBool("isRunForward", true);
        yield return new WaitForSeconds(2);
        _animator.SetBool("isRunForward", false);
        isStartDoing = false;
    }

    private IEnumerator changeDistanation()
    {

        yield return new WaitForSeconds(2);
        isChangeDistanation = false;
    }
    private void RotateToTarget()
    {
        Vector3 lookVector;

        if (isChangeDistanation)
        {
            lookVector = rotationSide * transform.position;
        }
        else
            lookVector = _target.transform.position - _agent.transform.position;


        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            RotationSpeed * Time.deltaTime
            );
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Push"))
        {
            var control = other.gameObject.transform.parent.gameObject.GetComponent<PlayerControll>();
            var direction = transform.position - control.transform.position;
            direction.y = 0;
            Debug.Log(direction);
            StartCoroutine(Push(direction.normalized * control._puchForce));
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!isChangeDistanation)
            {
                rotationSide = Random.Range(-10, 10);
                if (rotationSide >= 0) rotationSide = 1;
                else if (rotationSide < 0) rotationSide = -1;
                StartCoroutine(changeDistanation());
            }
            isChangeDistanation = true;

        }

    }
    private IEnumerator Push(Vector3 force)
    {
        _force = force.normalized;
        Debug.Log($"{force}, {force.magnitude}");
        yield return new WaitForSeconds(force.magnitude / 30f);
        _force.x = 0;
        _force.z = 0;
        _force.y = 0;
    }
    
}
