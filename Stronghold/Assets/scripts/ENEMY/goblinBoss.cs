using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class goblinBoss : MonoBehaviour
{

    private GameObject _target;
    private NavMeshAgent _agent;
    private Animator _animator;

    private float _health;

    private Canvas _canvas;
    private Slider _healthSlider;

    private float _rotationSpeed;

    private bool _isAtack = false;
    private bool _isSimpleAtack = false;
    private bool _isHome = false;
    private bool _canMove = true;
    
    
    
    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private Transform _homeParent;
    private Transform _posToStay;
    private Transform[] _home;


    [SerializeField]
    private float _vewDistance;
    [SerializeField]
    private float _atkDistance;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _dmg;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("Player");
        _animator = GetComponent<Animator>();

        _home = _homeParent.GetComponentsInChildren<Transform>();

        _rotationSpeed = _agent.angularSpeed / 1.4f;

        #region health
        _health = _maxHealth;

        _canvas = transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        _healthSlider = transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        _healthSlider.maxValue = _maxHealth;
        _healthSlider.value = _health;
        _canvas.worldCamera = Camera.main;
        _canvas.transform.rotation = _canvas.worldCamera.transform.rotation;
        #endregion
    }


    void Update()
    {
        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
        
        if (!_isAtack)
        {
            RotateToTarget(_target.transform);
            if (distance < _vewDistance && distance > _atkDistance && _canMove)
            {
                transform.position += transform.forward * _speed * Time.deltaTime;
            }
            else if (distance < _atkDistance && !_isSimpleAtack)
            {
                _canMove = false;
                _isSimpleAtack = true;
                StartCoroutine(waitCor());
                _animator.SetTrigger("isSimpleAttack");
                _posToStay = _home[Random.Range(0, _home.Length)];
            }
        }
        else
        {
            if (!_isHome)
            {
                float distanceToHome = Vector3.Distance(_agent.transform.position, _posToStay.transform.position);
                RotateToTarget(_posToStay);
                if (distanceToHome > _atkDistance && _canMove)
                {
                    transform.position += transform.forward * _speed * Time.deltaTime;
                }
                else if (distanceToHome < _atkDistance) _isHome = true;
            }
            else
            {
                RotateToTarget(_target.transform);
            }
        }



        _canvas.transform.LookAt(_canvas.worldCamera.transform);
    }


    private IEnumerator waitCor()
    {
        yield return new WaitForSeconds(1);
        transform.tag = "Enemy";
        _isAtack = true;
        yield return new WaitForSeconds(1);
        _canMove = true;
        yield return new WaitForSeconds(4);
        _isAtack = false;
        _isSimpleAtack = false;
        _isHome = false;
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 2f;
        sphereCollider.center = new Vector3(0, 2f, 3f);
        sphereCollider.tag = "EnemyHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = _dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
    }

    private void RotateToTarget(Transform target)
    {
        Vector3 lookVector;
        lookVector = target.transform.position - _agent.transform.position;
        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            _rotationSpeed * Time.deltaTime
            );
    }
}
