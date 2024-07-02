
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SeatButton : UdonSharpBehaviour
{
    public VRCStation seat;

    public override void Interact()
    {
        seat.UseStation(Networking.LocalPlayer);
    }
}
