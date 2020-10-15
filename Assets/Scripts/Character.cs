﻿using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
        Dying,
        Dead,
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Fist,
    }

    Animator animator;
    State state;

    public Weapon weapon;
    public GameObject target;
    private Transform targetTransform;
    public float runSpeed;
    public float distanceFromEnemy;
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        state = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        targetTransform = target.GetComponent<Transform>();
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        if (IsDead()) return;
        switch (weapon) {
            case Weapon.Bat:
            case Weapon.Fist:
                state = State.RunningToEnemy;
                break;
            case Weapon.Pistol:
                state = State.BeginShoot;
                break;
        }
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f) {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude) {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    void FixedUpdate()
    {
        switch (state) {
            case State.Idle:
                transform.rotation = originalRotation;
                animator.SetFloat("Speed", 0.0f);
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(targetTransform.position, distanceFromEnemy))
                    state = State.BeginAttack;
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    state = State.Idle;
                break;

            case State.BeginAttack:
                animator.SetTrigger((weapon == Weapon.Bat) ? "MeleeAttack" : "FistAttack");
                state = State.Attack;
                break;

            case State.Attack:
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                state = State.Shoot;
                break;

            case State.Shoot:
                break;
            
            case State.Dying:
                animator.SetTrigger("Death");
                state = State.Dead;
                break;
        
            case State.Dead:
                break;
        }
    }

    public void HitTarget()
    {
        var targetCharacter = target.GetComponent<Character>();
        if (targetCharacter.IsNotDead()) {
            targetCharacter.SetState(State.Dying);
        }
    }

    private bool IsNotDead()
    {
        return !IsDead();
    }

    private bool IsDead()
    {
        return state == State.Dying || state == State.Dead;
    }
}