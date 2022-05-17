using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class orkWithAxeBrain : MonoBehaviour
{
    GameObject _target;
    NavMeshAgent _agent;
    Animator _playerAnimator;
    Animator _animator;
    AudioSource[] _audioSource;
    CapsuleCollider _myColider;
    Camera _camera;


    bool isAtack = false;
    Vector3 whereAtackDistance;
    float health;
    private Canvas canvas;
    private Slider healthSlider;

    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float maxHealth;
    [SerializeField]
    float dmg;
    [SerializeField]
    float vewDistance;
    [SerializeField]
    float atackDistance;
    [SerializeField]
    [Range(1f,15f)]
    float goBackDistance;
    [SerializeField]
    float stayTime;
    float start = 0;
    private Vector3 _force;
    bool nearOther = false;

    bool inSmoke = false;

    void Start()
    {
        _camera = Camera.main;
        _myColider = GetComponent<CapsuleCollider>();
        _agent = (NavMeshAgent)this.GetComponent("NavMeshAgent");
        _target = GameObject.Find("Player");
        _playerAnimator = _target.GetComponent<Animator>();
       
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
    }
    void Update()
    {
        transform.position += _force;
        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
        if (!inSmoke)
        {
            if (!nearOther)
            {
                if (!IsAnimationPlayerPlaying("Death", 0))
                {
                    if (!isAtack)
                    {
                        if (distance < vewDistance && distance >= atackDistance)
                        {
                            _agent.SetDestination(_target.transform.position);
                            _animator.SetBool("isRunForward", true);

                        }
                        else if (distance < vewDistance && distance < atackDistance)
                        {
                            _agent.velocity = Vector3.zero;
                            _agent.SetDestination(_target.transform.position);
                            _animator.SetBool("isRunForward", false);
                            _animator.SetInteger("AtackPhase", Random.Range(0, 10));
                            whereAtackDistance = _agent.transform.position;
                            start = Time.time;
                        }
                        else
                        {
                            _agent.velocity = Vector3.zero;
                            _animator.SetBool("isRunForward", false);

                        }
                    }
                    else
                    {
                        if (distance > 25f)
                        {
                            isAtack = false;
                            _animator.SetBool("isRunBack", false);
                            _animator.SetBool("isRunLeft", false);
                        }
                        else
                        {
                            float dist = Vector3.Distance(_agent.transform.position, whereAtackDistance);
                            _animator.SetInteger("AtackPhase", 0);
                            if (dist < goBackDistance)
                            {
                                _animator.SetBool("isRunBack", true);
                                isAtack = true;

                            }
                            else if (dist >= goBackDistance)
                            {

                                if (Time.time - start <= stayTime)
                                {

                                    _animator.SetBool("isRunLeft", true);
                                    _agent.SetDestination(_target.transform.position);
                                }
                                else
                                {
                                    _animator.SetBool("isRunBack", false);
                                    _animator.SetBool("isRunLeft", false);
                                    isAtack = false;
                                }

                            }

                        }

                    }

                }
                else
                {
                    _animator.SetInteger("AtackPhase", 0);
                }
            }
            else
            {
                StartCoroutine(changeDistanation());

            }
        }
        
         
        

        canvas.transform.LookAt(canvas.worldCamera.transform);
    }
    void CkeckAtack()
    {
        isAtack = true;
        _myColider.tag = "Enemy";
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 5f;
        sphereCollider.center = new Vector3(0, 5f, 4f);
        sphereCollider.tag = "EnemyHit";
        sphereCollider.gameObject.AddComponent<DamageProperty>();
        sphereCollider.GetComponent<DamageProperty>().Damage = dmg;
        Destroy(sphereCollider, 0.1f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.1f);


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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            //_playerControl._weponColider.tag = "Untagged";
            

        }
        
        
        if (other.gameObject.CompareTag("Push"))
        {
            var control = other.gameObject.transform.parent.gameObject.GetComponent<PlayerControll>();
            var direction = transform.position - control.transform.position;
            direction.y = 0;
            Debug.Log(direction);
            StartCoroutine(Push(direction.normalized * control._puchForce));
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            nearOther = true;
        }

        if (other.gameObject.CompareTag("Smoke"))
        {
            inSmoke = true;
            _animator.SetBool("isRunForward", false);
            _animator.SetBool("isRunBack", false);
            _animator.SetBool("isRunLeft", false);
        }
        else
        {
            inSmoke = false;
        }


    }
    private IEnumerator changeDistanation()
    {
        _animator.SetBool("isRunForward", true);
        _agent.SetDestination(_target.transform.position + new Vector3(100, 0, 0));
        yield return new WaitForSeconds(2);
        nearOther = false;
    }
    
    private IEnumerator Push(Vector3 force)
    {
        _force = force.normalized;
        yield return new WaitForSeconds(force.magnitude / 30f);
        _force.x = 0;
        _force.z = 0;
        _force.y = 0;
    }
	private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Smoke"))
        {
            inSmoke = true;
        }
        
    }
}