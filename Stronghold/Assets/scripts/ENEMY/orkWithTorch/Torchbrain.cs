using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class Torchbrain : MonoBehaviour
{
    GameObject _target;
    [SerializeField]
    GameObject blood;
    PlayerControll _playerControl;
    NavMeshAgent _agent;
    Animator _playerAnimator;
    Animator _animator;
    AudioSource[] _audioSource;



    bool isAtack = false;
    Vector3 whereAtackDistance;
    float health;
    private Canvas canvas;
    private Slider healthSlider;


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

    void Start()
    {
        _agent = (NavMeshAgent)this.GetComponent("NavMeshAgent");
        _target = GameObject.Find("Player");
        _playerAnimator = _target.GetComponent<Animator>();
        _playerControl = _target.GetComponent<PlayerControll>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponents<AudioSource>();
        #region health
        health = maxHealth;
        
        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();
        
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion
    }
    void Update()
    {
        
        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
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
            _animator.SetInteger("AtackPhase",0);
        }

        canvas.transform.LookAt(canvas.worldCamera.transform);
    }
    void CkeckAtack()
    {
        isAtack = true;
    }

    void DoHit()
    {
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 3.4f;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            TakeDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            //_playerControl._weponColider.tag = "Untagged";
            

        }

        
    }
	private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Smoke")) Debug.Log("I'm in smoke");
    }
}