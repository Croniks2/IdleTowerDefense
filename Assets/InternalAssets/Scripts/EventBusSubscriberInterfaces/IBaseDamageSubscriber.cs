using EventBusSystem;

public interface IBaseDamageSubscriber : IGlobalSubscriber
{
    void HandleBaseDamage(float damage);
}