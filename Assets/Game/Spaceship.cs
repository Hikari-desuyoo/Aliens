
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Spaceship : UdonSharpBehaviour
{
    public VRCStation station;
    public float upForce = 10f;
    private Rigidbody _rb;
    private float _roll = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        Vector3 eulerRotation = rotation.eulerAngles;
        eulerRotation.z = _roll;
        rotation = Quaternion.Euler(eulerRotation);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _rb.AddForce(transform.rotation * Vector3.up * upForce, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(KeyCode.Q)) _roll -= 5f;

        if (Input.GetKeyUp(KeyCode.E)) _roll += 5f;
    }
}
