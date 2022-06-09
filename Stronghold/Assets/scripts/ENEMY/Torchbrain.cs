using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class Torchbrain : MonoBehaviour
{
    //true script for ork with axe and with torch
    GameObject _target;
    NavMeshAgent _agent;
    Animator _playerAnimator;
    Animator _animator;
    
    AudioSource[] _audioSource;
    [SerializeField]
    AudioClip[] slashClips;
    [SerializeField]
    AudioClip[] seeGrowlClips;
    [SerializeField]
    AudioClip[] attackGrowlClips;
    [SerializeField]
    AudioClip[] hurtlClips;
    [SerializeField]
    AudioClip[] strongHurtlClips;


    CapsuleCollider _myColider;

    bool isStartDoing = true;

    bool isAtack = false;
    bool inSmoke = false;
    bool isForwardMove = false;

    bool isChangeDistanation = false;

    bool canAtack = true;
    bool canReact = true;

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
    [Range(11f, 30f)]
    float goBackDistance;

    [SerializeField]
    float stayTime;

    private Vector3 _force;

    float rotationSide;

    float RotationSpeed;

    bool seeSoundPlay = false;

    void Start()
    {

        _myColider = GetComponent<CapsuleCollider>();
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("Player");
        _playerAnimator = _target.GetComponent<Animator>();

        RotationSpeed = _agent.angularSpeed / 2;

        _animator = GetComponent<Animator>();
        _audioSource = GetComponents<AudioSource>();
        _audioSource[1].maxDistance = vewDistance;
        #region health
        health = maxHealth;

        canvas = transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        canvas.worldCamera = Camera.main;
        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion


        StartCoroutine(startDoing());
    }
    void Update()
    {
        transform.position += _force;
        if (!isStartDoing)
        {
            if (!inSmoke)
            {
                if (!IsAnimationPlayerPlaying("Death", 0))
                {
                    float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
                    if (distance < vewDistance)
                    {
                        if (!seeSoundPlay)
                        {
                            _audioSource[1].PlayOneShot(seeGrowlClips[Random.Range(0, seeGrowlClips.Length)]);
                            StartCoroutine(seeSoundDelay()); 
                            seeSoundPlay = true;
                        }
                        
                        if (!isAtack)
                        {
                            RotateToTarget();
                            _animator.SetBool("isRunBack", false);
                            _animator.SetBool("isRunLeft", false);

                            if (distance > atackDistance)
                            {
                                _animator.SetBool("isRunForward", true);
                                _animator.SetInteger("AtackPhase", 0);
                            }

                            if (distance <= atackDistance)
                            {
                                _animator.SetBool("isRunForward", false);
                                int r = Random.Range(1, 10);
                                
                                _animator.SetInteger("AtackPhase", r);
                            }
                        }
                        else
                        {
                            RotateToTargetOnly();
                            _animator.SetInteger("AtackPhase", 0);
                            if (canAtack)
                            {
                                canAtack = false;
                                StartCoroutine(atackDelay());
                            }


                            if (distance <= goBackDistance && !IsAnimationPlaying("RunLeft", 0) && !isForwardMove)
                            {
                                _animator.SetBool("isRunForward", false);
                                _animator.SetInteger("AtackPhase", 0);
                                _animator.SetBool("isRunBack", true);
                            }
                            else if (!isForwardMove)
                            {
                                _animator.SetBool("isRunForward", false);
                                _animator.SetInteger("AtackPhase", 0);
                                _animator.SetBool("isRunBack", false);
                                _animator.SetBool("isRunLeft", true);
                            }

                        }

                    }
                }
            }
            else
            {
                isAtack = false;
                canAtack = true;
                _animator.SetInteger("AtackPhase", 0);
                _animator.SetBool("isRunForward", false);
                _animator.SetBool("isRunBack", false);
                _animator.SetBool("isRunLeft", false);
            }
        }


        canvas.transform.LookAt(canvas.worldCamera.transform);
    }


    private IEnumerator seeSoundDelay()
    {
        yield return new WaitForSeconds(Random.Range(8,14));
        seeSoundPlay = false;
    }



    private IEnumerator startDoing()
    {
        _animator.SetBool("isRunForward", true);
        yield return new WaitForSeconds(3);
        _animator.SetBool("isRunForward", false);
        isStartDoing = false;
    }
    
    
    private IEnumerator changeDistanation()
    {
        yield return new WaitForSeconds(0.5f);
        isChangeDistanation = false;
    }


    private IEnumerator forwardRun()
    {

        yield return new WaitForSeconds(0.3f);
        isForwardMove = false;
        _animator.SetBool("isRunForward", false);
        _animator.SetBool("isRunLeft", true);


    }

    private IEnumerator atackDelay()
    {

        yield return new WaitForSeconds(stayTime);
        isAtack = false;
        canAtack = true;
    }
    private IEnumerator reactDelay()
    {
        yield return new WaitForSeconds(2.5f);
        canReact = true;
    }

        private IEnumerator outSmoke(float delay)
    {
        yield return new WaitForSeconds(delay);
        inSmoke = false;
    }


    void CkeckAtack()
    {
        isAtack = true;
        _myColider.tag = "Enemy";
    }

    void DoHit()
    {
        _audioSource[1].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);

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


    private void RotateToTarget()
    {
        Vector3 lookVector;
       
        if (isChangeDistanation
            && !IsAnimationPlaying("SecondAtack", 0) 
            && !IsAnimationPlaying("FirstAtack", 0)) 
        {  
            lookVector = rotationSide * transform.position;
        }
        else 
            lookVector = _target.transform.position - _agent.transform.position;


        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            RotationSpeed * Time.deltaTime
            );
    }
    private void RotateToTargetOnly()
    {
        Vector3 lookVector;
        lookVector = _target.transform.position - _agent.transform.position;
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

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void TakeDamage(float? dmg)
    {
        if (!IsAnimationPlaying("FirstAtack", 0) && !IsAnimationPlaying("SecondAtack", 0) && canReact)
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
        _audioSource[0].PlayOneShot(slashClips[soundNumber]);

        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;

        if (health == 0) Kill();
        healthSlider.value = health;

        _myColider.tag = "Enemy";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            //_playerControl._weponColider.tag = "Untagged";


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
        if (!IsAnimationPlaying("SecondAtack", 0) && !IsAnimationPlaying("FirstAtack", 0))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (!isChangeDistanation)
                {          
                    rotationSide = Random.Range(-10, 10);
                    if (rotationSide >= 0) rotationSide = 1;
                    else if (rotationSide < 0) rotationSide = -1;
                    StartCoroutine(changeDistanation());
                }
                isChangeDistanation = true;

            }
        }
        if (other.gameObject.CompareTag("Untagged") && isAtack)
        {
            
            if (IsAnimationPlaying("RunBack", 0))
            {
                _animator.SetInteger("AtackPhase", 0);
                _animator.SetBool("isRunBack", false);
                _animator.SetBool("isRunLeft", true);
            }
            else if (IsAnimationPlaying("RunLeft", 0))
            {
                isForwardMove = true;
                _animator.SetBool("isRunLeft", false);
                _animator.SetBool("isRunForward", true);
                StartCoroutine(forwardRun());
            }

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