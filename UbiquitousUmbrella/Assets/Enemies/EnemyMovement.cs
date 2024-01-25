using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]

public class EnemyMovement : MonoBehaviour
{
    public Transform Target;
    [SerializeField]
    private Animator Animator;
    public float UpdateSpeed = 0.1f; //how frequrntly to recalculaet path based on Target transform's position


    private NavMeshAgent Agent;
    private AgentLinkMover LinkMover;

    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    public FieldOfView FovScript;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        LinkMover.OnLinkEnd += HandleLinkEnd;
        LinkMover.OnLinkStart += HandleLinkStart;
    }

    private void Start () 
    { 
        StartCoroutine(FollowTarget());
    }

    private void HandleLinkStart()
    {
        Animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        Animator.SetTrigger(Landed);
    }

    private void Update()
    {

        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateSpeed);

        while (enabled)
        {
            //if (FovScript.canSeePlayer)
           // {
            Agent.SetDestination(Target.transform.position - (Target.transform.position - transform.position).normalized * 0.5f);
            
            yield return Wait;
           // }

        }
    }

    public void Die()
    {
        Agent.speed = 0f;
    }
}
