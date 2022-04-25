using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBossScrtpy : MonoBehaviour
{
    GameObject _target;
    Animator _animator;
    NavMeshAgent _agent;
    Rigidbody _rb;
    float dashtime;
    [SerializeField]
    float startDashtime;
    [SerializeField]
    float dashSpeed;
    //Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        //transform = GetComponent<Transform>();
        _agent = (NavMeshAgent)this.GetComponent("NavMeshAgent");
        _target = GameObject.Find("Player");
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        dashtime = startDashtime;

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(_agent.transform.position, _target.transform.position);
        if (distance < 40f && distance > 20f)
        {
            _agent.SetDestination(_target.transform.position);
            _agent.velocity = Vector3.zero;
        }
        else if (distance < 20f)
        {

            if (dashtime <= 0)
            {
                dashtime = startDashtime;
                _agent.velocity = Vector3.zero;
            }
            else
            {
                dashtime -= Time.deltaTime;
                _agent.transform.position += _agent.transform.forward * dashSpeed;
            }
            

        }
        
    }
}
