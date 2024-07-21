
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

    public static float GetDistance(Controller left, Controller right)
    {
        // localPosition to avoid flickering during fast spaceship movement
        return Vector3.Distance(left.transform.localPosition, right.transform.localPosition);
    }
    public static float GetPitch(Controller left, Controller right, Spaceship spaceship)
    {
        return (left.GetRotation() + right.GetRotation()) / 2;
    }

    public static float GetYaw(Controller left, Controller right, Spaceship spaceship)
    =>
    GetAxis(left, right, spaceship, Quaternion.Euler(0, -90, 0));

    public static float GetRoll(Controller left, Controller right, Spaceship spaceship)
    =>
    GetAxis(left, right, spaceship, Quaternion.Euler(-90, 0, 0));

    public static float GetAxis(Controller left, Controller right, Spaceship spaceship, Quaternion rotation)
    {
        var angle = Vector3.Angle(
            left.transform.localPosition - right.transform.localPosition,
            rotation * spaceship.transform.forward
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
        Debug.Log(angle);
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
        _moveToOrigin = Vector3.Distance(transform.localPosition, _restPosition) > 0.1f;
    }
}
