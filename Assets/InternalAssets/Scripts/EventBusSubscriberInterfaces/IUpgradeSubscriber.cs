using EventBusSystem;

public interface IUpgradeSubscriber : IGlobalSubscriber
{
    void HandleDamageUpgrade(int gradeNumber);
    void HandleSpeedUpgrade(int gradeNumber);
    void HandleRangeUpgrade(int gradeNumber);
}