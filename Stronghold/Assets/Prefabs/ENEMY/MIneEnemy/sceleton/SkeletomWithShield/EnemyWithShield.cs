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
    private bool _isAttack;
    private float _rotationSpeed;
    private bool _IsSees;
    private float _defSpeed;
    private bool _remoteShield;
    private bool _isStan;
    private float _fixedTIme;
    private bool Started = false;
    private bool Attacked = false;
    private float AttackCoolDown;
    [SerializeField]
    private bool _goBack;
    private bool _fight;

    private bool StrongReact;
    private bool SimpleReact;
    [SerializeField]
    float maxHealth;
    public float health;
    private Canvas canvas;
    private Slider healthSlider;

    private bool _simpleWas;
    private bool _strongWas;

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

        _isStan = false;
        #region health
        health = maxHealth;

        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if(!Attacked)
        {
            AttackCoolDown += Time.deltaTime;
        }
        if(Shield.highDamage) { StrongReact = true; }
        if(Shield.lowDamage) { SimpleReact = true; }
        if(StrongReact)
        {
            _animator.SetBool("Sees", false);
            RotateToTarget();
            _isStan=true;
            _remoteShield = true;
            _agent.speed = 0;
            _agent.velocity = Vector3.zero;
            if(!_strongWas) { _animator.SetTrigger("StrongReact 0"); _strongWas = true; }
            
            CheckTime(3.5f);
            if(Time.time > _fixedTIme)
            {
                _animator.SetBool("Sees", true);
                _isStan = false;
                _remoteShield = false;
                StrongReact = false;
                _fixedTIme = 0.0f;
                _strongWas = false;
                Shield.lowDamage = false;
                Shield.highDamage = false;
            }
        }
        if(SimpleReact)
        {
            RotateToTarget();
            if(!_simpleWas) { _animator.SetTrigger("SimpleReact 0"); _simpleWas = true; }
            
            _remoteShield = false;
            CheckTime(1f);
            if(Time.time > _fixedTIme)
            {
                SimpleReact = false;
                _simpleWas = false;
                _fixedTIme = 0;
                Shield.lowDamage = false;
                Shield.highDamage = false;
            }
        }

        float _distanceToPlayer = Vector3.Distance(_agent.transform.position, _target.transform.position);
        if (_distanceToPlayer < _viewDistance && !_IsSees)
        {
            _animator.SetBool("Sees", true);
            RotateToTarget();
            _IsSees = true;
        }
        
        if(_IsSees && !StrongReact && !SimpleReact)
        {
            
            RotateToTarget();
            if(_distanceToPlayer < _attackDistance && AttackCoolDown > 3.0f && _distanceToPlayer < _viewDistance)
            {
                
                _isAttack = true;
                _remoteShield = true;
                RotateToTarget();
                if (!_fight) 
                { 
                    _animator.SetTrigger("Attack");
                    DoHit();
                    
                    _fight = true; 
                }
                    
                

                CheckTime(2.2f);
                if(Time.time > _fixedTIme)
                {
                    _remoteShield = false;
                    AttackCoolDown = 0.0f;
                    print("CoolDown 000000");
                    _fixedTIme = 0;
                }
                
                _animator.SetBool("RunBack", true);
                _isAttack = false;
            }


            if(_distanceToPlayer > _StayDistance && !_isAttack)
            {
                _fight = false;
                _animator.SetBool("RunBack", false);
                _agent.speed = _defSpeed;
                RotateToTarget();
                _animator.SetBool("Run", true);
                _agent.SetDestination(_target.transform.position);
            } 
            if(!_isAttack && _distanceToPlayer < _StayDistance)
            {
                _animator.SetBool("Run", false);
                RotateToTarget();                
                _agent.speed = 0;
                _agent.velocity = Vector3.zero;
            }
        }
        canvas.transform.LookAt(canvas.worldCamera.transform);
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 9f;
        sphereCollider.center = new Vector3(0, 5f, 4f);

        if (gameObject.layer == 8) sphereCollider.tag = "fireHit";
        else sphereCollider.tag = "EnemyHit";

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
        if (other.gameObject.CompareTag("Hit") && _remoteShield)
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
        }
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
