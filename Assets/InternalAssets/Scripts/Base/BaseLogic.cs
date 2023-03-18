using System.Collections.Generic;

using EventBusSystem;

using UnityEngine;


public class BaseLogic : MonoBehaviour, IBaseDamageSubscriber
{

    [SerializeField] private Transform _damageSpriteTransform;

    [SerializeField, Space] private ProjectilesPool _projectilesPool;
    [SerializeField] private BaseShootingGradesSystem _shootingGradesSystem;
    
    private ISettingsGetter _settings;
    private IEnumerable<IShootingTarget> _shootingTargets;
    private LinkedList<ProjectilePoolObject> _projectiles;
    private List<LinkedListNode<ProjectilePoolObject>> _projectilesNodesForRemove;

    private float _maxHP;
    private float _currentHP;
    private Vector3 _basePosition;
    
    private float _timeElapsedSinceLastShot = 0f;
    
    #region SetupLogic

    public void Setup(ISettingsGetter settings, IEnumerable<IShootingTarget> shootingTargets)
    {
        _basePosition = transform.position;

        _settings = settings;
        _shootingTargets = shootingTargets;

        if (_projectiles != null)
        {
            foreach (var projectile in _projectiles)
            {
                projectile.ReturnToPool();
            }

            _projectiles.Clear();
        }
        _projectiles = new LinkedList<ProjectilePoolObject>();

        if (_projectilesNodesForRemove != null)
        {
            _projectilesNodesForRemove.Clear();
        }
        _projectilesNodesForRemove = new List<LinkedListNode<ProjectilePoolObject>>();
        
        _maxHP = _currentHP = _settings.BaseMaxHP;
        _damageSpriteTransform.localScale = Vector3.zero;

        _shootingGradesSystem.Setup(_settings);
    }
    
    #endregion

    #region UnityCalls
    
    private void Update()
    {
        SpawnProjectile();

        MoveProjectiles();
    }
    
    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    #endregion

    #region PrivateMethods

    private void SpawnProjectile()
    {
        _timeElapsedSinceLastShot += Time.deltaTime;

        float shotTime = 1 / _shootingGradesSystem.ShotAmountPerSecondValue;

        if (_timeElapsedSinceLastShot >= shotTime)
        {
            float closestDistance = float.MaxValue;
            IShootingTarget currentTarget = null;
            float squareAttackRange = Mathf.Pow(_shootingGradesSystem.ShootingRangeValue, 2);

            foreach (IShootingTarget target in _shootingTargets)
            {
                if (target.IsDestroyed == true)
                {
                    continue;
                }

                Vector3 targetPosition = target.GetTargetPosition();
                float distanceToEnemy = (targetPosition - _basePosition).sqrMagnitude;
                float deltaRange = squareAttackRange - distanceToEnemy;

                if (deltaRange >= 0f && distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    currentTarget = target;
                }
            }

            if (currentTarget != null)
            {
                ProjectilePoolObject projectile = _projectilesPool.Spawn();
                projectile.transform.position = _basePosition;
                LinkedListNode<ProjectilePoolObject> node = _projectiles.AddLast(projectile);

                projectile.Setup(node, _settings, currentTarget, _shootingGradesSystem.DamageAmountValue);
                TurnProjectileTowardsEnemy(projectile.transform, currentTarget.GetTargetPosition());

                _timeElapsedSinceLastShot = 0f;
            }
        }
    }

    private void MoveProjectiles()
    {
        foreach (var projectile in _projectiles)
        {
            if (projectile.enabled == true)
            {
                projectile.MoveToEnemy();
            }
            else
            {
                _projectilesNodesForRemove.Add(projectile.ProjectileNode);
                projectile.ReturnToPool();
            }
        }

        if (_projectilesNodesForRemove.Count > 0)
        {
            _projectilesNodesForRemove.ForEach(n => _projectiles.Remove(n));
            _projectilesNodesForRemove.Clear();
        }
    }

    private void TurnProjectileTowardsEnemy(Transform projectileTrans, Vector3 enemyPosition)
    {
        Vector3 direction = enemyPosition - projectileTrans.position;
        projectileTrans.rotation = Quaternion.LookRotation(direction, projectileTrans.up);
    }

    #endregion

    #region EventHandlers

    public void HandleBaseDamage(float damage)
    {
        _currentHP -= damage;

        if(_currentHP <= 0f)
        {
            _damageSpriteTransform.localScale = Vector3.one;
            EventBus.RaiseEvent<IBaseDestroyedSubscriber>(h => h.HandleBaseDestroyed());
        }
        else
        {
            _damageSpriteTransform.localScale = (1f - (_currentHP / _maxHP)) * Vector3.one;
        }
    }
    
    #endregion
}