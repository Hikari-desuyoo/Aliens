
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpaceshipSeat : UdonSharpBehaviour
{
    public Spaceship spaceship;
    public override void OnStationEntered(VRCPlayerApi player)
    {
        if(player == Networking.LocalPlayer)
        {
            spaceship.localPlayerUsing = true;
            // this part is really important
            // because then all the movement
            // that is applied to the local
            // player spaceship will be synced
            // to everyboidy else
            Networking.SetOwner(Networking.LocalPlayer, spaceship.gameObject);
        }
    }

    public override void OnStationExited(VRCPlayerApi player)
    {
        if(player == Networking.LocalPlayer) spaceship.localPlayerUsing = false;
    }
}
