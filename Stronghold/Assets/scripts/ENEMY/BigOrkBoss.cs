using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BigOrkBoss : MonoBehaviour
{
    [SerializeField]
    float health;
    private Canvas canvas;
    private Slider healthSlider;

    secondPortal spawner;

    AudioSource audioSource;
    [SerializeField]
    AudioClip[] slashsounds;

    [SerializeField]
    AudioClip[] hitSounds;

    [SerializeField]
    AudioClip roaringSounds;

    Rigidbody _rb;
    CameraMove shake;

    [SerializeField]
    GameObject kickParticle;

    Transform home;

    [SerializeField]
    GameObject groundAtackParticle;
    GameObject groundAtkP;

    [SerializeField]
    GameObject blood;

    GameObject _target;
   
    Animator _animator;
    NavMeshAgent _agent;
    PlayerControll _playerControl;

    bool isDoKick = false;
    bool isDoGroundAtk = false;

    bool can = true;
    bool canRun = true;
    bool canKick = true;

    bool nearOther = false;
    bool inSmoke = false;

    internal bool isFirstState = false;
    bool firstStateStart = false;
    bool secondStateStart = false;

    bool isHome = false;

    internal bool allDie = false;

    float RotationSpeed;

    [SerializeField]
    float dmg;

    [SerializeField]
    float maxHealth;

    [SerializeField]
    float vewDist;

    [SerializeField]
    float kickDist;

    [SerializeField]
    float groundAtackDist;

    bool startDoing = true;

    [SerializeField]
    internal float atkDelay;
    internal float curAtkDelay;


    void Start()
    {
        spawner = GameObject.Find("secondSpawner").GetComponent<secondPortal>();

        home = GameObject.Find("bossHome").GetComponent<Transform>();

        _agent = (NavMeshAgent)this.GetComponent("NavMeshAgent");
        _target = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _playerControl = _target.GetComponent<PlayerControll>();
        _animator = GetComponent<Animator>();

        shake = Camera.main.GetComponent<CameraMove>();

        audioSource = GetComponent<AudioSource>();
        
        RotationSpeed = _agent.angularSpeed / 2;
        #region health
        health = maxHealth;

        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();
        
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        canvas.worldCamera = Camera.main;
        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion

        StartCoroutine(startDoingCor());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFirstState)
        {
            if (!inSmoke)
            {
                if (!nearOther)
                {
                    if(startDoing) _rb.AddForce(transform.forward * 70000 * Time.deltaTime, ForceMode.Acceleration);
                    
                    if (!startDoing)
                    {
                        RotateToTarget();
                        canvas.transform.LookAt(canvas.worldCamera.transform);
                        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
                        if (distance < vewDist && distance > kickDist && distance > groundAtackDist && canRun && !IsAnimationPlaying("kick", 0) && !IsAnimationPlaying("groundAtack", 0))
                        {
                            RotateToTarget();
                            _rb.AddForce(transform.forward * 30000 * Time.deltaTime, ForceMode.Acceleration);
                            _animator.SetBool("isRun", true);
                        }
                        if (distance < kickDist)
                        {
                            _animator.SetBool("isRun", false);
                            RotateToTarget();
                            if (!isDoKick)
                            {
                                isDoKick = true;
                                canRun = false;
                                canKick = true;

                                _animator.SetTrigger("isKick");

                                StartCoroutine(canRunning());
                                StartCoroutine(kickDelay());

                            }

                            if (canKick) StartCoroutine(kikcAtack());


                        }
                        else if (distance < groundAtackDist && distance > kickDist)
                        {
                            _animator.SetBool("isRun", false);
                            if (!isDoGroundAtk)
                            {
                                canRun = false;
                                isDoGroundAtk = true;

                                _animator.SetTrigger("isGroundAtack");

                                StartCoroutine(canRunning());

                                StartCoroutine(groundAtkDelay());

                            }
                            else RotateToTarget();

                        }
                    }
                }
                else
                {
                    StartCoroutine(changeDistanation());
                }
            }

        }
        else if (isHome)
        {
            #region MainHomeLogic
            float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
            _rb.AddForce(Vector3.zero);
            RotateToTarget();

            atkDelay = 2f;

            
            if (distance < kickDist)
            {

                if (!isDoKick)
                {
                    isDoKick = true;
                    canRun = false;
                    canKick = true;

                    _animator.SetTrigger("isKick");

                    StartCoroutine(kickDelay());

                }

                if (canKick) StartCoroutine(kikcAtack());


            }
            #endregion

        }
        else if (!isHome)
        {
            
            RotateToHome();
            _rb.AddForce(transform.forward * 70000 * Time.deltaTime, ForceMode.Acceleration);
            _animator.SetBool("isRun", true);
            if (Vector3.Distance(transform.position, home.position) <= 10f) 
            {
                _animator.SetBool("isRun", false);
                _animator.SetTrigger("isRoaring");
                
                isHome = true;
            } 
        }

        if (health <= 500 && !secondStateStart)
        {
            firstStateStart = false;
            secondStateStart = true;
        }
        

        if (!firstStateStart && !isFirstState && health <= 1000)
        {
            isHome = false;
            Debug.Log("firstState");
            isFirstState = true;
            firstStateStart = true;
            atkDelay = Mathf.Round(atkDelay/1.5f);
            curAtkDelay = atkDelay;
        }
    
    }

    void roaringTrue() => spawner.roaring = true;

    void playRoaringSound()
    {
        audioSource.volume = 0.5f;
        audioSource.pitch = 1.5f;
        audioSource.PlayOneShot(roaringSounds);
    }
   
    
    public bool IsAnimationPlaying(string animationName, int index)
    {
        // ����� ���������� � ���������
        var animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(index);
        // �������, ���� �� � ��� ��� �����-�� ��������, �� ���������� true
        if (animatorStateInfo.IsName(animationName))
            return true;

        return false;
    }

    void StartGroundAtk() => StartCoroutine(groundAtack());

    private IEnumerator startDoingCor()
    {

        _animator.SetBool("isRun", true);
        yield return new WaitForSeconds(2);
        _animator.SetBool("isRun", false);
        _rb.AddForce(Vector3.zero);
        startDoing = false;
        
        
    }

    private IEnumerator canRunning()
    {
        yield return new WaitForSeconds(atkDelay*1.4f);
        canRun = true;
    }

    private IEnumerator kickDelay()
    {
        yield return new WaitForSeconds(atkDelay);

        isDoKick = false;
        isDoGroundAtk = false;
        can = true;
    }

    private IEnumerator groundAtkDelay()
    {
        yield return new WaitForSeconds(atkDelay);

        isDoGroundAtk = false;
    }
    private IEnumerator groundAtack()
    {


        groundAtkP = Instantiate(groundAtackParticle, transform.position, transform.rotation);
        DoGroundHit(new Vector3(0, 0f, 0f));
        yield return new WaitForSeconds(0.1f);
        DoGroundHit(new Vector3(0, 0f, 3f));
        yield return new WaitForSeconds(0.1f);
        DoGroundHit(new Vector3(0, 0f, 6f));
       
        
        
    }
    private IEnumerator kikcAtack()
    {

        
        
        yield return new WaitForSeconds(0.6f);

        
        if (can)
        {

            Instantiate(kickParticle, transform.position, transform.rotation);
            shake.Shake();
            DoStunHit();
            can = false;
        }
        _playerControl._playerRigidbody.AddForce(_agent.transform.forward * Time.deltaTime * 20000, ForceMode.Impulse);
        canKick = false;
        yield return new WaitForSeconds(1);
        _agent.tag = "Untagged";
        
       
    }

    private IEnumerator changeDistanation()
    {
        _animator.SetBool("isRun", true);
        _agent.SetDestination(_target.transform.position + new Vector3(100, 0, 0));
        yield return new WaitForSeconds(2);
        _animator.SetBool("isRun", false);
        nearOther = false;
    }



    void DoGroundHit(Vector3 center)
    {
        var sphereCollider = groundAtkP.AddComponent<SphereCollider>();
        sphereCollider.radius = 3f;
        sphereCollider.isTrigger = true;     
        sphereCollider.center = center;
        sphereCollider.tag = "EnemyHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg/2;
        Destroy(sphereCollider, 0.2f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.2f);
    }
    void DoStunHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 15f;
        sphereCollider.center = new Vector3(0, 5f, 6f);
        sphereCollider.tag = "StunHit";
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
            RotationSpeed * Time.deltaTime
            );
    }
    private void RotateToHome()
    {
        Vector3 lookVector = home.transform.position - _agent.transform.position;
        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            RotationSpeed * Time.deltaTime * 4.0f
            );
    }
    private void TakeDamage(float? dmg)
    {
        dmg ??= 0;
        
        _agent.speed = 0;
        Instantiate(blood, transform);
        int soundNumber = Random.Range(0, 20);
        if (soundNumber <= 10) soundNumber = 0;
        if (soundNumber > 10) soundNumber = 1;
        audioSource.volume = 1;
        audioSource.pitch = Random.Range(0.7f, 1.2f);
        audioSource.PlayOneShot(slashsounds[soundNumber]);

        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(hitSounds[Random.Range(0,hitSounds.Length)]);


        health -= (float)dmg;
        if (health <= 0.001) health = 0f;

        if (health == 0) Kill();
        healthSlider.value = health;

        _agent.tag = "Untagged";
    }
    private void Kill()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
           
        }
        if (other.gameObject.CompareTag("Smoke"))
        {
            inSmoke = true;
            _animator.SetBool("isRun", false);
            StartCoroutine(outSmoke());
            
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            nearOther = true;
        }

    }

    private IEnumerator outSmoke()
    {
        yield return new WaitForSeconds(15);
        inSmoke = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Smoke"))
        {
            inSmoke = true;
        }

    }
}
