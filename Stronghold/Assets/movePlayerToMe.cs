using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class movePlayerToMe : MonoBehaviour
{

    private GameObject _target;
    private PlayerControll _playerControl;
    private NavMeshAgent _agent;
    private Animator _playerAnimator;
    private DialogueTriiger _dialogueTriiger;
    [SerializeField]
    private float tellDistance;
    [SerializeField]
    private float moveDistance;
    [SerializeField]
    private GameObject explosion;

    internal bool _isTell = false;
    private bool _isMoving = false;
    internal bool _isDestroy = false;

    void Start()
    {
        _target = GameObject.Find("Player");
        _agent = _target.GetComponent<NavMeshAgent>();
        _playerAnimator = _target.GetComponent<Animator>();
        _playerControl = _target.GetComponent<PlayerControll>(); 
        _dialogueTriiger = GetComponent<DialogueTriiger>();
    }

    void Update()
    {
        if (!_isTell)
        {
            float distance = Vector3.Distance(_target.transform.position, transform.position);
            if (distance < moveDistance && distance > tellDistance)
            {
                _playerControl.canDoSmth = false;
                RotateToTarget();
                _playerAnimator.SetFloat("Vertical", 1f, 1/ 15, Time.deltaTime);
                _isMoving = true;
            }
            else if (distance < tellDistance)
            {
                _playerAnimator.SetFloat("Vertical", 0, 1 / 15, Time.deltaTime);
                _isMoving = false; 
                _isTell = true;
                _dialogueTriiger.TriggerDialogue();     
            }
        }else if (_isDestroy)
        {
            _playerControl.canDoSmth = true;
            GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
            Destroy(exp, 0.5f);
            _isDestroy = false;
        }
    }

    private void FixedUpdate()
    {
        if(_isMoving)
         _playerControl._playerRigidbody.AddForce(_agent.transform.forward * 10000);
    }
    private void RotateToTarget()
    {
        Vector3 lookVector;
        lookVector = transform.position - _agent.transform.position;
        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        _agent.transform.rotation = Quaternion.RotateTowards
            (
            _agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            _agent.angularSpeed * Time.deltaTime
            );
    }
}
