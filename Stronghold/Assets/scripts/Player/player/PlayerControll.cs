using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour

    
{
    internal Animator _playerAnimator;

    private Rigidbody _playerRigidbody;

    [SerializeField]
    private HUDBarScript hud;

    [SerializeField]
    internal Collider _weponColider;
   


    #region ForMovement

    private Vector3 _movementVector;

    private Vector3 direction;

    private Quaternion rotation;

    private bool isPlayer;


    #endregion

    #region ForAttack

    [SerializeField]
    internal bool canClick = true;

    internal int noOfClick = 0;
    
    internal bool isUlting;

    private bool ultRegenerate;

    private bool mouseDown = false;    

    private float main_time;

    public float click_time;

    private float bool_time = 0.3f;

    private void DoHit(float dmg = 0)
    { 
        
        _weponColider.GetComponent<DamageProperty>().Damage = dmg;
        
    }

    private void SetTriggerHit()
    {
        _weponColider.tag = "Hit";
    }
    private void SetTriggerUntagged()
    {
        _weponColider.tag = "Untagged";
    }

    #endregion

    #region Serialized
    [Header("Effects")]
    
    [SerializeField]
    GameObject blood;
    
    [SerializeField]
    ParticleSystem _FirstSlash;
    
    [SerializeField]
    ParticleSystem _SecondSlash;
    
    [SerializeField]
    ParticleSystem _ThirdSlash;
    
    [SerializeField]
    ParticleSystem _StrongSlash;
    
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

    #endregion

    void Awake()
    {
        _weponColider.gameObject.AddComponent<DamageProperty>();
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
        if (!IsAnimationPlaying("Death", 0) && !IsAnimationPlaying("ULTIMATE", 0))
        {
            _movementVector = CalculateMovementVector();
            //ultimate
            if (Input.GetKeyDown(KeyCode.Q) && !isUlting && !ultRegenerate)
            {
                StartCoroutine(UltCooldown(_ultTime, _ultRegenerateTime));
                _playerAnimator.SetTrigger("isUlt");
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
                    noOfClick = 4;
                    _playerAnimator.SetInteger("isAttackPhase", 4);
                    mouseDown = true;
                    DoHit(_strongAttackDamage);

                }
            } //atack
            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
                if (Time.time - main_time < bool_time)//fast click
                {
                    ComboStarter();
                    main_time = 0.0f;
                }
                else
                {
                    main_time = 0.0f;
                }
            }
            ResetAngularVelocity();
        }
        
    }
    #region SlashPlayMethod
    void PlayFirstSlash()
    {
        _FirstSlash.Play();
    }
    void PlaySecondSlash()
    {
        _SecondSlash.Play();
    }
    void PlayThirdSlash()
    {
        _ThirdSlash.Play();
    }
    void PlayStrongSlash()
    {
        _StrongSlash.Play();
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyHit"))
        {
            hud.TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
    }

    private void ComboStarter()
    {
        if (canClick)
        {
            noOfClick++;

        }

        if (noOfClick == 1)
        {
            _playerAnimator.SetInteger("isAttackPhase", 1);
        }
    } //First attack

    public void ComboCheck()
    {
        canClick = false;
        if (IsAnimationPlaying("FirstSlash", 0) && noOfClick == 1)
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = true;
            noOfClick = 0;
            _weponColider.tag = "Untagged";
        }
        else if (IsAnimationPlaying("FirstSlash", 0) && noOfClick >= 2)
        {
            _playerAnimator.SetInteger("isAttackPhase", 2);
            canClick = true;
            DoHit(_simpleAttackDamage + 1);
        }
        else if (IsAnimationPlaying("SecondSlash", 0) && noOfClick == 2)
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = true;
            noOfClick = 0;
            _weponColider.tag = "Untagged";
        }
        else if (IsAnimationPlaying("SecondSlash", 0) && noOfClick >= 3)
        {
            _playerAnimator.SetInteger("isAttackPhase", 3);
            canClick = true;
            DoHit(_simpleAttackDamage + 2);
        }
        else if (IsAnimationPlaying("ThirdSlash", 0))
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = true;
            noOfClick = 0;
            _weponColider.tag = "Untagged";
        }
        else if (IsAnimationPlaying("Strong", 0))
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = true;
            noOfClick = 0;
            _weponColider.tag = "Untagged";
        }

        

    }  //Check combo attack

    public bool IsAnimationPlaying(string animationName, int index)
    {
        // берем информацию о состоянии
        var animatorStateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(index);
        // смотрим, есть ли в нем имя какой-то анимации, то возвращаем true
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    } //Check animation state


    private void FixedUpdate()
    {
        if (!IsAnimationPlaying("Death", 0) && !IsAnimationPlaying("ULTIMATE", 0))
        {
            RotateFromMouseVector(); //mouse rotate
            if (IsAnimationPlaying("movement", 0))
                _playerAnimator.transform.position += _movementVector * _movementSpeed / 45;
        }
        
    }


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



        _playerAnimator.SetFloat("Horizontal", relativeVector.x, 1 / _movingSens, Time.deltaTime);
        _playerAnimator.SetFloat("Vertical", relativeVector.z, 1 / _movingSens, Time.deltaTime);


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