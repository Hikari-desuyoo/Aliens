
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

    public static float GetAcceleration(Controller left, Controller right)
    {
        // localPosition to avoid flickering during fast spaceship movement
        var distance = Vector3.Distance(left.transform.localPosition, right.transform.localPosition);
        var min = 0.5f;
        var max = 0.9f;
        distance = Mathf.Clamp(distance, min, max);

        return (distance - min) / (max - min);
    }

    public static float GetPitch(Controller left, Controller right)
    {
        var result = -(left.GetRotation() + right.GetRotation()) / 2;
        if(result > -0.1 && result < 0.1) return 0f;

        return result;
    }

    public static float GetYaw(Controller left, Controller right)
    =>
    GetAxis(left, right, Vector3.right);

    public static float GetRoll(Controller left, Controller right)
    =>
    GetAxis(left, right, Vector3.up);

    public static float GetAxis(Controller left, Controller right, Vector3 axis)
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
    }

    public override void OnDrop()
    {
        _moveToOrigin = true;
    }

    public float GetRotation()
    {
        var angle = 360 - transform.localRotation.eulerAngles.z + 90;
        angle %= 180;
        return (angle / 180) * 2 - 1;
    }

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
