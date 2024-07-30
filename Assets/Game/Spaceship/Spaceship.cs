
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Spaceship : UdonSharpBehaviour
{
    // Defined by a script listening to the VRCStation events
    public bool localPlayerUsing = false;

    public AnimationCurve engineForce;
    public float engineRotationalForce;
    public Gun gun;
    public Controller leftController;
    public Controller rightController;

    // How much force is used for stabilization
    // 0 means 0% of engineForce
    // 100 means 100% of engineForce
    public float stabilization;

    // How much force is used for stabilization
    // of angular force. Same as stabilization
    // but relative to engineRotationalForce
    public float rotationalStabilization;
    private Rigidbody _rb;
    private float _accelerationTime = 0;
    private float _stabilizationTime = 0;
    private float _rotationTime = 0;
    private bool _vr = false;

    // Shoots gun if trigger/mouse button is pressed
    public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args) {
        var shootingValue = true;

        if (
            gun == null || // no gun or...
            !localPlayerUsing || // no local player on ship or...
            !value // not even pressing the button means...
        )
        {
            shootingValue = false; // won't shoot!
        };

        gun.shooting = shootingValue;
    }

    void Start()
    {
        // Caching components is good practice!
        _rb = GetComponent<Rigidbody>();
        _vr = Networking.LocalPlayer.IsUserInVR();
        _vr = true;
    }

    void PostLateUpdate()
    {
        // We only want to handle spaceship movement for the
        // local player. Each client will deal with their own
        // spaceship and then sync the transform
        if(!localPlayerUsing) return;

        // HandleForce();
        HandleTorque();
    }

    void HandleForce()
    {
        // When shift key is pressed, the spaceship receives force forward
        // in a similar way to the space key behaviour. The acceleration
        // gradually increases based on a curve, just like real engines!

        var acceleration = 0f;
        if(_vr) acceleration = Controller.GetAcceleration(leftController, rightController);
        if(Input.GetKey(KeyCode.LeftShift)) acceleration = 1f;

        if (acceleration > 0)
        {
            _accelerationTime += Time.deltaTime;
            if(_stabilizationTime > 0) _stabilizationTime -= Time.deltaTime;
            ApplyForce(
                transform.rotation *
                Vector3.forward *
                engineForce.Evaluate(_accelerationTime) *
                Time.deltaTime
            );
            return;
        } else {
            // we diminish the time gradually instead of
            // resetting altogether to be more realistic
            // if the player accelerate again
            if(_accelerationTime > 0) _accelerationTime -= Time.deltaTime;

            // Stabilize force
            // when it is not being controlled by user
            _stabilizationTime += Time.deltaTime;
            ApplyForce(
                -_rb.velocity.normalized *
                engineForce.Evaluate(_stabilizationTime) *
                stabilization *
                Time.deltaTime
            );
        }
    }

    // Applies angular force to spin the ship based
    // on user input
    void HandleTorque()
    {
        float force = engineRotationalForce;

        var pitch = 0f;
        var yaw = 0f;
        var roll = 0f;

        if (_vr)
        {
            pitch = Controller.GetPitch(leftController, rightController);
            yaw = Controller.GetYaw(leftController, rightController);
            roll = Controller.GetRoll(leftController, rightController);
        }

        if (Input.GetKey(KeyCode.Q)) roll = 1;
        if (Input.GetKey(KeyCode.E)) roll = -1;
        if (Input.GetKey(KeyCode.A)) yaw = -1;
        if (Input.GetKey(KeyCode.D)) yaw = 1;
        if (Input.GetKey(KeyCode.S)) pitch = -1;
        if (Input.GetKey(KeyCode.W)) pitch = 1;

        var torqueDirection =
            transform.rotation *
            (
                Vector3.forward * roll +
                Vector3.up * yaw +
                Vector3.right * pitch
            );

        // Stabilize angular motion
        // when it is not being controlled by user
        if (torqueDirection == Vector3.zero)
        {
            // Stabilize angular motion when it is not being controlled by user
            torqueDirection = -_rb.angularVelocity.normalized;
            force = engineRotationalForce * rotationalStabilization;
        }

        ApplyTorque(torqueDirection * force * Time.deltaTime);
    }

    void ApplyTorque(Vector3 torque)
    {
        // ForceMode.Force means continuous force, considering mass
        _rb.AddTorque(torque, ForceMode.Force);
    }

    void ApplyForce(Vector3 torque)
    {
        // ForceMode.Force means continuous force, considering mass
        _rb.AddForce(torque, ForceMode.Force);
    }


}
