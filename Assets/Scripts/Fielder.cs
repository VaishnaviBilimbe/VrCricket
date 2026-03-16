using System;
using System.Net.Http.Headers;
using UnityEngine;

public class Fielder : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private Animator animator;

    private Transform targetBall;
    private bool isChasing = false;
    private static readonly int IsRunning = Animator.StringToHash("isChasing");
    private Transform defaultTransform;

    public Transform target;
    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        defaultTransform = transform;
    }

    public void StartChasing(Transform ball)
    {
        targetBall = ball;
        isChasing = true;
        
        if (animator != null)
        {
            // Only set the bool if it's not already true to avoid redundant updates
            if (!animator.GetBool(IsRunning))
            {
                Debug.Log("isrunning chase");
                animator.SetBool(IsRunning, true);
            }
        }
    }

    public void StopChasing()
    {
        isChasing = false;
        if (target != null)
        {
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0; // Keep the rotation level
                           //  if (lookPos != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookPos);
            }
        }

        targetBall = null;
        if (animator != null)
            animator.SetBool(IsRunning, false);
    }
    public void ResetPosotion()
    {
        transform.localPosition=defaultTransform.localPosition;
        Debug.Log("reset position");
    }
    private void Update()
    {
        if (isChasing && targetBall != null)
        {
            Vector3 direction = (targetBall.position - transform.position).normalized;
            direction.y = 0; // Keep fielder on ground
          
            if (Vector3.Distance(transform.position, targetBall.position) > stopDistance)
            {
               // Vector3.MoveTowards(transform.position, targetBall.position, movementSpeed * Time.deltaTime);
                 transform.position += direction * movementSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Fielder touched the ball!");
            StopChasing();
        }
    }
}
