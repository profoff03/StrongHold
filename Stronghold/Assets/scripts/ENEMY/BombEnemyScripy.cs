using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombEnemyScripy : MonoBehaviour
{
    private GameObject _target;
    private bool _isSees;
    private Animator _animator;
    private NavMeshAgent _agent;
    [SerializeField]
    private ParticleSystem _particleSystem;
    [SerializeField]
    float viewDistance;
    [SerializeField]
    float explosionDistance;
    [SerializeField]
    float bombDamage;
    [SerializeField]
    float RotationSpeed;
    [SerializeField]
    float moveSpeed;

    Rigidbody _rb;

    private Vector3 _force;

    float _rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _isSees = false;
        _target = GameObject.Find("Player");
        _rotationSpeed = _agent.angularSpeed;
    }

    // Update is called once per frame
    void Update()
    {        
        transform.position += _force;

        float DistanceToPlayer = Vector3.Distance(_agent.transform.position, _target.transform.position);
        //  Debug.Log(DistanceToPlayer);
        if(DistanceToPlayer < viewDistance)
        {
            _isSees = true;
        }

        if(_isSees)
        {
            //_agent.transform.position += transform.forward * moveSpeed * Time.deltaTime;
            
            _animator.SetBool("isRunForward", true);
            _agent.SetDestination(_target.transform.position);
            _rb.AddForce(transform.forward * moveSpeed * 1000);
            RotateToTarget();
        }

        if (DistanceToPlayer < explosionDistance)
        {
            Instantiate(_particleSystem, _target.transform.position, Quaternion.identity);
            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 10f;
            sphereCollider.center = new Vector3(0, 5f, 4f);
            sphereCollider.tag = "EnemyHit";
            sphereCollider.gameObject.AddComponent<DamageProperty>();
            sphereCollider.GetComponent<DamageProperty>().Damage = bombDamage;

            Destroy(gameObject,0.019f);
            
        }
    }

    private void RotateToTarget()
    {
        Vector3 lookVector = _target.transform.position - _agent.transform.position;
        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
           _rotationSpeed * Time.deltaTime * RotationSpeed
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
