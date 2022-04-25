using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllThief : MonoBehaviour
{
    private Animator _playerAnimator;

    private Rigidbody _playerRigidbody;

    #region ForMovement

    private Vector3 _movementVector;

    private Vector3 direction;

    private Quaternion rotation;

    private bool isPlayer;

    #endregion

    #region ForAttack

    private bool canClick = true;

    private int noOfClick = 0;

    private bool mouseDown = false;

    private float main_time;
    
    public float click_time;
    
    private float bool_time = 0.3f;

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 3f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "Hit";
        Destroy(sphereCollider, 0.1f);
    }


    #endregion

    #region Serialized

    [SerializeField] private float _movementSpeed = 2f;

    [SerializeField] private float _runningSpeed = 2f;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _mainCamera;

    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float _movingSens = 15f;

    #endregion


    void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
    }



    void Update()
    {
        _movementVector = CalculateMovementVector();


        if (Input.GetMouseButton(0))
        {
            if (main_time == 0.0f)
            {
                main_time = Time.time;
                
            }
            if (Time.time - main_time > bool_time && !mouseDown)//long press
            {
                if (!IsAnimationPlaying("Dash", 0))//strong atack
                {
                    _playerAnimator.SetInteger("isAttackPhase", 3);
                    noOfClick = 3;
                    mouseDown = true;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            if (Time.time - main_time < bool_time)
            {     
                ComboStarter();
                main_time = 0.0f;
            }
            else
            {
                main_time = 0.0f;
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            _playerAnimator.SetTrigger("Dash");
            _playerAnimator.SetInteger("isAttackPhase", 0);
            noOfClick = 0;
            
        }

        

        ResetAngularVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        Console.WriteLine("enter trigger");
        Console.WriteLine(other);
    }

    private void ComboStarter()
    {
       

        if (IsAnimationPlaying("Dash", 0))
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = false;
            noOfClick = 0;
            DoHit();
        }
        else
        {
            canClick = true;

        }


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
            DoHit();

        }
        else if (IsAnimationPlaying("FirstSlash", 0) && noOfClick >= 2)
        {
            _playerAnimator.SetInteger("isAttackPhase", 2);
            canClick = true;
            DoHit();
        }
        else if (IsAnimationPlaying("SecondSlash", 0) )
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = true;
            noOfClick = 0;
            DoHit();
        }
        else if (IsAnimationPlaying("ThirdSlash", 0))
        {
            _playerAnimator.SetInteger("isAttackPhase", 0);
            canClick = true;
            noOfClick = 0;
            DoHit();
        }
        else
        {
            canClick = true;
            noOfClick = 0;
        }
        
    } //Check combo attack

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
        RotateFromMouseVector(); //mouse rotate
        
        //movement
        if (IsAnimationPlaying("movement", 0))
            _playerAnimator.transform.position += (_movementVector * _movementSpeed) / 45;
    }

    
    private Vector3 CalculateMovementVector()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        #region movment direction   
        if (v > 0)
        {
            _playerAnimator.SetFloat("direct(FB)", 1);
        }
        else if (v < 0)
        {
            _playerAnimator.SetFloat("direct(FB)", -1);
        }
        else
        {
            _playerAnimator.SetFloat("direct(FB)", 0);
        }

        if (h > 0)
        {
            _playerAnimator.SetFloat("direct(RL)", 1);
        }
        else if (h < 0)
        {
            _playerAnimator.SetFloat("direct(RL)", -1);
        }
        else
        {
            _playerAnimator.SetFloat("direct(RL)", 0);
        }
        #endregion

        #region Moving Local coordinates

        Vector3 playerR = transform.right;
        Vector3 playerF = transform.forward;
        playerR.y = 0;
        playerF.y = 0;

        #endregion

        

        Vector3 movementVector =
            playerF.normalized * v + playerR.normalized * h; //cameraF.normalized* v +cameraR.normalized * h;

        movementVector = Vector3.ClampMagnitude(movementVector, 1);

        
        
        if (Input.GetKey(KeyCode.LeftShift) && v > 0 && h == 0 && StaminaScript.singleton)
        {
            movementVector *= _runningSpeed;
        }

        Vector3 relativeVector = transform.InverseTransformDirection(movementVector);


        _playerAnimator.SetFloat("Horizontal", relativeVector.x, 1 / _movingSens, Time.deltaTime);
        _playerAnimator.SetFloat("Vertical", relativeVector.z, 1 / _movingSens, Time.deltaTime);


        if (v < 0 || (h != 0 && v == 0) || (Mathf.Abs(h) == Mathf.Abs(v)))
        {
            movementVector = movementVector * 0.8f;
        }


        return movementVector;
    }

    private void ResetAngularVelocity()
    {
        _playerRigidbody.angularVelocity = Vector3.zero;
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
        if (!isPlayer)
        {
            direction = mousePos - transform.position;
            rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    #endregion
}