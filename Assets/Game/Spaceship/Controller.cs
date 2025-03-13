
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Controller : UdonSharpBehaviour
{
    public float speed = 1f;
    Vector3 _restPosition;
    Quaternion _restRotation;
    bool _moveToOrigin = false;
    bool _holded = false;

    public static float GetForceInput(Controller left, Controller right)
    {
        // localPosition to avoid flickering during fast spaceship movement
        return Vector3.Distance(left.transform.localPosition, right.transform.localPosition);
    }

    public static float GetPitch(Controller left, Controller right)
    =>
    -GetRotation(left, right, 2);

    public static float GetYaw(Controller left, Controller right)
    =>
    (-GetRotation(left, right, 1) + GetAngle(left, right, Vector3.right)) / 2;

    public static float GetRoll(Controller left, Controller right)
    =>
    (GetRotation(left, right, 0) + GetAngle(left, right, Vector3.up)) / 2;

    public static float GetRotation(Controller left, Controller right, int i)
    =>
    left.GetRotation(i) + right.GetRotation(i) / 2;

    public static float GetAngle(Controller left, Controller right, Vector3 axis)
    {
        var angle = Vector3.Angle(
            left.transform.localPosition - right.transform.localPosition,
            axis
        );
        return (angle / 180) * 2 - 1;
    }

    public override void OnPickup()
    {
        _moveToOrigin = false;
        _holded = true;
    }

    public override void OnDrop()
    {
        _moveToOrigin = true;
        _holded = false;
    }

    public float GetRotation(int i)
    {
        var angle = 360 - transform.localRotation.eulerAngles[i] + 90;
        angle %= 180;
        return (angle / 180) * 2 - 1;
    }

    public bool IsBeingHolded() => _holded;

    void Start()
    {
        _restPosition = transform.localPosition;
        _restRotation = transform.localRotation;
    }

    void Update()
    {
        if(!_moveToOrigin) return;

        transform.localPosition = Vector3.Lerp(transform.localPosition, _restPosition, speed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _restRotation, speed * Time.deltaTime);
        _moveToOrigin = Vector3.Distance(transform.localPosition, _restPosition) > 0.01f;
    }
}
