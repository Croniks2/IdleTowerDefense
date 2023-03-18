using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemiesLogic : MonoBehaviour
{
    [SerializeField] private EnemiesPool _enemiesPool;

    public IEnumerable<IShootingTarget> ShootingTargets => _enemies;
    private LinkedList<EnemyPoolObject> _enemies;
    private List<LinkedListNode<EnemyPoolObject>> _nodesForRemove;

    private ISettingsGetter _settings;

    private float _circleRadiusOffset;
    private int _wavesCount;
    private float _wavesInterval;
    private int _enemiesCountPerWave;
    private float _enemiesInterval;
    
    private Vector3 _basePosition;
    private Vector3 _spawnCircleRadius;
    private Coroutine _spawnCoroutine;

    private float _timeElapsedSinceLastWave = 0f; 
    private int _enemiesCountSpawnedSinceLastWave = 0; 
    private float _timeElapsedSinceLastEnemySpawned = 0f; 

    #region SetupLogic

    public void Setup(ISettingsGetter settings, Vector3 basePosition)
    {
        _settings = settings;
        ApplySettings(settings);
        _basePosition = basePosition;
        DefineSpawnCircleRadius();

        if(_enemies != null)
        {
            foreach (var enemy in _enemies)
            {
                enemy.ReturnToPool();
            }

            _enemies.Clear();
        }
        _enemies = new LinkedList<EnemyPoolObject>();
        
        if (_nodesForRemove != null)
        {
            _nodesForRemove.Clear();
        }
        _nodesForRemove = new List<LinkedListNode<EnemyPoolObject>>();
    }
    
    private void ApplySettings(ISettingsGetter settings)
    {
        _circleRadiusOffset = settings.SpawnCircleRadiusOffset;
        _wavesCount = settings.WavesCount;
        _wavesInterval = settings.WavesInterval;
        _enemiesCountPerWave = settings.EnemiesCountPerWave;
        _enemiesInterval = settings.EnemiesInterval;
    }

    private void DefineSpawnCircleRadius()
    {
        Vector3 center = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
        Vector3 downRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1f + _circleRadiusOffset, 0f - _circleRadiusOffset, 1f));

        _spawnCircleRadius = downRightCorner - center;
    }

    #endregion

    #region UnityCalls

    private void Start()
    {
        //_spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        SpawnEnemiesEndlessly();

        ControlEnemies();
    }
    
    private void OnDisable()
    {
        if(_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
    }

    #endregion

    #region PrivateMethods

    private IEnumerator SpawnEnemies()
    {
        for(int i = 0; i < _wavesCount; i++)
        {
            yield return new WaitForSeconds(_wavesInterval);

            for (int j = 0; j < _enemiesCountPerWave; j++)
            {
                yield return new WaitForSeconds(_enemiesInterval);

                Vector3 spawnPos = GetRandomSpawnPosition();

                EnemyPoolObject enemy = _enemiesPool.Spawn();
                enemy.transform.position = spawnPos;
                LinkedListNode<EnemyPoolObject> enemyNode = _enemies.AddLast(enemy);
                
                enemy.Setup(enemyNode, spawnPos, _basePosition, _settings);
            }
        }
    }

    private void SpawnEnemiesEndlessly()
    {
        _timeElapsedSinceLastWave += Time.deltaTime;

        if (_timeElapsedSinceLastWave >= _wavesInterval)
        {
            if(_enemiesCountSpawnedSinceLastWave < _enemiesCountPerWave)
            {
                _timeElapsedSinceLastEnemySpawned += Time.deltaTime;

                if (_timeElapsedSinceLastEnemySpawned >= _enemiesInterval)
                {
                    Vector3 spawnPos = GetRandomSpawnPosition();

                    EnemyPoolObject enemy = _enemiesPool.Spawn();
                    enemy.transform.position = spawnPos;
                    LinkedListNode<EnemyPoolObject> enemyNode = _enemies.AddLast(enemy);

                    enemy.Setup(enemyNode, spawnPos, _basePosition, _settings);

                    _enemiesCountSpawnedSinceLastWave++;
                    _timeElapsedSinceLastEnemySpawned = 0f;
                }
            }
            else
            {
                _enemiesCountSpawnedSinceLastWave = 0;
                _timeElapsedSinceLastWave = 0f;
            }
        }
    }

    private void ControlEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.enabled == true)
            {
                enemy.MoveToBase();
            }
            else
            {
                _nodesForRemove.Add(enemy.EnemyNode);
                enemy.ReturnToPool();
            }
        }

        if (_nodesForRemove.Count > 0)
        {
            _nodesForRemove.ForEach(n => _enemies.Remove(n));
            _nodesForRemove.Clear();
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int randomAngle = UnityEngine.Random.Range(0, 359);
        Vector3 spawnPosition = Quaternion.AngleAxis(randomAngle, Vector3.forward) * _spawnCircleRadius;
        return spawnPosition;
    }

    #endregion
}