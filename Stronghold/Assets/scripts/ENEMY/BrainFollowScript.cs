using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BrainFollowScript : MonoBehaviour
{
    [Range(0, 360)] public float ViewAngle = 90f;
    public float ViewDistance = 15f;
    public float DetectionDistance = 3.0f;
    public Transform EnemyEye;
    public Transform Target;

    private NavMeshAgent agent;
    private float RotationSpeed;
    private Transform agentTransform;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false; 
        RotationSpeed = agent.angularSpeed;
        agentTransform = agent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(Target.transform.position, agent.transform.position);
        if (distanceToPlayer <= DetectionDistance || IsInView())
        {
            RotateToTarget();
            MoveToTarget();
            animator.SetBool("isRunForward", true);
        }
        else
        {
            animator.SetBool("isRunForward", false);
        }
        DrawViewState();
    }

    private bool IsInView()
    {
        float RealAngle = Vector3.Angle(EnemyEye.forward, Target.position - EnemyEye.position);
        RaycastHit hit;
        if(Physics.Raycast(EnemyEye.transform.position, Target.position - EnemyEye.position,out hit, ViewDistance))
        {
            if(RealAngle < ViewAngle / 2f && Vector3.Distance(EnemyEye.position, Target.position) <= ViewDistance && hit.transform == Target.transform)
            {
                return true;
            }
        }
        return false;
    }
    private void RotateToTarget()
    {
        Vector3 lookVector = Target.position - agentTransform.position;
        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        agentTransform.rotation = Quaternion.RotateTowards
            (
            agent.transform.rotation,
            Quaternion.LookRotation(lookVector, Vector3.up),
            RotationSpeed * Time.deltaTime
            );
    }

    private void MoveToTarget()
    {
        agent.SetDestination(Target.position);
        
    }

    private void DrawViewState()
    {
        Vector3 Left = EnemyEye.position + Quaternion.Euler(new Vector3(0, ViewAngle / 2f, 0)) * (EnemyEye.forward * ViewDistance);
        Vector3 Right = EnemyEye.position + Quaternion.Euler(-new Vector3(0, ViewAngle / 2f, 0)) * (EnemyEye.forward * ViewDistance);
        Debug.DrawLine(EnemyEye.position, Left,  Color.yellow);
        Debug.DrawLine(EnemyEye.position, Right, Color.yellow);

    }
}
