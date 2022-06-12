using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BigOrkBoss : MonoBehaviour
{
    private Rigidbody _rb;
    private CameraMove shake;
    private Transform home;
    private secondPortal spawner;
    private Canvas canvas;
    private Slider healthSlider;
    private GameObject _target;
    internal Animator _animator;
    private NavMeshAgent _agent;
    private PlayerControll _playerControl;

    private Collider _collider;

    [Header("Health and DMG")]
    [SerializeField]
    private float health;
    
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float dmg;


    [Header("Sounds")]
    AudioSource audioSource;
    [SerializeField]
    AudioClip[] slashsounds;

    [SerializeField]
    AudioClip[] hitSounds;

    [SerializeField]
    AudioClip roaringSounds;


    [Header("Particle")]
    [SerializeField]
    GameObject kickParticle;

    [SerializeField]
    internal GameObject dashParticle;

    [SerializeField]
    GameObject groundAtackParticle;
    GameObject groundAtkP;

    [SerializeField]
    GameObject blood;
    
    goblinBossLocation _goblinBossLocation;


    #region bool
    bool isDoKick = false;
    bool isDoGroundAtk = false;
    internal bool isRush = false;
    internal bool canRush = false;

    internal bool isJump = false;
    internal bool canJump = false;
    private bool canAddForce = false;

    bool can = true;
    bool canRun = true;
    bool canKick = true;

    bool nearOther = false;
    bool inSmoke = false;

    internal bool isFirstState = false;
    internal bool firstStateStart = false;
    internal bool secondStateStart = false;


    bool isHome = false;

    internal bool allDie = false;
    #endregion




    [Header("distance")]
    [SerializeField]
    float vewDist;

    [SerializeField]
    float kickDist;

    [SerializeField]
    float groundAtackDist;

    bool startDoing = true;

    [Header("delay")]
    [SerializeField]
    internal float atkDelay;
    internal float curAtkDelay;

    [SerializeField]
    float dashDelay;

    [SerializeField]
    float jumpDelay;

    float RotationSpeed;
    void Start()
    {
        _goblinBossLocation = GameObject.Find("goblinBossLocation").GetComponent<goblinBossLocation>();

        _collider = GetComponent<Collider>();

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
                        if (!isRush && !isJump)
                            RotateToTarget();
                        
                        
                        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
                        
                        if ((distance < vewDist && 
                            distance > kickDist 
                            && distance > groundAtackDist
                            && canRun|| isRush || isJump)
                            && !IsAnimationPlaying("kick", 0) 
                            && !IsAnimationPlaying("groundAtack", 0) )
                        {
                            if (canAddForce)
                                _rb.AddForce(transform.forward * 90000 * Time.deltaTime, ForceMode.Acceleration);
                            if (canJump)
                            {
                                
                                if (!isJump)
                                {
                                    _animator.SetBool("isRun", false);
                                    StartCoroutine(jumpFalse());
                                }
                                isJump = true;
                                _animator.SetTrigger("isJump");
                            }//jumoLogic
                            

                            if (canRush && !canJump) 
                            {
                                _rb.AddForce(transform.forward * 90000 * Time.deltaTime, ForceMode.Acceleration);
                                if (!isRush)
                                {
                                    _animator.SetBool("isRun", false);
                                    StartCoroutine(rushFalse());
                                }
                                isRush = true;
                                _animator.SetBool("isRush", true);
                            } //rushLogic



                            if (!canRush && !canJump && !isJump)
                            {
                                _rb.AddForce(transform.forward * 40000 * Time.deltaTime, ForceMode.Acceleration);
                                _animator.SetBool("isRun", true);
                            }//simpleRunLogic
                            
                            
                        
                        
                        }
                        if (distance < kickDist && !isRush && !isJump)
                        {
                            _animator.SetBool("isRun", false);
                            RotateToTarget();
                            if (!isDoKick && !isRush)
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
                        else if (distance < groundAtackDist && distance > kickDist && !isRush && !isJump)
                        {
                            _animator.SetBool("isRun", false);
                            if (!isDoGroundAtk && !isRush)
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
            else
            {
                _animator.SetBool("isRush", false);
                _animator.SetBool("isRun", false);
                isRush = false;
                isJump = false;
                isDoKick = false;
                isDoGroundAtk=false;
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
            atkDelay = Mathf.Round(atkDelay/1.2f);
            curAtkDelay = atkDelay;
        }
        canvas.transform.LookAt(canvas.worldCamera.transform);
    }

    void roaringTrue() => spawner.roaring = true;
    void canAddForceTrue() => canAddForce = true;
    void canAddForceFalse() => canAddForce = false;

    void playRoaringSound()
    {
        audioSource.volume = 0.5f;
        audioSource.pitch = 1.5f;
        audioSource.PlayOneShot(roaringSounds);
    }
    void StartGroundAtk() => StartCoroutine(groundAtack());
    void StartJumpAtk() => StartCoroutine(jumpAtack());

    private IEnumerator rushFalse()
    {
        
        dashParticle.SetActive(true);
        yield return new WaitForSeconds(2);
        isRush = false;
        canRush = false;
        dashParticle.SetActive(false);
        _animator.SetBool("isRush", false);
        yield return new WaitForSeconds(dashDelay);
        
        canRush = true;
    }
    private IEnumerator jumpFalse()
    {
        _collider.isTrigger = true;
        canJump = false;
        yield return new WaitForSeconds(4);
        _collider.isTrigger = false;
        isJump = false;
        yield return new WaitForSeconds(jumpDelay);

        canJump = true;
    }


   

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
            Instantiate(kickParticle, transform.position,transform.rotation);
            shake.Shake();
            DoStunHit(new Vector3(0, 5f, 6f), 20);
            can = false;
        }
        _playerControl._playerRigidbody.AddForce(transform.forward * Time.deltaTime * 20000, ForceMode.Impulse);
        canKick = false;
        yield return new WaitForSeconds(1);
        _agent.tag = "Untagged";


    }
    private IEnumerator kickDelay()
    {
        yield return new WaitForSeconds(atkDelay);

        isDoKick = false;
        isDoGroundAtk = false;
        can = true;
    }
    
    private IEnumerator jumpAtack()
    {

        for (float i = 0; i < 10; i++)
            Instantiate(kickParticle, transform.position, Quaternion.Euler(new Vector3(0, i * 500)));
        
        Instantiate(kickParticle, transform.position, transform.rotation);
        shake.Shake(); 
        yield return new WaitForSeconds(0.2f);
        DoStunHit(Vector3.zero, 25);
        yield return new WaitForSeconds(0.5f);
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

    private IEnumerator outSmoke(float delay)
    {
        yield return new WaitForSeconds(delay);
        inSmoke = false;
    }

    void DoGroundHit(Vector3 center)
    {
        var sphereCollider = groundAtkP.AddComponent<SphereCollider>();
        sphereCollider.radius = 3f;
        sphereCollider.isTrigger = true;     
        sphereCollider.center = center;
        sphereCollider.tag = "punchHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg/2;
        Destroy(sphereCollider, 0.2f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.2f);
    }
    void DoStunHit( Vector3 center, float r)
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = r;
        sphereCollider.center = center;
        sphereCollider.tag = "StunHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.6f);
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
        _goblinBossLocation.disablevillageWall();
        Destroy(gameObject,0.4f);
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

            
            float t = GameObject.Find("FX_Grenade_Smoke_01(Clone)").GetComponent<smokeTimer>()._smokeTime;
            if (15f - t > 0)
                StartCoroutine(outSmoke(15f - t));
            else inSmoke = false;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            nearOther = true;
        }

    }

    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Smoke"))
        {
            inSmoke = true;
        }

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

}
