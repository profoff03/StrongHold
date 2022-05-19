using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControll : MonoBehaviour
{
    internal Animator _playerAnimator;

    internal Rigidbody _playerRigidbody;

    internal bool isStan;

    [SerializeField]
    private HUDBarScript hud;

    //[SerializeField]
    //internal Collider _weponColider;
    [SerializeField]
    Bomb bombPref;

    internal bool canThrowBomb = true;

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


    private float main_time = 0;

    private float bool_time = 0.3f;

    private void DoHit(float dmg = 0)
    {

        _FirstSlash.GetComponent<DamageProperty>().Damage = dmg;
        _SecondSlash.GetComponent<DamageProperty>().Damage = dmg;
        _ThirdSlash.GetComponent<DamageProperty>().Damage = dmg;
        _4Slash.GetComponent<DamageProperty>().Damage = dmg;
        _StrongSlash.GetComponent<DamageProperty>().Damage = dmg;
        
    }

    //private void SetTriggerHit()
    //{
    //    //gameObject.tag = "Hit";
    //}
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
    GameObject blood;
    
    [SerializeField]
    GameObject _FirstSlash;
    
    [SerializeField]
    GameObject _SecondSlash;
    
    [SerializeField]
    GameObject _ThirdSlash;
    
    [SerializeField]
    GameObject _4Slash;

    [SerializeField]
    GameObject _StrongSlash;
    
    [Header("Ultimate")]
    [SerializeField] internal float _ultTime = 20f;

    [SerializeField] internal float _ultRegenerateTime = 30f;
    
    [Header("Moving")]
    [SerializeField] private Camera _camera;
    
    [SerializeField] private float _movementSpeed = 2f;

    [SerializeField] private float _runningSpeed = 2f;

    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float _movingSens = 15f;

    [Header("AtackDamage")]
    [SerializeField] private float _simpleAttackDamage = 10;

    [SerializeField] private float _strongAttackDamage = 15;
    [SerializeField] public float _puchForce = 5f;

    [SerializeField] GameObject sphere;
    

    #endregion

    void Awake()
    {
        _FirstSlash.AddComponent<DamageProperty>();
        _SecondSlash.AddComponent<DamageProperty>();
        _ThirdSlash.AddComponent<DamageProperty>();
        _4Slash.AddComponent<DamageProperty>();
        _StrongSlash.AddComponent<DamageProperty>();
        
        _playerAnimator = GetComponent<Animator>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
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

        
        if (Input.GetKeyDown(KeyCode.L) && IsAnimationPlaying("Death", 0))
        {
            Time.timeScale = 1f;
            _playerAnimator.SetTrigger("isLive");
            hud.HP = 100f;
        }
        if (!IsAnimationPlaying("Death", 0) && !IsAnimationPlaying("ULTIMATE", 0) && !isStan)
        {
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
                    DoHit(_strongAttackDamage);

                }
            } //atack
            if (Input.GetMouseButtonUp(0)) 
            {
                mouseDown = false;
                if (Time.time - main_time < bool_time)//fast click
                {
                    _playerAnimator.SetTrigger("isAtack");
                    main_time = 0.0f;
                    DoHit(_simpleAttackDamage);
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
    void isNotStun()=> isStan = false;

    private void FixedUpdate()
    {
        if (!IsAnimationPlaying("Death", 0) && !IsAnimationPlaying("ULTIMATE", 0) && !isStan)
        {
            RotateFromMouseVector(); //mouse rotate
            if (IsAnimationPlaying("movement", 0))
                _playerRigidbody.AddForce(_movementVector * _movementSpeed * 1000);
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
        _FirstSlash.SetActive(true);
        AudioSource audioSource = _FirstSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.Play();
        //_FirstSlash.GetComponent<ParticleSystem>().Play();
        
        
    }
    void PlaySecondSlash()
    {
        _SecondSlash.SetActive(true);
        AudioSource audioSource = _SecondSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.Play();
        
    }
    void PlayThirdSlash()
    {
        _ThirdSlash.SetActive(true);
        AudioSource audioSource = _ThirdSlash.GetComponentInParent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.Play();
        

    }
    void Play4Slash()
    {
        _4Slash.SetActive(true);
        AudioSource audioSource = _ThirdSlash.GetComponentInParent<AudioSource>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyHit"))
        {
            hud.TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        if (other.gameObject.CompareTag("StunHit"))
        {
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

        #region Moving Local coordinates

        Vector3 playerR = transform.right;
        Vector3 playerF = transform.forward;
        playerR.y = 0;
        playerF.y = 0;

        #endregion

        #region Moving Global coordinates

        //Vector3 cameraR = _mainCamera.right;
        //Vector3 cameraF = _mainCamera.forward;

        //cameraF.y = 0;
        //cameraR.y = 0;

        #endregion

        Vector3 movementVector = playerF.normalized * v + playerR.normalized * h;//cameraF.normalized* v +cameraR.normalized * h;
        movementVector = Vector3.ClampMagnitude(movementVector, 1);

        if (Input.GetKey(KeyCode.LeftShift) && v > 0 && h == 0 && StaminaScript.singleton.CanRunning )
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
        Time.timeScale = 0f;
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
        #region old

        // изначальный вариант
        // Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        // if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        // {
        //     var target = hitInfo.point;
        //     target.y = transform.position.y;
        //     if (!isPlayer)
        //     {
        //         //transform.LookAt(target);
        //         direction = target - transform.position;
        //         // direction = mousePos - transform.position;
        //         rotation = Quaternion.LookRotation(direction);
        //         transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        //     }
        //}

        #endregion

        //new
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