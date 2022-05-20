using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BigOrkBoss : MonoBehaviour
{
    float health;
    private Canvas canvas;
    private Slider healthSlider;

    bool can = true;


    CameraMove shake;

    [SerializeField]
    ParticleSystem kickParticle;

    [SerializeField]
    GameObject blood;

    GameObject _target;
   
    Animator _animator;
    NavMeshAgent _agent;
    Animator _playerAnimator;
    PlayerControll _playerControl;

    bool isDoKick = false;

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



    void Start()
    {
        _agent = (NavMeshAgent)this.GetComponent("NavMeshAgent");
        _target = GameObject.Find("Player");
        _playerAnimator = _target.GetComponent<Animator>();
        _playerControl = _target.GetComponent<PlayerControll>();
        _animator = GetComponent<Animator>();

        shake = Camera.main.GetComponent<CameraMove>();

        RotationSpeed = _agent.angularSpeed / 2;
        #region health
        health = maxHealth;

        canvas = _agent.transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = _agent.transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        canvas.transform.rotation = canvas.worldCamera.transform.rotation;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        canvas.transform.LookAt(canvas.worldCamera.transform);
        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
        if (distance < vewDist)
        {
            RotateToTarget();
        }
        if (distance < kickDist)
        {
            RotateToTarget();
            if (!isDoKick)
            {
                _animator.SetTrigger("isKick");
                
            }
                
           
            StartCoroutine(kikcAtack());
            
        }else if(distance < groundAtackDist && distance > kickDist)
        {
            _animator.SetTrigger("isGroundAtack");
        }
    }

    private IEnumerator kikcAtack()
    {


        isDoKick = true;

        yield return new WaitForSeconds(0.6f);
        _playerControl._playerRigidbody.AddForce(_agent.transform.forward * Time.deltaTime * 20000, ForceMode.Impulse);
        
       if (can)
        {
            
            kickParticle.Play();
            shake.Shake();
            DoStunHit();
            can = false;
        }
        //_playerControl.isStan = true;
        yield return new WaitForSeconds(2);
        isDoKick = false;
        can = true;


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
        Destroy(sphereCollider, 0.3f);
        Destroy(sphereCollider.GetComponent<DamageProperty>(), 0.3f);
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
}
