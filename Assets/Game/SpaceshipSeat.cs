
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpaceshipSeat : UdonSharpBehaviour
{
    public Spaceship spaceship;
    public override void OnStationEntered(VRCPlayerApi player)
    {
        if(player == Networking.LocalPlayer) spaceship.localPlayerUsing = true;
    }

    public override void OnStationExited(VRCPlayerApi player)
    {
        if(player == Networking.LocalPlayer) spaceship.localPlayerUsing = false;
    }
}
