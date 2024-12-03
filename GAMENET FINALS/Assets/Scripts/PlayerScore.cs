using UnityEngine;
using Mirror;

public class PlayerScore : NetworkBehaviour
{
    [SyncVar] public int Score = 0;

    public void AddPoint()
    {
        if (isServer)
        {
            Score++;
        }
    }
}
