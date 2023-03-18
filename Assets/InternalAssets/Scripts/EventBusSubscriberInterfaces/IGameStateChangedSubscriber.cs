using EventBusSystem;

public interface IGameStateChangedSubscriber : IGlobalSubscriber
{
    void HandleGameRestart();
    void HandleGameOver();
}