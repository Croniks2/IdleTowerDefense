using UnityEngine;

public class AttackRangeCircle : MonoBehaviour
{ 
    [SerializeField] private Transform _extremePoint;

    public float GetDistanceToExtremePoint(Vector3 fromPostion)
    {
        return (_extremePoint.position - fromPostion).magnitude;
    }

    public void SetRange(float value)
    {
        transform.localScale = Vector3.one * value;
    }
}