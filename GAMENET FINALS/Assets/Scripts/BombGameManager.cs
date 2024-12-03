using UnityEngine;
using Mirror;

public class BombGameManager : NetworkBehaviour
{
    [SyncVar] public GameObject bombHolder;
    [SyncVar] private float timer = 10f;

    public float roundDuration = 10f;
    public GameObject bombEffect; // Optional particle effect for the bomb

    private void Start()
    {
        if (isServer)
        {
            AssignBombToRandomPlayer();
        }
    }

    private void Update()
    {
        if (!isServer) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            EndRound();
        }
    }

    [Server]
    private void AssignBombToRandomPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            bombHolder = players[Random.Range(0, players.Length)];
            RpcShowBombEffect(bombHolder);
            timer = roundDuration;
        }
    }

    [Server]
    public void TransferBomb(GameObject newHolder)
    {
        bombHolder = newHolder;
        RpcShowBombEffect(newHolder);
        timer = roundDuration;
    }

    [Server]
    private void EndRound()
    {
        if (bombHolder != null)
        {
            // Assign points to other players
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                if (player != bombHolder)
                {
                    player.GetComponent<PlayerScore>().AddPoint();
                }
            }
        }

        // Check for a winner
        var winner = CheckWinner();
        if (winner != null)
        {
            RpcAnnounceWinner(winner.GetComponent<NetworkIdentity>().connectionToClient);
        }
        else
        {
            AssignBombToRandomPlayer();
        }
    }

    [Server]
    private GameObject CheckWinner()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerScore>().Score >= 5)
            {
                return player;
            }
        }
        return null;
    }

    [ClientRpc]
    private void RpcShowBombEffect(GameObject holder)
    {
        if (bombEffect != null && holder != null)
        {
            Instantiate(bombEffect, holder.transform.position, Quaternion.identity, holder.transform);
        }
    }

    [TargetRpc]
    private void RpcAnnounceWinner(NetworkConnection target)
    {
        Debug.Log("Game Over! We have a winner!");
    }
}
