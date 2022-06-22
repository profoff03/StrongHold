using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBullet : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private GameObject _target;
    private float _rotationSpeed;
    [SerializeField]
    private float _viewDistance;
    [SerializeField]
    private float _bulletDistance;
    private bool _isSpawned;

    [SerializeField]
    private Transform Spawner;
    [SerializeField]
    private GameObject Arrow;
    [SerializeField]
    private GameObject Effect;

    private bool _isSees;
    private float _fixedTIme;
    private float _defSpeed;
    private bool _fnished;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("Player");
        _rotationSpeed = _agent.angularSpeed;
        _defSpeed = _agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        float disranceToLayer = Vector3.Distance(_agent.transform.position, _target.transform.position);
        if(disranceToLayer < _viewDistance) { _isSees = true; _animator.SetBool("RunForwart", true); }

        if(_isSees)
        {
            RotateToTarget();
            if (disranceToLayer < _bulletDistance)
            {
                _agent.velocity = Vector3.zero;
                _agent.speed = 0;
                _animator.SetBool("RunForwart", false);
                CheckTime(4);     
                if (Time.time < _fixedTIme)
                {
                    if (!_isSpawned)
                    {
                        Effect.SetActive(true);
                        _animator.SetTrigger("Attack");
                        _isSpawned = true;
                    }
                }
                else
                {
                    _agent.speed = _defSpeed;
                    _fixedTIme = 0;
                    _isSpawned = false;
                }
            } else
            {
                if(disranceToLayer > _bulletDistance && !Effect.activeSelf)
                _animator.SetBool("RunForwart", true);
                _agent.SetDestination(_target.transform.position);
            }
        }
    }

    private void spawnArrow()
    {
        Effect.SetActive(false);
        Instantiate(Arrow, Spawner.transform.position, transform.rotation);
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
            _rotationSpeed * Time.deltaTime * 3.0f
            );
    }

    public static bool IsAnimationPlaying(Animator animator, string AnimationName)
    {
        // берем информацию о состоянии 
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // смотрим, есть ли в нем имя какой-то анимации, то возвращаем true
        if (animatorStateInfo.IsName(AnimationName))
            return true;
        return false;
    }

    private void CheckTime(float waitTime)
    {
        if (_fixedTIme == 0)
        {
            _fixedTIme = Time.time + waitTime;
        }
    }
}
