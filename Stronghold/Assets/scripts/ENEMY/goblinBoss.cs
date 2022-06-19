using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class goblinBoss : MonoBehaviour
{

    private GameObject _target;
    private HUDBarScript _hudScript;
    private PlayerControll _playerControl;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Animator _playerAnimator;

    public float _health;

    private Canvas _canvas;
    private Slider _healthSlider;

    private AudioSource[] _audioSource;

    [SerializeField]
    EnemyBomb _bombPref;
    [SerializeField]
    private AudioClip[] _audioClips;
    [SerializeField]
    private AudioClip[] _atkVoiceSound;
    [SerializeField]
    private AudioClip[] _dashSound;
    [SerializeField]
    AudioClip[] whoosh;

    [SerializeField]
    private GameObject _simpleAtkParticle;

    private float _rotationSpeed;

    [SerializeField]
    private goblinBossLocation _location;

    private bool _isAtack = false;
    private bool _isTired = false;
    private bool _isSimpleAtack = false;
    private bool _isFastAtack = false;
    private bool _isHome = true;
    private bool _isFindPos = false;
    private bool _canMove = true;
    private bool _waitRotateCorStart = false;

    private bool _canDoFirstStateActions = true;
    private bool _secondWaveStart = false;
    private bool _secondWave = false;
    private bool _isThrowSmoke = false;

    private bool _soundIsPlay = false;



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
        _hudScript = GameObject.Find("Canvas").GetComponentInChildren<HUDBarScript>();
        _playerControl = _target.GetComponent<PlayerControll>();
        _animator = GetComponent<Animator>();
        _playerAnimator = _target.GetComponent<Animator>();
        _home = _homeParent.GetComponentsInChildren<Transform>();
        _rotationSpeed = _agent.angularSpeed;
        _audioSource = GetComponents<AudioSource>();

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
                if (!_isThrowSmoke) 
                {
                    StartCoroutine(throwSmoke(distance));
                    StartCoroutine(checkPlayerInSmoke());
                    _soundIsPlay = false;
                } 

                if (_playerControl._inSmoke)
                {
                    _dmg = 4;
                    if (_canMove && _waitRotateCorStart) StartCoroutine(waitRotateToTarget(0.5f));
                    RotateToTarget(_target.transform);
                    if (distance < _vewDistance && distance > _atkDistance && _canMove)
                    {
                        transform.position += transform.forward * _speed * Time.deltaTime;
                        _animator.SetBool("isRun", true);
                        if (!_soundIsPlay)
                        {
                            _soundIsPlay=true;
                            _audioSource[2].PlayOneShot(_dashSound[Random.Range(0, _dashSound.Length)]);
                        }
                        
                    }
                    else if (distance < _atkDistance && !_isFastAtack)
                    {
                        _soundIsPlay = false;
                        _isFastAtack = true;
                        _animator.SetBool("isRun", false);
                        _animator.SetTrigger("isAttack");
                        _audioSource[1].PlayOneShot(_atkVoiceSound[Random.Range(0, _atkVoiceSound.Length)]);

                    }
                }
               
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
                        if (!_soundIsPlay)
                        {
                            _soundIsPlay = true;
                            _audioSource[2].PlayOneShot(_dashSound[Random.Range(0, _dashSound.Length)]);
                        }

                    }
                    else if (distance < _atkDistance && !_isSimpleAtack)
                    {
                        _dashCount++;
                        _simpleAtkParticle.SetActive(false);
                        _soundIsPlay = false;
                        _waitRotateCorStart = false;
                        _canMove = false;
                        _isSimpleAtack = true;
                        _isFindPos = false;
                        StartCoroutine(dashDelayCor());
                        StartCoroutine(waitRotateToHomeCor(0.6f));
                        _animator.SetBool("isRun", false);
                        _animator.SetTrigger("isSimpleAttack");
                        _audioSource[1].PlayOneShot(_atkVoiceSound[Random.Range(0, _atkVoiceSound.Length)]);
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
                    _audioSource[2].PlayOneShot(_dashSound[Random.Range(0, _dashSound.Length)]);
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
        
        if (_health <= 350 && !_secondWaveStart)
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
    private void simpleAtkEffect()
    {
        _audioSource[0].PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);
        _simpleAtkParticle.SetActive(true);
    } 
    private void startSmokeDelayCor() => StartCoroutine(smokeDelay());
    private IEnumerator checkPlayerInSmoke()
    {
        yield return new WaitForSeconds(4);
        if (!_playerControl._inSmoke)
        {
            StartCoroutine(smokeDelay());
        }
    }
    private IEnumerator smokeDelay()
    {
        _dmg = _startDmg;
        _animator.SetBool("isRun", false);
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

    private IEnumerator throwSmoke(float dist)
    {
        _isThrowSmoke = true;
        SpawnBomb(dist);
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

    internal void SpawnBomb(float dist)
    {
        var forward = transform.forward;
        var bomb = Instantiate(_bombPref,
            transform.position + Vector3.up * 8F + forward * 10F,
            Quaternion.identity);
        // bomb.GetComponent<Rigidbody>().velocity = _playerRigidbody.velocity;
        bomb.direction = (forward + transform.up / 2).normalized;
        bomb.force = dist;
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 2f;
        sphereCollider.center = new Vector3(0, 2f, 2f);
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
        _hudScript.StartHeal();
        _location.disablecastleWall();
        _location.disableSpawnWall();
        Destroy(gameObject, 0.4f);
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
        _audioSource[0].pitch = Random.Range(0.7f, 1.2f);
        _audioSource[0].PlayOneShot(_audioClips[soundNumber]);

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
