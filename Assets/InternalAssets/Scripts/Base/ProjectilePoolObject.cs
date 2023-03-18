using UnityEngine;
using Spawners;
using System.Collections.Generic;

public class ProjectilePoolObject : PoolObject
{
    public LinkedListNode<ProjectilePoolObject> ProjectileNode { get; private set; }
    
    private IShootingTarget _target;
    private float _projectileMoveSpeed;
    private float _attackDamage;

    private Vector3 _normolizedMoveDirection;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _initialRotation = transform.rotation;
    }

    public void Setup
    (
        LinkedListNode<ProjectilePoolObject> projectileNode,
        ISettingsGetter settings,
        IShootingTarget target,
        float attactDamage
    )
    {
        transform.rotation = _initialRotation;

        ProjectileNode = projectileNode;
        _target = target;
        _projectileMoveSpeed = settings.ProjectileMoveSpeed;
        _attackDamage = attactDamage;

        _normolizedMoveDirection = (_target.GetTargetPosition() - transform.position).normalized;
        enabled = true;
    }

    public void MoveToEnemy()
    {
        Vector3 targetPosition = _target.GetTargetPosition();
        
        if((targetPosition - transform.position).sqrMagnitude < 0.1f)
        {
            _target.TakeDamage(_attackDamage);
            enabled = false;
        }
        else
        {
            transform.position += _normolizedMoveDirection * Time.deltaTime * _projectileMoveSpeed;
        }
    }
}