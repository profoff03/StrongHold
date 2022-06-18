using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerControll : MonoBehaviour
{
    internal bool canDoSmth = true;

    internal Animator _playerAnimator;

    internal Rigidbody _playerRigidbody;

    internal bool isStan;

    [SerializeField]
    internal AudioClip[] _atkVoiceSound;
    [SerializeField]
    private AudioClip _atkStrongVoiceSound;
    [SerializeField]
    internal AudioClip[] _hitVoiceSound;
    [SerializeField]
    internal AudioClip[] _hitSound;
    [SerializeField]
    internal AudioClip[] _slashHitSound;
    [SerializeField]
    internal AudioClip[] _dashSound;
    internal AudioSource[] _audioSource;

    [SerializeField]
    private HUDBarScript hud;

    [SerializeField]
    Bomb bombPref;

    internal bool canThrowBomb = true;

    internal bool _inSmoke = false;

    internal bool canTakeThing = true;
    internal bool canKillGhost = false;

    bool isFire = false;

    #region ForMovement

    private Vector3 _movementVector;

    private Vector3 direction;

    private Quaternion rotation;

    private bool isPlayer;

    internal bool isMove;

    #endregion

    #region ForAttack

    [SerializeField]
    private bool ultRegenerate;

    internal bool isUlting;

    private bool mouseDown = false;

    internal bool isAtack = false;

    private bool canWalk = true;

    private float main_time = 0;

    private float bool_time = 0.3f;

    [SerializeField]
    private float _ghostEffectTime;
    [SerializeField]
    private float fireDelay;

    private void SetTriggerUntagged()
    {
        
        _FirstSlash.SetActive(false);
        _SecondSlash.SetActive(false);
        _ThirdSlash.SetActive(false);
        _4Slash.SetActive(false);
        _StrongSlash.SetActive(false);
        isAtack = false;
    }

    #endregion

    #region Serialized
    [Header("Effects")]
    
    [SerializeField]
    private GameObject blood;

    [SerializeField]
    private GameObject fireEffect;

    [SerializeField]
    private GameObject SwordEffectForGhost;

    [SerializeField]
    private GameObject _FirstSlash;
    
    [SerializeField]
    private GameObject _SecondSlash;
    
    [SerializeField]
    private GameObject _ThirdSlash;
    
    [SerializeField]
    private GameObject _4Slash;

    [SerializeField]
    private GameObject _StrongSlash;
    
    [Header("Ultimate")]
    [SerializeField] internal float _ultTime = 20f;

    [SerializeField] internal float _ultRegenerateTime = 30f;
    
    [Header("Moving")]
    [SerializeField] 
    private Camera _camera;
    
    [SerializeField] 
    private float _movementSpeed = 2f;

    [SerializeField] 
    private float _runningSpeed = 2f;

    [SerializeField] 
    private float rotationSpeed = 10f;

    [SerializeField] 
    private float _movingSens = 15f;

    [Header("AtackDamage")]
    [SerializeField] 
    private float _simpleAttackDamage = 10;

    [SerializeField] 
    private float _strongAttackDamage = 15;
    
    [SerializeField] 
    public float _puchForce = 5f;

    [SerializeField] GameObject sphere;


    #endregion

    void Awake()
    {
        SetDmg();
        _playerAnimator = GetComponent<Animator>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
        _audioSource = GetComponents<AudioSource>();
 
    }
    private void SetDmg()
    {

        _FirstSlash.GetComponent<DamageProperty>().Damage = _simpleAttackDamage;
        _SecondSlash.GetComponent<DamageProperty>().Damage = _simpleAttackDamage;
        _ThirdSlash.GetComponent<DamageProperty>().Damage = _simpleAttackDamage;
        _4Slash.GetComponent<DamageProperty>().Damage = _simpleAttackDamage;
        _StrongSlash.GetComponent<DamageProperty>().Damage = _strongAttackDamage;

    }

    private IEnumerator UltCooldown(float duration1, float duration2)
    {
        const float buff = 1.3f;
        var i = 0;
        float[] prevStats = {
            _simpleAttackDamage,
            _strongAttackDamage,
            _movementSpeed,
            _runningSpeed
        };
        _simpleAttackDamage *= buff;
        _strongAttackDamage *= buff;
        _movementSpeed *= buff;
        _runningSpeed *= buff;
        isUlting = true;
        yield return new WaitForSeconds(duration1); // ulting
        _simpleAttackDamage = prevStats[i++];
        _strongAttackDamage = prevStats[i++];
        _movementSpeed = prevStats[i++];
        _runningSpeed = prevStats[i++];
        isUlting = false;
        ultRegenerate = true;
        yield return new WaitForSeconds(duration2); // regenerate
        ultRegenerate = false;
    }

    void Update()
    {
        if (!IsAnimationPlaying("Death", 0) && !IsAnimationPlaying("ULTIMATE", 0) && !isStan && !_inSmoke  && canWalk && canDoSmth)
        {
            if (!IsAnimationPlaying("dash", 0))
                _movementVector = CalculateMovementVector();
           
            
            if (_movementVector.magnitude != 0)
            {
                isMove = true;
            }
            else
            {
                isMove = false;
            }
           
            //ultimate
            if (Input.GetKeyDown(KeyCode.Q) && !isUlting && !ultRegenerate)
            {
                StartCoroutine(UltCooldown(_ultTime, _ultRegenerateTime));
                _playerAnimator.SetTrigger("isUlt");
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                PushEnemy(5f);
            }

            if (Input.GetMouseButton(0))
            {
                if (main_time == 0.0f)
                {
                    main_time = Time.time;

                }
                if (Time.time - main_time > bool_time && !mouseDown)//long press
                {
                    //strong attack
                    _playerAnimator.SetTrigger("isStrongAtack");
                    isAtack = true;
                    mouseDown = true;
                    //DoHit(_strongAttackDamage);

                }
            } //atack
            if (Input.GetMouseButtonUp(0)) 
            {
                mouseDown = false;
                if (Time.time - main_time < bool_time)//fast click
                {
                    
                    _playerAnimator.SetTrigger("isAtack");
                    main_time = 0.0f;
                    //DoHit(_simpleAttackDamage);
                    isAtack = true;
                }
                else
                {
                    main_time = 0.0f;
                }
            }
           
            ResetAngularVelocity();
        }
        
    }

    internal void killGhostEffectTrue()
    {
        SwordEffectForGhost.SetActive(true);
        canKillGhost = true;
        canTakeThing = false;
        StartCoroutine(killGhostEffectFalse());
    }
    internal IEnumerator killGhostEffectFalse()
    {
        yield return new WaitForSeconds(_ghostEffectTime);
        SwordEffectForGhost.SetActive(false);
        canKillGhost = false;
        yield return new WaitForSeconds(1);
        canTakeThing = true;
    }

    void isNotStun()=> isStan = false;

    private void FixedUpdate()
    {
        if (!IsAnimationPlaying("Death", 0) && !IsAnimationPlaying("ULTIMATE", 0) && !isStan && canDoSmth)
        {
            if (!IsAnimationPlaying("dash", 0))
                RotateFromMouseVector(); //mouse rotate

            if (IsAnimationPlaying("movement", 0))
            {
                _playerRigidbody.AddForce(_movementVector * _movementSpeed * 1000);
            } 
            else if (IsAnimationPlaying("dash", 0))
            {
                SetTriggerUntagged();
                if (!isMove) _movementVector = transform.forward;
                _playerRigidbody.AddForce(_movementVector.normalized * _movementSpeed * 3000);
                _playerAnimator.SetBool("isDash", false);
               
            }
        }
        else if(canDoSmth)
        {
            _playerAnimator.SetFloat("Horizontal", 0);
            _playerAnimator.SetFloat("Vertical", 0);
        }

    }

    private void PushEnemy(float force=0)
    {
        _puchForce = force;
        var spherecoll = Instantiate(sphere, transform.position, quaternion.identity);
        spherecoll.transform.parent = gameObject.transform;
        spherecoll.transform.localPosition = new Vector3(0, 5, 3f);
        spherecoll.tag = "Push";
        spherecoll.GetComponent<SphereCollider>().center= Vector3.zero;
        Destroy(spherecoll, 5f);
    }

    internal void SpawnBomb()
    {
        if (canThrowBomb)
        {
            var forward = transform.forward;
            var bomb = Instantiate(bombPref,
                transform.position + Vector3.up * 3.3F + forward * 10F,
                quaternion.identity);
            // bomb.GetComponent<Rigidbody>().velocity = _playerRigidbody.velocity;
            bomb.direction = (forward + transform.up / 2).normalized;
            bomb.force = 30;
            canThrowBomb = false;
        }
        
    }

    #region SlashPlayMethod
    void PlayFirstSlash()
    {
        _audioSource[1].PlayOneShot(_atkVoiceSound[Random.Range(0, _atkVoiceSound.Length)]);
        _FirstSlash.SetActive(true);
        AudioSource audioSource = _FirstSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.Play();
        //_FirstSlash.GetComponent<ParticleSystem>().Play();
        
        
    }
    void PlaySecondSlash()
    {
        _audioSource[1].PlayOneShot(_atkVoiceSound[Random.Range(0, _atkVoiceSound.Length)]);
        _SecondSlash.SetActive(true);
        AudioSource audioSource = _SecondSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.Play();
        
    }
    void PlayThirdSlash()
    {
        _audioSource[1].PlayOneShot(_atkVoiceSound[Random.Range(0, _atkVoiceSound.Length)]);
        _ThirdSlash.SetActive(true);
        AudioSource audioSource = _ThirdSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.Play();
        

    }
    void Play4Slash()
    {
        _audioSource[1].PlayOneShot(_atkVoiceSound[Random.Range(0, _atkVoiceSound.Length)]);
        _4Slash.SetActive(true);
        AudioSource audioSource = _4Slash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }
    void PlayStrongSlash()
    {
        _StrongSlash.SetActive(true);
        AudioSource audioSource = _StrongSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }
    #endregion
    

    void playStrongVoice()=> _audioSource[1].PlayOneShot(_atkStrongVoiceSound);

    private IEnumerator fireDelayCor()
    {
        StartCoroutine(fireDmg());
        yield return new WaitForSeconds(fireDelay);
        fireEffect.SetActive(false);
        isFire = false;
    }
    private IEnumerator fireDmg()
    {
        while (isFire)
        {
            hud.TakeDamage(1);
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyHit"))
        {
            _audioSource[1].PlayOneShot(_slashHitSound[Random.Range(0, _slashHitSound.Length)]);
            hud.TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        if (other.gameObject.CompareTag("punchHit"))
        {
            _audioSource[0].PlayOneShot(_hitSound[Random.Range(0, _hitSound.Length)]);
            hud.TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        if (other.gameObject.CompareTag("fireHit"))
        {
            _audioSource[0].PlayOneShot(_hitSound[Random.Range(0, _hitSound.Length)]);
            hud.TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            if (!isFire)
            {
                fireEffect.SetActive(true);
                isFire = true;
                StartCoroutine(fireDelayCor());
            }
            
            
        }

        if (other.gameObject.CompareTag("EnemySmoke"))
        {
            _playerAnimator.SetBool("isDash", false);
            _inSmoke = true;
            _movementVector = Vector3.zero;
            _playerAnimator.SetFloat("Horizontal", 0);
            _playerAnimator.SetFloat("Vertical", 0);

        }
        if (other.gameObject.CompareTag("StunHit"))
        {
            _audioSource[0].PlayOneShot(_hitSound[Random.Range(0, _hitSound.Length)]);
            _playerAnimator.SetBool("isDash", false);
            _playerAnimator.SetTrigger("isStun");
            hud.TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            isStan = true;
        }

    }


    public bool IsAnimationPlaying(string animationName, int index)
    {
        // берем информацию о состоянии
        var animatorStateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(index);
        // смотрим, есть ли в нем имя какой-то анимации, то возвращаем true
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    } //Check animation state

    private Vector3 CalculateMovementVector()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerR = transform.right;
        Vector3 playerF = transform.forward;
        playerR.y = 0;
        playerF.y = 0;

        Vector3 movementVector = playerF.normalized * v + playerR.normalized * h;//cameraF.normalized* v +cameraR.normalized * h;
        movementVector = Vector3.ClampMagnitude(movementVector, 1);

        if (Input.GetKey(KeyCode.LeftShift) && StaminaScript.singleton.CanRunning 
            && !IsAnimationPlaying("FirstSlash",0)
            && !IsAnimationPlaying("SecondSlash", 0)
            && !IsAnimationPlaying("ThirdSlash", 0)
            && !IsAnimationPlaying("4Slash", 0)
            && !IsAnimationPlaying("Strong", 0))
        {

            _playerAnimator.SetBool("isRunning", true);
            movementVector *= _runningSpeed;
        }
        else { _playerAnimator.SetBool("isRunning", false); }

        Vector3 relativeVector = transform.InverseTransformDirection(movementVector);



        _playerAnimator.SetFloat("Horizontal", relativeVector.x, 1 / _movingSens, Time.fixedDeltaTime);
        _playerAnimator.SetFloat("Vertical", relativeVector.z, 1 / _movingSens, Time.fixedDeltaTime);


        if (v < 0 || h != 0 && v == 0 || Math.Abs(Mathf.Abs(h) - Mathf.Abs(v)) < 0.00001d)
        {
            movementVector *= 0.8f;
        }


        return movementVector;
    }

    private void ResetAngularVelocity()
    {
        _playerRigidbody.angularVelocity = Vector3.zero;
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    #region MouseRotate

    private void OnMouseEnter()
    {
        isPlayer = true;
    }

    private void OnMouseExit()
    {
        isPlayer = false;
    }

    private void RotateFromMouseVector()
    {
        
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        var camT = _camera.transform;
        var mousePos = camT.position + ray.direction * (transform.position - camT.position).magnitude;
        mousePos.y = transform.position.y;
        if (isPlayer) return;
        direction = mousePos - transform.position;
        rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    #endregion
}