using UnityEngine;

public interface IShootingTarget
{
    bool IsDestroyed { get; set; }
    Vector3 GetTargetPosition();
    void TakeDamage(float damage);
}