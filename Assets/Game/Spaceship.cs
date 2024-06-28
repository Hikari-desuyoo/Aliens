
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Spaceship : UdonSharpBehaviour
{
    // Defined by a script listening to the VRCStation events
    public bool localPlayerUsing = false;

    public float engineForce;
    public float engineRotationalForce;

    // How much force is used for stabilization
    // 0 means 0% of engineForce
    // 100 means 100% of engineForce
    public float stabilization;

    // How much force is used for stabilization
    // of angular force. Same as stabilization
    // but relative to engineRotationalForce
    public float rotationalStabilization;
    private Rigidbody _rb;

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
        // We only want to handle spaceship movement for the
        // local player. Each client will deal with their own
        // spaceship and then sync the transform
        if(!localPlayerUsing) return;

        HandleForce();
        HandleTorque();
    }

    private void HandleForce()
    {
        // When space key is pressed, the spaceship receives force upwards
        // (relative to the spaceship, hence the `transform.rotation *`) on
        // acceleration mode (continuous, mass-independent)
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyForce(transform.rotation * Vector3.up * engineForce);
            return;
        }

        // When shift key is pressed, the spaceship receives force forward
        // in a similar way to the space key behaviour.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ApplyForce(transform.rotation * Vector3.forward * engineForce);
            return;
        }

        // Stabilize force
        // when it is not being controlled by user
        ApplyForce(-_rb.velocity.normalized * engineForce * stabilization);
    }

    // Applies angular force to spin the ship based
    // on user input
    private void HandleTorque()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            ApplyTorque(transform.rotation * Vector3.forward * engineRotationalForce);
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            ApplyTorque(transform.rotation * -Vector3.forward * engineRotationalForce);
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            ApplyTorque(transform.rotation * -Vector3.up * engineRotationalForce);
            return;
        }

        if (Input.GetKey(KeyCode.D))
        {
            ApplyTorque(transform.rotation * Vector3.up * engineRotationalForce);
            return;
        }

        if (Input.GetKey(KeyCode.S))
        {
            ApplyTorque(transform.rotation * -Vector3.right * engineRotationalForce);
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            ApplyTorque(transform.rotation * Vector3.right * engineRotationalForce);
            return;
        }

        // Stabilize angular motion
        // when it is not being controlled by user
        ApplyTorque(-_rb.angularVelocity.normalized * engineRotationalForce * rotationalStabilization);
    }

    private void ApplyTorque(Vector3 torque)
    {
        // ForceMode.Force means continuous force, considering mass
        _rb.AddTorque(torque, ForceMode.Force);
    }

    private void ApplyForce(Vector3 torque)
    {
        // ForceMode.Force means continuous force, considering mass
        _rb.AddForce(torque, ForceMode.Force);
    }
}
