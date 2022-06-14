using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class simpleGhost : MonoBehaviour
{
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
    [SerializeField]
    AudioClip[] whoosh;

    [SerializeField]
    GameObject AttackParticle;
    

    Transform home;
    [SerializeField]
    Transform firstPos;
    [SerializeField]
    Transform secondPos;

    CapsuleCollider _myColider;

    bool isStartDoing = true;

    bool isAtack = false;

    bool canAtack = true;
    bool canReact = true;
    bool _isSee = false;

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

    float RotationSpeed;

    bool seeSoundPlay = false;

    private PlayerControll playerControll;

    void Start()
    {
        home = secondPos;

        _myColider = GetComponent<CapsuleCollider>();
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("Player");
        playerControll = _target.GetComponent<PlayerControll>();
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

            if (!IsAnimationPlayerPlaying("Death", 0))
            {
                float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
                if (distance < vewDistance && !_isSee) _isSee = true;
                if (_isSee)
                {
                    if (!seeSoundPlay)
                    {
                        _audioSource[0].PlayOneShot(seeGrowlClips[Random.Range(0, seeGrowlClips.Length)]);
                        StartCoroutine(seeSoundDelay());
                        seeSoundPlay = true;
                    }
                    RotateToTarget();
                    if (!isAtack)
                    {

                        if (distance > atackDistance)
                        {
                            _animator.SetBool("isMove", true);
                        }

                        if (distance <= atackDistance)
                        {
                            _animator.SetBool("isMove", false);
                            _animator.SetTrigger("isAttack");
                            isAtack = true;
                        }
                    }
                    else
                    {
                        if (canAtack)
                        {
                            canAtack = false;
                            StartCoroutine(atackDelay());
                        }

                    }

                }
                else
                {
                    float distanceToHome = Vector3.Distance(_agent.transform.position, home.transform.position);
                    RotateToHome();
                    _animator.SetBool("isMove", true);
                    if (distanceToHome < atackDistance)
                    {
                        if (home.position == firstPos.position) home = secondPos;
                        else if (home.position == secondPos.position) home = firstPos;
                    }
                }
            }


        }


        canvas.transform.LookAt(canvas.worldCamera.transform);
    }


    private IEnumerator seeSoundDelay()
    {
        yield return new WaitForSeconds(Random.Range(8, 14));
        seeSoundPlay = false;
    }

    private IEnumerator startDoing()
    {
        _animator.SetBool("isMove", true);
        yield return new WaitForSeconds(3);
        _animator.SetBool("isMove", false);
        isStartDoing = false;
    }

    private IEnumerator atackDelay()
    {

        yield return new WaitForSeconds(stayTime);
        isAtack = false;
        canAtack = true;
    }
    private IEnumerator reactDelay()
    {
        yield return new WaitForSeconds(4f);
        canReact = true;
    }

    void AtkEffect()
    {
        _audioSource[0].PlayOneShot(attackGrowlClips[Random.Range(0, attackGrowlClips.Length)]);
        _audioSource[1].PlayOneShot(whoosh[Random.Range(0, whoosh.Length)]);
        //AttackParticle.SetActive(true);
    }
    void CheckAttack()
    {
        //AttackParticle.SetActive(false);
        _myColider.tag = "Enemy";
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 2f;
        sphereCollider.center = new Vector3(0, 5f, 2f);
        sphereCollider.tag = "punchHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);
    }


    private void RotateToTarget()
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
    private void RotateToHome()
    {
        Vector3 lookVector;
        lookVector = home.transform.position - _agent.transform.position;
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
                _audioSource[0].PlayOneShot(strongHurtlClips[Random.Range(0, strongHurtlClips.Length)]);
                //_animator.SetTrigger("strongReact");

            }
            else
            {
                _audioSource[0].PlayOneShot(hurtlClips[Random.Range(0, hurtlClips.Length)]);
                //_animator.SetTrigger("react");
            }
        }


        int soundNumber = Random.Range(0, 20);
        if (soundNumber <= 10) soundNumber = 0;
        if (soundNumber > 10) soundNumber = 1;
        _audioSource[1].pitch = Random.Range(0.7f, 1.2f);
        _audioSource[1].PlayOneShot(slashClips[soundNumber]);

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
            if (playerControll.canKillGhost)
                TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
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
