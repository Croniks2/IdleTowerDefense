using EventBusSystem;

using UnityEngine;

public class GameLogic : MonoBehaviour, IBaseDestroyedSubscriber, IGameStateChangedSubscriber
{
    [SerializeField] private BaseLogic _baseLogic;
    [SerializeField] private EnemiesLogic _enemiesLogic;
    [SerializeField] private SettingsObject _settings;

    private void Awake()
    {
        _enemiesLogic.Setup(_settings, _baseLogic.transform.position);
        _baseLogic.Setup(_settings, _enemiesLogic.ShootingTargets);
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void HandleBaseDestroyed()
    {
        EventBus.RaiseEvent<IGameStateChangedSubscriber>(h => h.HandleGameOver());
    }

    public void HandleGameRestart()
    {
        _enemiesLogic.Setup(_settings, _baseLogic.transform.position);
        _baseLogic.Setup(_settings, _enemiesLogic.ShootingTargets);

        _enemiesLogic.enabled = true;
        _baseLogic.enabled = true;
    }

    public void HandleGameOver()
    {
        _enemiesLogic.enabled = false;
        _baseLogic.enabled = false;
    }
}