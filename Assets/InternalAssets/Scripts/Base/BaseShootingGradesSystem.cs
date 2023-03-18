using System.Collections.Generic;

using EventBusSystem;

using UnityEngine;


public class BaseShootingGradesSystem : MonoBehaviour, IUpgradeSubscriber
{
    [SerializeField] private AttackRangeCircle _attackRangeCircle;

    private ISettingsGetter _settings;

    private Dictionary<int, ValueCostPair> _damageAmountGrades;
    public float DamageAmountValue { get => _damageAmountGrades[_currentDamageAmountGrade].value; }
    private int _currentDamageAmountGrade = 0;

    private Dictionary<int, ValueCostPair> _shotAmountPerSecondGrades;
    public float ShotAmountPerSecondValue { get => _shotAmountPerSecondGrades[_currentShotAmountPerSecondGrade].value; }
    private int _currentShotAmountPerSecondGrade = 0;

    private Dictionary<int, ValueCostPair> _shootingRangeGrades;
    public float ShootingRangeValue { get => _shootingRangeValue; }
    private int _currentShootingRangeGrade = 0;
    private float _shootingRangeValue;


    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void Setup(ISettingsGetter settings)
    {
        _settings = settings;

        _damageAmountGrades = SetupGradeCollection(_settings.BaseDamageSettings.DamageAmountPerShot);
        _shotAmountPerSecondGrades = SetupGradeCollection(_settings.BaseDamageSettings.ShotAmountPerSecond);
        _shootingRangeGrades = SetupGradeCollection(_settings.BaseDamageSettings.ShootingRange);

        HandleRangeUpgrade(0);
    }

    private Dictionary<int, ValueCostPair> SetupGradeCollection(IEnumerable<BaseDamageSettings.UpgradeValuePair> from)
    {
        Dictionary<int, ValueCostPair> to = new Dictionary<int, ValueCostPair>();

        foreach (var pair in from)
        {
            to.Add(pair.upgradeLvl, new ValueCostPair(pair.value, pair.cost));
        }

        return to;
    }

    public void HandleDamageUpgrade(int gradeNumber)
    {
        if (gradeNumber < _damageAmountGrades.Count)
        {
            _currentDamageAmountGrade = gradeNumber;
        }
    }

    public void HandleSpeedUpgrade(int gradeNumber)
    {
        if (gradeNumber < _shotAmountPerSecondGrades.Count)
        {
            _currentShotAmountPerSecondGrade = gradeNumber;
        }
    }
    
    public void HandleRangeUpgrade(int gradeNumber)
    {
        if (gradeNumber < _shootingRangeGrades.Count)
        {
            _currentShootingRangeGrade = gradeNumber;
            _attackRangeCircle.SetRange(_shootingRangeGrades[_currentShootingRangeGrade].value);
            _shootingRangeValue = _attackRangeCircle.GetDistanceToExtremePoint(transform.position);
        }
    }

    private struct ValueCostPair
    {
        public float value;
        public int cost;

        public ValueCostPair(float value, int cost)
        {
            this.value = value;
            this.cost = cost;
        }
    }
}