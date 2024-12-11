using UnityEngine;
using Mirror;
using System.Collections;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public float roundDuration = 40f;
    private float roundTimeRemaining;
    private int playerCount;
    private bool roundInProgress = false;
    public float _getRoundTimeRemaining { get { return roundTimeRemaining; } }
    [SyncVar] Player m_PlayerHost;
    public Player PlayerHost { get => m_PlayerHost; set => m_PlayerHost = value; }
    private static int playerScore = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        roundTimeRemaining = roundDuration;
        playerCount = NetworkServer.connections.Count;
        roundInProgress = true;
    }

    void Update()
    {
        Debug.Log(roundTimeRemaining);
        if (roundInProgress)
        {
            HandleRoundTimer();
        }
    }

    void HandleRoundTimer()
    {
        roundTimeRemaining -= Time.deltaTime;
        PlayerHost.m_BombTimer.text = "Time: " + Mathf.Ceil(roundTimeRemaining).ToString();

        if (roundTimeRemaining <= 0)
        {
            EndRound();
        }
    }

    void EndRound()
    {
        CmdCheckRoundEnd();
    }

    [Command]
    public void CmdCheckRoundEnd()
    {
        int remainingPlayers = CountRemainingPlayers();
        if (remainingPlayers == 1)
        {
            RpcDeclareWinner();
        }
        else if (roundTimeRemaining <= 0)
        {
            RpcRoundEnd();
        }
    }

    [ClientRpc]
    void RpcDeclareWinner()
    {
        foreach (var conn in NetworkServer.connections)
        {
            var playerScript = conn.Value.identity.GetComponent<PickUpBomb>();
            if (playerScript != null && playerScript.isPlayerAlive)
            {
                PlayerHost.score += 1;
                RpcRoundEnd();
                break;
            }
        }

        RpcRoundEnd();
    }

    [ClientRpc]
    void RpcRoundEnd()
    {
        roundInProgress = false;
        StartCoroutine(RestartRound());
    }

    IEnumerator RestartRound()
    {
        yield return new WaitForSeconds(2f);
        roundTimeRemaining = roundDuration;
        roundInProgress = true;
        playerCount = NetworkServer.connections.Count;
    }

    int CountRemainingPlayers()
    {
        int count = 0;
        foreach (var conn in NetworkServer.connections)
        {
            var playerScript = conn.Value.identity.GetComponent<PickUpBomb>();
            if (playerScript != null && playerScript.isPlayerAlive)
            {
                count++;
            }
        }
        return count;
    }

}
