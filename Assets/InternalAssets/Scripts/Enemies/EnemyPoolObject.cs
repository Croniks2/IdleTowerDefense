using UnityEngine;

using Spawners;
using EventBusSystem;
using System.Collections.Generic;

public class EnemyPoolObject : PoolObject, IShootingTarget
{
    public bool IsDestroyed { get; set; }

    public LinkedListNode<EnemyPoolObject> EnemyNode { get; private set; }
    private Vector3 _spawnPosition;
    private Vector3 _basePosition;

    private float _destinationTime;
    private float _destinationPercent;
    private float _maxHP;

    private float _percentTraveled = 0f;
    private float _currentHP;
    
    
    public void Setup
    (
        LinkedListNode<EnemyPoolObject> enemyNode,
        Vector3 spawnPosition,
        Vector3 basePosition,
        ISettingsGetter settings
    )
    {
        EnemyNode = enemyNode;
        _spawnPosition = spawnPosition;
        _basePosition = basePosition;
        _destinationTime = settings.DestinationTime;
        _destinationPercent = settings.DestinationPercent;
        _maxHP = _currentHP = settings.EnemyMaxHP;

        _percentTraveled = 0f;
        IsDestroyed = false;
        enabled = true;
    }
    
    public void MoveToBase()
    {
        _percentTraveled += Time.deltaTime / _destinationTime;
        
        if(_percentTraveled < _destinationPercent)
        {
            transform.position = Vector3.Lerp(_spawnPosition, _basePosition, _percentTraveled);
        }
        else
        {
            EventBus.RaiseEvent<IBaseDamageSubscriber>(h => h.HandleBaseDamage(0.05f));
        }
    }
    
    public Vector3 GetTargetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0f)
        {
            enabled = false;
            IsDestroyed = true;
        }
    }
}