using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Dead,
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Fist,
    }

    public float runSpeed;
    public float distanceFromEnemy;
    public Character target;
    public Weapon weapon;
    public float damage;
    private AudioPlay _audioPlay;
    private Animator _animator;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private State _state = State.Idle;
    private static readonly int Speed = Animator.StringToHash("speed");

    private void Start()
    {
        _audioPlay = GetComponentInChildren<AudioPlay>();
        _animator = GetComponentInChildren<Animator>();
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    public void AttackEnemy()
    {
        if (_state != State.Idle || target._state == State.Dead)
            return;

        switch (weapon)
        {
            case Weapon.Bat:
            case Weapon.Fist:
                _state = State.RunningToEnemy;
                break;

            case Weapon.Pistol:
                _state = State.BeginShoot;
                break;
        }
    }

    public bool IsIdle()
    {
        return _state == State.Idle;
    }

    public bool IsDead()
    {
        return _state == State.Dead;
    }

    public void SetState(State newState)
    {
        _state = newState;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_state)
        {
            case State.Idle:
                _animator.SetFloat(Speed, 0.0f);
                transform.rotation = _originalRotation;
                break;

            case State.RunningToEnemy:
                _animator.SetFloat(Speed, runSpeed);
                if (RunTowards(target.transform.position, distanceFromEnemy))
                    _state = State.BeginAttack;
                break;

            case State.RunningFromEnemy:
                _animator.SetFloat(Speed, runSpeed);
                if (RunTowards(_originalPosition, 0.0f))
                    _state = State.Idle;
                break;

            case State.BeginAttack:
                _animator.SetFloat(Speed, 0.0f);
                switch (weapon)
                {
                    case Weapon.Bat:
                        _animator.SetTrigger("attack");
                        break;
                    case Weapon.Fist:
                        _animator.SetTrigger("fistAttack");
                        break;
                }

                _state = State.Attack;
                break;

            case State.Attack:
                _animator.SetFloat(Speed, 0.0f);
                break;

            case State.BeginShoot:
                _animator.SetFloat(Speed, 0.0f);
                _animator.SetTrigger("shoot");
                _state = State.Shoot;
                break;

            case State.Shoot:
                _animator.SetFloat(Speed, 0.0f);
                break;

            case State.Dead:
                break;
        }
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 vector = direction * runSpeed;
        if (vector.magnitude < distance.magnitude)
        {
            transform.position += vector;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    public void Die()
    {
        _animator.SetTrigger("died");
        SetState(State.Dead);
    }

    public void DoDamageToTarget()
    {
        PlayHitSound();
        target.GetDamageFromEnemy(damage);
    }

    private void PlayHitSound()
    {
        var audioName = "";
        switch (weapon)
        {
            case Weapon.Bat:
                audioName = SoundNames.BatHit;
                break;
            case Weapon.Fist:
                audioName = SoundNames.HandHit;
                break;
            case Weapon.Pistol:
                audioName = SoundNames.GunHit;
                break;
        }

        _audioPlay.Play(audioName);
    }

    private void GetDamageFromEnemy(float receivedDamage)
    {
        GetComponent<HitEffectAnimation>().PlayEffect();
        var health = GetComponent<Health>();
        if (health != null)
        {
            health.ApplyDamage(receivedDamage);
            if (health.current > 0.0f)
            {
                _audioPlay.Play(SoundNames.TakeDamage);
            }
            else
            {
                Die();
                _audioPlay.Play(SoundNames.Die);
            }
        }   
    }
}