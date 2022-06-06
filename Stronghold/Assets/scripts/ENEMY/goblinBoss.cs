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
    private Animator _playerAnimator;

    private float _health;

    private Canvas _canvas;
    private Slider _healthSlider;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _audioClips;

    private float _rotationSpeed;


    private bool _isAtack = false;
    private bool _isTired = false;
    private bool _isSimpleAtack = false;
    private bool _isHome = false;
    private bool _isChangeHome = false;
    private bool _canMove = true;



    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private Transform _homeParent;
    private Transform _posToStay;
    private Transform[] _home;

    [SerializeField]
    private float _dashDelay;
    [SerializeField]
    private float _tiredDelay;
    [SerializeField]
    private float _dashCountToTired;
    private float _dashCount = 0;

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
        _playerAnimator = _target.GetComponent<Animator>();
        _home = _homeParent.GetComponentsInChildren<Transform>();
        _rotationSpeed = _agent.angularSpeed / 1.4f;
        _audioSource = GetComponent<AudioSource>();

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

        if (!_isAtack && !_isTired)
        {
            RotateToTarget(_target.transform);
            if (distance < _vewDistance && distance > _atkDistance && _canMove)
            {
                transform.position += transform.forward * _speed * Time.deltaTime;
            }
            else if (distance < _atkDistance && !_isSimpleAtack)
            {
                _dashCount++;
                _canMove = false;
                _isSimpleAtack = true;
                StartCoroutine(waitCor(0.5f));
                StartCoroutine(dashDelayCor());
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
                _canMove = false;
                if (_dashCount == _dashCountToTired && !_isTired)
                {
                    _isTired = true;
                    _dashCount = 0;
                }
                RotateToTarget(_target.transform);
                if (distance < _atkDistance && !_isTired && !_isChangeHome)
                {
                    _isChangeHome = true;
                    _posToStay = _home[Random.Range(0, _home.Length)];
                    _isHome = false;                 
                    StartCoroutine(changeHomeCor(0.6f));
                }
            }
        }

        if (_isTired)
        {
            _animator.SetBool("isTired", true);
            StartCoroutine(tiredDelayCor());
        }



        _canvas.transform.LookAt(_canvas.worldCamera.transform);
    }
    private IEnumerator tiredDelayCor()
    {
        yield return new WaitForSeconds(_tiredDelay);
        _animator.SetBool("isTired", false);
        _isTired = false;
    }

    private IEnumerator changeHomeCor(float delay)
    {
        yield return new WaitForSeconds(delay);
        _canMove = true;
        _isChangeHome = false;

    }

    private IEnumerator waitCor(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.tag = "Enemy";
        _isAtack = true;
        yield return new WaitForSeconds(delay);
        _canMove = true;

    }
    private IEnumerator dashDelayCor()
    {
        yield return new WaitForSeconds(_dashDelay);
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

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void TakeDamage(float? dmg)
    {
       
       if (_isTired)
        {
            if (IsAnimationPlayerPlaying("Strong", 0))
            {
                _animator.SetTrigger("strongReact");

            }
            else
            {
                _animator.SetTrigger("react");
            }
        }
        


        int soundNumber = Random.Range(0, 20);
        if (soundNumber <= 10) soundNumber = 0;
        if (soundNumber > 10) soundNumber = 1;
        _audioSource.pitch = Random.Range(0.7f, 1.2f);
        _audioSource.PlayOneShot(_audioClips[soundNumber]);

        dmg ??= 0;
        _health -= (float)dmg;
        if (_health <= 0.001) _health = 0f;

        if (_health == 0) Kill();
        _healthSlider.value = _health;

        transform.tag = "Enemy";
    }

    public bool IsAnimationPlayerPlaying(string animationName, int index)
    {
        var animatorStateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(index);
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    } //Check animation state
    public bool IsAnimationPlaying(string animationName, int index)
    {
        var animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(index);
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);

        }
    }
}
