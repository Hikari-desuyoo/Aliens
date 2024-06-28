
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Spaceship : UdonSharpBehaviour
{
    // Defined by a script listening to the VRCStation events
    public bool localPlayerUsing = false;
    public float engineForce;
    private Rigidbody _rb;
    // Defines the targeted roll of the spacecraft: https://en.wikipedia.org/wiki/Aircraft_principal_axes
    private float _roll = 0f;

    void Start()
    {
        // Caching components is good practice!
        _rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called instead of Update to prevent
    // different behaviours based on the players fps, e.g.
    // engine force appearing stronger or weaker
    void FixedUpdate()
    {
        // We only want to handle controls for the local player.
        // Each client will deal with their own spaceship and
        // then sync the transform
        if(!localPlayerUsing) return;

        HandleRotation();

        // When space key is pressed, the spaceship receives force upwards
        // (relative to the spaceship, hence the `transform.rotation *`) on
        // acceleration mode (continuous, mass-independent)
        if (Input.GetKey(KeyCode.Space))
        {
            _rb.AddForce(transform.rotation * Vector3.up * engineForce, ForceMode.Acceleration);
        }

        // When shift key is pressed, the spaceship receives force forward
        // in a similar way to the space key behaviour.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _rb.AddForce(transform.rotation * Vector3.forward * engineForce, ForceMode.Acceleration);
        }


        // Roll is controlled by using Q and E keys
        if (Input.GetKey(KeyCode.Q)) _roll += 5f;

        if (Input.GetKey(KeyCode.E)) _roll -= 5f;
    }

    // Uses Slerp method to smoothly rotate the spaceship
    // in the targeted rotation, defined by where the player
    // is looking + the target roll angle
    private void HandleRotation()
    {
        var targetEulerRotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation.eulerAngles;
        targetEulerRotation.z = _roll;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(targetEulerRotation),
            Time.deltaTime
        );
    }
}
