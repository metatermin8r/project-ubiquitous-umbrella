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
    public float Deg;

    private NavMeshAgent Agent;
    private AgentLinkMover LinkMover;

    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private bool canSeePlayer;

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        LinkMover.OnLinkEnd += HandleLinkEnd;
        LinkMover.OnLinkStart += HandleLinkStart;
    }

    public void StartChasing() 
    { 
        if (FollowCoroutine == null)
        {
            FollowCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        } 

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
        {      //this if statement controls whether or not th player is in the enemy's FOV
            Vector3 dir = Target.transform.position - transform.position;
            if(Mathf.Abs(Vector3.Angle(transform.forward, dir))<Deg) 
            {
                Agent.SetDestination(Target.transform.position - (Target.transform.position - transform.position).normalized * 0.5f);
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }

            yield return Wait;
        }
    }

    public void Die()
    {
        Agent.speed = 0f;
    }
}
