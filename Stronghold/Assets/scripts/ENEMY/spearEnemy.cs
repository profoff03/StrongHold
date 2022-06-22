using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class spearEnemy : MonoBehaviour
{
    GameObject _target;
    NavMeshAgent _agent;
    Animator _playerAnimator;
    Animator _animator;
    
    AudioSource[] _audioSource;
    [SerializeField]
    AudioClip[] audioClips;
    [SerializeField]
    AudioClip[] seeGrowlClips;
    [SerializeField]
    AudioClip[] attackGrowlClips;
    [SerializeField]
    AudioClip[] hurtlClips;
    [SerializeField]
    AudioClip[] strongHurtlClips;
    [SerializeField]
    AudioClip[] dieClips;
    [SerializeField]
    AudioClip[] whoosh;

    [SerializeField]
    GameObject stunAtkParticle;
    [SerializeField]
    GameObject slashParticle;
    [SerializeField]
    GameObject sparkParticle;

    bool seeSoundPlay = false;


    CapsuleCollider _myColider;
    Camera _camera;
    Rigidbody _myRigidbody;
    
    [SerializeField]
    Transform[] homePos;
    Transform home;

    [SerializeField]
    GameObject spearHit;

    public bool isStartDoing = true;

    public bool isAtack = false;
    public bool isStab = false;
    public bool isSlash = false;
    public bool isStunAtk = false;

    public bool inSmoke = false;
    public bool isHome = false;
    public bool findPos = false;

    bool isChangeDistanation = false;

    bool canAtack = true;
    bool canReact = true;
    bool canRun = true;

    bool _isDie = false;

    float health;

    private Canvas canvas;
    private Slider healthSlider;

    [SerializeField]
    float maxHealth;

    [SerializeField]
    float dmg;

    [SerializeField]
    [Range(21f, 100f)]
    float vewDistance;

    [SerializeField]
    float atackDistance;

    [SerializeField]
    float stayTime;

    [SerializeField]
    float slashDelay;

    [SerializeField]
    float dashSpeed;

    private Vector3 _force;

    float rotationSide;

    float RotationSpeed;

    void Start()
    {
        spearHit.GetComponent<DamageProperty>().Damage = dmg;

        _camera = Camera.main;
        _myRigidbody = GetComponent<Rigidbody>();
        _myColider = GetComponent<CapsuleCollider>();
        
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("Player");
        
        _playerAnimator = _target.GetComponent<Animator>();

        RotationSpeed = _agent.angularSpeed / 2;

        _animator = GetComponent<Animator>();
        _audioSource = GetComponents<AudioSource>();
        
        #region health
        health = maxHealth;

        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        canvas.worldCamera = _camera;
        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion


        StartCoroutine(startDoing());
    }
    void Update()
    {
        if (!_isDie)
        {
            transform.position += _force;
            if (!isStartDoing)
            {
                if (!inSmoke)
                {
                    if (!IsAnimationPlayerPlaying("Death", 0))
                    {
                        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
                        if (!seeSoundPlay)
                        {
                            _audioSource[1].PlayOneShot(seeGrowlClips[Random.Range(0, seeGrowlClips.Length)]);
                            StartCoroutine(seeSoundDelay());
                            seeSoundPlay = true;
                        }
                        if (!isAtack)
                        {
                            RotateToTarget(_target.transform);
                            if (distance < vewDistance)
                            {
                                if (distance > atackDistance)
                                {
                                    _animator.SetBool("isRunForward", true);
                                    spearHit.SetActive(true);
                                    _myRigidbody.AddForce(transform.forward * dashSpeed * 1000);
                                }

                                if (distance <= atackDistance)
                                {
                                    spearHit.SetActive(false);
                                    if (!isStab)
                                    {
                                        isStab = true;
                                        _animator.SetTrigger("isStab");
                                        _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
                                    }
                                    _animator.SetBool("isRunForward", false);

                                }
                            }
                        }
                        else if (!isHome)
                        {
                            if (!findPos)
                            {
                                home = homePos[Random.Range(0, homePos.Length)];
                                isStab = false;
                                findPos = true;

                                StartCoroutine(waitRunForAtk());
                            }
                            else
                            {

                                var distanceToHome = Vector3.Distance(_agent.transform.position, home.transform.position);

                                if (distance < atackDistance && !canRun)
                                {
                                    spearHit.SetActive(false);
                                    _animator.SetBool("isRunForward", false);
                                    RotateToTarget(_target.transform);
                                    if (!isStunAtk)
                                    {
                                        isStunAtk = true;
                                        _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
                                        StartCoroutine(StunAtackDelay());
                                    }


                                }
                                else if (canRun)
                                {

                                    RotateToTarget(home);
                                    _animator.SetBool("isRunForward", true);
                                }

                                if (distanceToHome < atackDistance)
                                {
                                    isHome = true;
                                    _animator.SetBool("isRunForward", false);
                                }

                            }

                        }
                        else if (isHome)
                        {
                            spearHit.SetActive(false);
                            RotateToTarget(_target.transform);
                            if (distance <= atackDistance)
                            {
                                if (!isSlash)
                                {
                                    _animator.SetTrigger("isSlash");
                                    _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
                                    StartCoroutine(slashDelayCor());
                                    isSlash = true;
                                }

                            }

                            if (canAtack)
                            {
                                StartCoroutine(atackDelay());
                                canAtack = false;
                            }
                        }


                    }
                }
                else
                {
                    BoolStateFalse();
                    canAtack = true;
                    canRun = true;
                    _animator.SetBool("isRunForward", false);
                }
            }


            canvas.transform.LookAt(canvas.worldCamera.transform);
        }
    }
    private IEnumerator seeSoundDelay()
    {
        yield return new WaitForSeconds(Random.Range(8, 15));
        seeSoundPlay = false;
    }
    private IEnumerator startDoing()
    {
        _animator.SetBool("isRunForward", true);
        yield return new WaitForSeconds(2);
        _animator.SetBool("isRunForward", false);
        isStartDoing = false;
    }


    private IEnumerator changeDistanation()
    {
        yield return new WaitForSeconds(0.5f);
        isChangeDistanation = false;
    }

    private IEnumerator reactDelay()
    {
        yield return new WaitForSeconds(2.5f);
        canReact = true;
    }

    private IEnumerator StunAtackDelay()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger("isAtack");
        yield return new WaitForSeconds(1.5f);
        canRun = true;
        yield return new WaitForSeconds(0.5f);
        isStunAtk = false;
    }

    private IEnumerator waitRunForAtk()
    {
        yield return new WaitForSeconds(1f);
        spearHit.SetActive(true);
        yield return new WaitForSeconds(0.5f); 
        canRun = false;
    }


    private IEnumerator atackDelay()
    {
        yield return new WaitForSeconds(stayTime);
        BoolStateFalse();
        canRun = true;
        canAtack = true;
    }

    private IEnumerator slashDelayCor()
    {
        yield return new WaitForSeconds(slashDelay);
        isSlash= false;
    }

    private IEnumerator outSmoke(float delay)
    {
        yield return new WaitForSeconds(delay);
        inSmoke = false;
    }

    void stunAtkEffect()
    {
        _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
        _audioSource[0].PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);
        stunAtkParticle.SetActive(true);
    }
    void slashEffect()
    {
        _audioSource[0].PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);
        _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
        slashParticle.SetActive(true);
    }
    void sparkEffect()
    {
        _audioSource[0].PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);
        _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
        sparkParticle.SetActive(true);
    }

    void CkeckAtack()
    {
        stunAtkParticle.SetActive(false);
        sparkParticle.SetActive(false);
        slashParticle.SetActive(false);
        isAtack = true;
        _myColider.tag = "Enemy";
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 9f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "EnemyHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
    }
    void DoStunHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 9f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "StunHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
    }

    private void RotateToTarget(Transform target)
    {
        Vector3 lookVector;

        if (isChangeDistanation)
        {
            lookVector = rotationSide * Camera.main.transform.position - target.position;
        }
        else
            lookVector = target.position - _agent.transform.position;


        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            RotationSpeed * Time.deltaTime
            );
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

    private IEnumerator Kill()
    {     
        _isDie = true;
        _animator.SetTrigger("isDie");
        yield return new WaitForSeconds(0.5f);
        _audioSource[1].PlayOneShot(dieClips[Random.Range(0, dieClips.Length)]);
        yield return new WaitForSeconds(3f);
        _agent.enabled = false;
        _myColider.enabled = false;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void TakeDamage(float? dmg)
    {
        if (!IsAnimationPlaying("Stab", 0) && !IsAnimationPlaying("slash", 0) && !IsAnimationPlaying("Run", 0) && canReact)
        {
            canReact = false;
            StartCoroutine(reactDelay());
            if (IsAnimationPlayerPlaying("Strong", 0))
            {
                _audioSource[1].PlayOneShot(strongHurtlClips[Random.Range(0, strongHurtlClips.Length)]);
                _animator.SetTrigger("strongReact");

            }
            else
            {
                _audioSource[1].PlayOneShot(hurtlClips[Random.Range(0, hurtlClips.Length)]);
                _animator.SetTrigger("react");
            }
        }


        int soundNumber = Random.Range(0, 20);
        if (soundNumber <= 10) soundNumber = 0;
        if (soundNumber > 10) soundNumber = 1;
        _audioSource[0].pitch = Random.Range(0.7f, 1.2f);
        _audioSource[0].PlayOneShot(audioClips[soundNumber]);

        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;

        if (health == 0) StartCoroutine(Kill());
        healthSlider.value = health;

        _myColider.tag = "Enemy";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit") && !_isDie)
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



        if (other.gameObject.CompareTag("Push"))
        {
            var control = other.gameObject.transform.parent.gameObject.GetComponent<PlayerControll>();
            var direction = transform.position - control.transform.position;
            direction.y = 0;
            Debug.Log(direction);
            StartCoroutine(Push(direction.normalized * control._puchForce));
        }


    }
    private void OnTriggerStay(Collider other)
    {
        if (!IsAnimationPlaying("Stab", 0) && !IsAnimationPlaying("slash", 0))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (!isChangeDistanation)
                {
                    StartCoroutine(changeDistanation());
                    rotationSide = Random.Range(-10, 10);
                    if (rotationSide >= 0) rotationSide = 1;
                    else if (rotationSide < 0) rotationSide = -1;
                }
                isChangeDistanation = true;

            }
        }
        if (other.gameObject.CompareTag("Untagged") && isAtack) isHome = true;
    }

    private IEnumerator Push(Vector3 force)
    {
        _force = force.normalized;
        yield return new WaitForSeconds(force.magnitude / 30f);
        _force.x = 0;
        _force.z = 0;
        _force.y = 0;
    }

    private void BoolStateFalse()
    {
        spearHit.SetActive(false);
        isStunAtk = false;
        findPos = false;
        isAtack = false;
        isHome = false;
        isSlash = false;
    }
}

