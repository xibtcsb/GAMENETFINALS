using UnityEngine;
using Mirror;
using System.Collections;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public TextMeshProUGUI roundTimerText;
    public TextMeshProUGUI playerScoreText;
    public float roundDuration = 40f;
    private float roundTimeRemaining;
    private int playerCount;
    private bool roundInProgress = false;

    private static int playerScore = 0;

    void Start()
    {
        roundTimeRemaining = roundDuration;
        playerCount = NetworkServer.connections.Count;
    }

    void Update()
    {
        if (roundInProgress)
        {
            HandleRoundTimer();
        }
    }

    void HandleRoundTimer()
    {
        roundTimeRemaining -= Time.deltaTime;
        roundTimerText.text = "Time: " + Mathf.Ceil(roundTimeRemaining).ToString();

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
                playerScore += 1;
                UpdateScoreUI(playerScore);
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

    void UpdateScoreUI(int score)
    {
        playerScoreText.text = "Score: " + score;
    }
}
