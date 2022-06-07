using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class goblinBoss : MonoBehaviour
{

    private GameObject _target;
    private PlayerControll _playerControl;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Animator _playerAnimator;

    public float _health;

    private Canvas _canvas;
    private Slider _healthSlider;

    private AudioSource _audioSource;

    [SerializeField]
    EnemyBomb bombPref;
    [SerializeField]
    private AudioClip[] _audioClips;

    private float _rotationSpeed;


    public bool _isAtack = false;
    public bool _isTired = false;
    public bool _isSimpleAtack = false;
    public bool _isFastAtack = false;
    public bool _isHome = true;
    public bool _isFindPos = false;
    public bool _canMove = true;
    public bool _waitRotateCorStart = false;

    public bool _canDoFirstStateActions = true;
    public bool _secondWaveStart = false;
    public bool _secondWave = false;
    public bool _isThrowSmoke = false;



    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private Transform _homeParent;
    private Transform _posToStay;
    private Transform[] _home;

    [SerializeField]
    private float _dashDelay;
    [SerializeField]
    private float _smokeDelay;
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
    private float _startDmg;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("Player");
        _playerControl = _target.GetComponent<PlayerControll>();
        _animator = GetComponent<Animator>();
        _playerAnimator = _target.GetComponent<Animator>();
        _home = _homeParent.GetComponentsInChildren<Transform>();
        _rotationSpeed = _agent.angularSpeed;
        _audioSource = GetComponent<AudioSource>();

        _startDmg = _dmg;

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
        RotateToTarget(_target.transform);
        if (!_isTired)
        {
            if (_secondWave && _isHome)
            {
                if (!_isThrowSmoke) StartCoroutine(throwSmoke());

                if (_playerControl._inSmoke)
                {
                    _dmg = 1;
                    if (_canMove && _waitRotateCorStart) StartCoroutine(waitRotateToTarget(0.5f));
                    RotateToTarget(_target.transform);
                    if (distance < _vewDistance && distance > _atkDistance && _canMove)
                    {
                        transform.position += transform.forward * _speed * Time.deltaTime;
                        _animator.SetBool("isRun", true);
                    }
                    else if (distance < _atkDistance && !_isFastAtack)
                    {
                        _isFastAtack = true;
                        _animator.SetBool("isRun", false);
                        _animator.SetTrigger("isAttack");
                        

                    }
                }
                else _dmg = _startDmg;
            }
            if (!_isAtack && _isHome)
            {

                if (_canDoFirstStateActions)
                {
                    if (_canMove && _waitRotateCorStart) StartCoroutine(waitRotateToTarget(1));
                    RotateToTarget(_target.transform);
                    if (distance < _vewDistance && distance > _atkDistance && _canMove)
                    {
                        transform.position += transform.forward * _speed * Time.deltaTime;
                        _animator.SetBool("isRun", true);
                    }
                    else if (distance < _atkDistance && !_isSimpleAtack)
                    {
                        _dashCount++;
                        _waitRotateCorStart = false;
                        _canMove = false;
                        _isSimpleAtack = true;
                        _isFindPos = false;
                        StartCoroutine(dashDelayCor());
                        StartCoroutine(waitRotateToHomeCor(0.6f));
                        _animator.SetBool("isRun", false);
                        _animator.SetTrigger("isSimpleAttack");

                    } 
                }
            }
            else if (!_isHome)
            {
                if (!_isFindPos)
                {
                    _posToStay = _home[Random.Range(0, _home.Length)];
                    _isFindPos = true;
                }

                if (_canMove)
                {
                    _agent.transform.position = _posToStay.position;
                    _isHome = true;
                    _canMove = false;
                }

            }
            else if (_isHome)
            {
                if (_dashCount == _dashCountToTired)
                {
                    _isTired = true;
                    _animator.SetBool("isTired", _isTired);
                    StartCoroutine(tiredDelayCor());
                    _dashCount = 0;
                }else if (distance < _atkDistance)
                {
                    _isFindPos = false;
                    _isHome = false;
                    _canMove = true;
                }
            }
        }
        
        if (_health <= 1000 && !_secondWaveStart)
        {
            _secondWaveStart = true;
            _secondWave = true;
            _canDoFirstStateActions = false;

            _dashCountToTired++;
            _tiredDelay++;
            _dashDelay--;
        }
        

            _canvas.transform.LookAt(_canvas.worldCamera.transform);
    }

    private void startSmokeDelayCor() => StartCoroutine(smokeDelay());
    private IEnumerator smokeDelay()
    {
        _waitRotateCorStart = false;
        _isFindPos = false;
        _secondWave = false;
        _isHome = false;
        _isAtack = false;
        _isSimpleAtack = false;
        _canDoFirstStateActions = true;
        
        yield return new WaitForSeconds(1);
        _canMove = true;
        yield return new WaitForSeconds(_smokeDelay);
        
        _secondWave = true;
        _isThrowSmoke = false;
        _isFastAtack = false;
        _canDoFirstStateActions = false;
    }

    private IEnumerator throwSmoke()
    {
        _isThrowSmoke = true;
        SpawnBomb();
        yield return new WaitForSeconds(7f);
        _playerControl._inSmoke = false;

    }

    private IEnumerator waitRotateToHomeCor(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isAtack = true;
        _isHome = false;
        yield return new WaitForSeconds(delay/2);
        _canMove = true;
    }
    private IEnumerator waitRotateToTarget(float delay)
    {
        _waitRotateCorStart = true;
        _canMove = false;
        yield return new WaitForSeconds(delay);
        _canMove = true;
    }

    private IEnumerator tiredDelayCor()
    {
        yield return new WaitForSeconds(_tiredDelay);
        _isTired = false;
        _animator.SetBool("isTired", _isTired);

    }

    private IEnumerator dashDelayCor()
    {
        yield return new WaitForSeconds(_dashDelay);
        _canMove = true;
        _isAtack = false;
        _isSimpleAtack = false;   

    }

    internal void SpawnBomb()
    {
        var forward = transform.forward;
        var bomb = Instantiate(bombPref,
            transform.position + Vector3.up * 3.3F + forward * 10F,
            Quaternion.identity);
        // bomb.GetComponent<Rigidbody>().velocity = _playerRigidbody.velocity;
        bomb.direction = (forward + transform.up / 2).normalized;
        bomb.force = 70;
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
