using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class spearEnemy : MonoBehaviour
{
    GameObject _target;
    Camera _camera;

    [SerializeField]
    GameObject _stayPositions;
    
    Transform[] _stayPosTransforms;  
    Transform _home;

    CapsuleCollider _myColider;
    
    PlayerControll _playerControl;
    NavMeshAgent _agent;
    
    Animator _playerAnimator;
    Animator _animator;

    AudioSource[] _audioSource;

    bool nearOther = false;

    private float RotationSpeed;
    private Vector3 _force;

    bool cantDo = false;
    bool iSee = false;
    bool isTired = false;
    bool isStunAtack = false;
    bool isHome = false;

    float health;
    private Canvas canvas;
    private Slider healthSlider;
    [SerializeField]
    float maxHealth;
    
    Vector3 playerPos;

    float distance;

    [SerializeField]
    float vewDistance;

    [SerializeField]
    float atackDistance;

    [SerializeField]
    float goBackDistance;

    [SerializeField]
    float dmg;  

    [SerializeField]
    float stayTime;
    [SerializeField]
    float tiredTime;


    float startSpeed;
    float startVewTime = 0;
    float startTiredTime = 0;

    void Start()
    {
        _camera = Camera.main;
        _audioSource = GetComponents<AudioSource>();
        _myColider = GetComponent<CapsuleCollider>();
        _agent = (NavMeshAgent)this.GetComponent("NavMeshAgent");
        _target = GameObject.Find("Player");
        _playerAnimator = _target.GetComponent<Animator>();
        _playerControl = _target.GetComponent<PlayerControll>();
        _animator = GetComponent<Animator>();
        RotationSpeed = _agent.angularSpeed/2;
        startSpeed = _agent.speed;
        _stayPosTransforms = _stayPositions.GetComponentsInChildren<Transform>();

        #region health
        health = maxHealth;

        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        canvas.worldCamera = _camera;
        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion

    }
    void FixedUpdate()
    {
        if (!nearOther)
        {
            if (!IsAnimationPlaying("react", 0) && !IsAnimationPlaying("reactStrong", 0) && !cantDo)
            {
                if (!iSee)
                {
                    distance = Vector3.Distance(_agent.transform.position, _target.transform.position);

                    if (distance <= vewDistance && !isTired)
                    {
                        iSee = true;
                        startVewTime = Time.time;
                        isStunAtack = false;
                        isHome = false;
                        _animator.SetBool("isHome", isHome);
                    }
                    if (isTired)
                    {


                        if (Vector3.Distance(_agent.transform.position, _home.position) <= goBackDistance)
                        {
                            if (Vector3.Distance(_agent.transform.position, _home.position) <= atackDistance)
                            {
                                isHome = true;
                                _animator.SetBool("isHome", isHome);
                                _animator.SetBool("isRunForward", false);
                                RotateToTarget();
                            }

                            if (isHome && isTired && distance <= atackDistance)
                            {
                                _animator.SetTrigger("isStab");
                            }


                            if (Time.time - startTiredTime >= tiredTime)
                            {
                                isTired = false;

                            }

                        }
                        if (!IsAnimationPlaying("Stab", 0) && !IsAnimationPlaying("atack", 0) && Time.time - startTiredTime >= 3f && !isHome)
                        {
                            _agent.speed = 0;
                            if (Vector3.Distance(_target.transform.position, _home.position) >= goBackDistance)
                            {

                                if (distance <= atackDistance && !isStunAtack && Time.time - startTiredTime >= 4f)
                                {
                                    _agent.speed = 0;
                                    _animator.SetBool("isRunForward", false);
                                    RotateToTarget();
                                    _animator.SetBool("isAtack", true);


                                }
                                else if (!IsAnimationPlaying("atack", 0))
                                {
                                    _agent.speed = startSpeed;
                                    _agent.SetDestination(_home.position);
                                    _animator.SetBool("isRunForward", true);

                                }
                            }
                            else
                            {
                                _home = _stayPosTransforms[Random.Range(0, _stayPosTransforms.Length)];
                                RotateToTarget();
                            }
                        }
                    }


                }
                else
                {
                    distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
                    if (Time.time - startVewTime >= stayTime)
                    {
                        _agent.speed = startSpeed;
                        _agent.SetDestination(playerPos);
                        //_agent.transform.position += transform.forward * moveSpeed * Time.deltaTime;

                        float distToDistanation = Vector3.Distance(_agent.transform.position, playerPos);
                        _animator.SetBool("isRunForward", true);
                        if (distToDistanation <= atackDistance)
                        {
                            _agent.speed = 0;
                            iSee = false;
                            isTired = true;
                            startTiredTime = Time.time;
                            _home = _stayPosTransforms[Random.Range(0, _stayPosTransforms.Length)];
                            _animator.SetBool("isRunForward", false);
                        }
                    }
                    else
                    {
                        _agent.speed = 0;
                        RotateToTarget();
                        playerPos = _target.transform.position;


                    }

                }
            }
        }
        else
        {
            StartCoroutine(changeDistanation());
        }


        canvas.transform.LookAt(canvas.worldCamera.transform);




    }
    private IEnumerator changeDistanation()
    {
        _animator.SetBool("isRunForward", true);
        _agent.SetDestination(_target.transform.position + new Vector3(100, 0, 0));
        yield return new WaitForSeconds(2);
        nearOther = false;
    }

    void canDo()
    {
        cantDo = false;
        _agent.speed = startSpeed;
    }
    private void TakeDamage(float? dmg)
    {
        cantDo = true;
        _agent.speed = 0;
        if (IsAnimationPlayerPlaying("Strong", 0))
        {
            _animator.SetTrigger("strongReact");

        }
        else
        {
            _animator.SetTrigger("react");
        }
        int soundNumber = Random.Range(0, 20);
        if (soundNumber <= 10) soundNumber = 0;
        if (soundNumber > 10) soundNumber = 1;
        _audioSource[soundNumber].pitch = Random.Range(0.7f, 1.2f);
        _audioSource[soundNumber].Play();

        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;

        if (health == 0) Kill();
        healthSlider.value = health;

        _myColider.tag = "Untagged";
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

    public bool IsAnimationPlaying(string animationName, int index)
    {
        // берем информацию о состоянии
        var animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(index);
        // смотрим, есть ли в нем имя какой-то анимации, то возвращаем true
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    }
    public bool IsAnimationPlayerPlaying(string animationName, int index)
    {
        var animatorStateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(index);
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
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
            RotationSpeed * Time.deltaTime * 4.0f
            );
    }
    void CkeckAtack()
    {
        _myColider.tag = "Enemy";
    }


    void DoStunHit()
    {
        isStunAtack = true;
        _animator.SetBool("isAtack", false);


        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 6f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "StunHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 7f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "EnemyHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            //_playerControl._weponColider.tag = "Untagged";


        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            nearOther = true;
        }


        if (other.gameObject.CompareTag("Push"))
        {
            var control = other.gameObject.transform.parent.gameObject.GetComponent<PlayerControll>();
            var direction = transform.position - control.transform.position;
            direction.y = 0;
            Debug.Log(direction);
            StartCoroutine(Push(direction.normalized * control._puchForce));
        }

    }
    private IEnumerator Push(Vector3 force)
    {
        _force = force.normalized;
        yield return new WaitForSeconds(force.magnitude / 30f);
        _force.x = 0;
        _force.z = 0;
        _force.y = 0;
    }
}
