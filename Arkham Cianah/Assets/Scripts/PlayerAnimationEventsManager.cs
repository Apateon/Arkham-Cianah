using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventsManager : MonoBehaviour
{
    public PlayerManager player;
    public void KickSuccess()
    {
        player.KickSummit();
    }
}
