using EventBusSystem;

public interface IBaseDestroyedSubscriber : IGlobalSubscriber
{
    void HandleBaseDestroyed();
}