using UnityEngine;
using System;

public class SoulMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.2f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkingParameterName = "IsWalking";

    private Transform targetPoint;
    private bool isMoving;
    private int walkingParameterHash;

    public bool HasReachedDestination { get; private set; }

    public event Action OnDestinationReached;

    private void Awake()
    {
        walkingParameterHash = Animator.StringToHash(walkingParameterName);

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        SetWalkingState(false);
    }

    public void MoveTo(Transform target)
    {
        targetPoint = target;
        isMoving = targetPoint != null;
        HasReachedDestination = false;

        SetWalkingState(isMoving);
    }



    private void Update()
    {
        if (!isMoving || targetPoint == null)
        {
            return;
        }

        Vector3 direction = targetPoint.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= stoppingDistance)
        {
            ArriveAtDestination();
            return;
        }

        Vector3 moveDirection = direction.normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        SetWalkingState(true);
    }

    public void ArriveAtDestination()
    {
        transform.SetPositionAndRotation(
               targetPoint.position,
               transform.rotation
           );

        isMoving = false;
        HasReachedDestination = true;
        
        SetWalkingState(false);
        OnDestinationReached?.Invoke();
        return;
    }

    private void SetWalkingState(bool isWalking)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(walkingParameterHash, isWalking);
    }
}