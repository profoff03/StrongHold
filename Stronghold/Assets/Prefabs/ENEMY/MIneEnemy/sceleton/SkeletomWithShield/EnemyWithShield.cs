using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyWithShield : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _target;

    [SerializeField]
    float dmg;
    [SerializeField, Range(0f, 30f)]
    private float _StayDistance;
    [SerializeField, Range(0f, 100f)]
    private float _viewDistance;
    [SerializeField, Range(0f, 10f)]
    private float _attackDistance;
    public bool _isAttack = false;
    private float _rotationSpeed;
    private bool _IsSees;
    private float _defSpeed;
    public bool _remoteShield;
    private float _fixedTIme;
    [SerializeField]
    private float AttackCoolDown;
    [SerializeField]
    private bool _goBack;

    [SerializeField]
    private float maxHealth;
    private float health;
    private Canvas canvas;
    private Slider healthSlider;

    private bool _simpleWas;
    private bool _strongWas;
    bool canAtack = true;
    bool iHome = false;

    [SerializeField]
    private ForShieldScript Shield;

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.Find("Player").GetComponent<Transform>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _rotationSpeed = _agent.angularSpeed;
        _defSpeed = _agent.speed;
        _remoteShield = false;

        #region health
        health = maxHealth;

        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        canvas.worldCamera = Camera.main;
        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (Shield.highDamage)
        {
            _animator.SetBool("Sees", false);
            RotateToTarget();
            _remoteShield = true;
            _agent.speed = 0;
            _agent.velocity = Vector3.zero;
            if (!_strongWas) 
            { 
                _animator.SetTrigger("StrongReact 0"); 
                _strongWas = true;
                StartCoroutine(strongReactDelay());
            }
        }
        if (Shield.lowDamage)
        {
            RotateToTarget();
            if (!_simpleWas) 
            { 
                _animator.SetTrigger("SimpleReact 0"); _simpleWas = true;
                StartCoroutine(simpleReactDelay());
            } 
        }
        float _distanceToPlayer = Vector3.Distance(_agent.transform.position, _target.transform.position);
        if (_distanceToPlayer < _viewDistance && !_IsSees)
        {
            _animator.SetBool("Sees", true);
            _IsSees = true;
        }

        if (_IsSees && !Shield.highDamage && !Shield.lowDamage)
        {
            if (_distanceToPlayer < _viewDistance)
            {
                if (!_isAttack)
                {

                    if (_distanceToPlayer > _attackDistance)
                    {
                        _agent.speed = _defSpeed;
                        _agent.SetDestination(_target.transform.position);
                        _animator.SetBool("Run", true);
                    }else if (_distanceToPlayer <= _attackDistance)
                    {
                        _isAttack = true;
                        _animator.SetBool("Run", false);
                        _animator.SetTrigger("Attack");
                    }
                }
                else
                {
                    RotateToTarget();
                    if (canAtack)
                    {
                        canAtack = false;
                        StartCoroutine(attackDelay());
                    }
                    if (!iHome)
                    {
                        if (_distanceToPlayer <= _StayDistance)
                        {
                            _agent.speed = 0;
                            _agent.velocity = Vector3.zero;
                            _animator.SetBool("Run", false);
                            _animator.SetBool("RunBack", true);
                        }
                        else if (_distanceToPlayer > _StayDistance + 5f)
                        {
                            _agent.speed = _defSpeed;
                            _agent.SetDestination(_target.transform.position);
                            _animator.SetBool("Run", true);
                            _animator.SetBool("RunBack", false);
                        }
                        else
                        {
                            iHome = true;
                            _agent.speed = 0;
                            _agent.velocity = Vector3.zero;
                            _animator.SetBool("Run", false);
                            _animator.SetBool("RunBack", false);
                        }
                    }
                    else
                    {
                        if (_distanceToPlayer > _StayDistance + 5f)
                        {
                            _agent.speed = _defSpeed;
                            _agent.SetDestination(_target.transform.position);
                            _animator.SetBool("Run", true);
                            _animator.SetBool("RunBack", false);
                        }
                        else
                        {
                            _agent.speed = 0;
                            _agent.velocity = Vector3.zero;
                            _animator.SetBool("Run", false);
                            _animator.SetBool("RunBack", false);
                        }
                    }
                    
                }

            }

        }
        
        canvas.transform.LookAt(canvas.worldCamera.transform);
    }
    private IEnumerator attackDelay()
    {
        yield return new WaitForSeconds(AttackCoolDown);
        iHome = false;
        _isAttack = false;
        canAtack = true;
    }
    private IEnumerator strongReactDelay()
    {
        yield return new WaitForSeconds(3.5f);
        _animator.SetBool("Sees", true);
        _remoteShield = false;
        _strongWas = false;
        Shield.lowDamage = false;
        Shield.highDamage = false;
        Shield.healShield();
    }
    private IEnumerator simpleReactDelay()
    {
        _remoteShield = false;
        yield return new WaitForSeconds(1);
        _simpleWas = false;
        Shield.lowDamage = false;
        Shield.highDamage = false;
    }

    void CkeckAtack()
    {
        _isAttack = true;
        gameObject.tag = "Enemy";
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 9f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "fireHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
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
    private void CheckTime(float waitTime)
    {
        if (_fixedTIme == 0)
        {
            _fixedTIme = Time.time + waitTime;
        }
    }

    private void TakeDamage(float? dmg)
    {
        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;
        if (health == 0) Kill();
        healthSlider.value = health;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_remoteShield)
        {
            if (other.gameObject.CompareTag("Hit"))
            {
                TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            }
        }
        
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
