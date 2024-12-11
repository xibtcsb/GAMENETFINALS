using UnityEngine;
using Mirror;
using System.Collections;

public class PickUpBomb : NetworkBehaviour
{
    public Transform holdPos;
    public float pickUpRange = 5f;
    public float passRange = 2f;
    public float passCooldown = 1.5f;
    public float roundDuration = 40f; // Round duration in seconds

    [SyncVar] private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canPassBomb = true;

    // Round management variables
    private float roundTimeRemaining;
    private bool isRoundOver = false;

    // UI Score
    [SyncVar] private int playerScore = 0;

    public bool isPlayerAlive { get; private set; } = true; // Added for alive check

    void Start()
    {
        if (isLocalPlayer)
        {
            roundTimeRemaining = roundDuration;
            StartCoroutine(RoundTimer());
        }
    }

    void Update()
    {
        if (!isLocalPlayer || isRoundOver) return;

        if (heldObj == null)
        {
            AutoPickUpObject();
        }
        else
        {
            AttemptToPassBomb();
        }

        if (roundTimeRemaining <= 0 && !isRoundOver)
        {
            EndRound();
        }
    }

    private void AutoPickUpObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickUpRange))
        {
            if (hit.transform.CompareTag("canPickUp"))
            {
                CmdPickUpObject(hit.transform.gameObject);
            }
        }
    }

    private void AttemptToPassBomb()
    {
        if (canPassBomb)
        {
            Collider[] nearbyPlayers = Physics.OverlapSphere(transform.position, passRange);
            foreach (Collider col in nearbyPlayers)
            {
                if (col.CompareTag("Player") && col.gameObject != gameObject)
                {
                    CmdPassBomb(col.gameObject);
                    break;
                }
            }
        }
    }

    [Command]
    private void CmdPickUpObject(GameObject pickUpObj)
    {
        if (heldObj != null) return;

        if (pickUpObj.TryGetComponent(out Rigidbody rb))
        {
            heldObj = pickUpObj;
            heldObjRb = rb;

            heldObjRb.isKinematic = true;
            heldObj.transform.SetParent(holdPos);
            heldObj.transform.localPosition = Vector3.zero;

            NetworkIdentity objNetId = heldObj.GetComponent<NetworkIdentity>();
            if (objNetId != null)
            {
                objNetId.RemoveClientAuthority();
                objNetId.AssignClientAuthority(connectionToClient);
            }

            RpcSyncPickUp(heldObj);
        }
    }

    [ClientRpc]
    private void RpcSyncPickUp(GameObject pickUpObj)
    {
        if (pickUpObj.TryGetComponent(out Rigidbody rb))
        {
            heldObj = pickUpObj;
            heldObjRb = rb;

            heldObjRb.isKinematic = true;
            heldObj.transform.SetParent(holdPos);
            heldObj.transform.localPosition = Vector3.zero;

            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
    }

    [Command]
    private void CmdPassBomb(GameObject targetPlayer)
    {
        if (heldObj == null || !canPassBomb) return;

        if (targetPlayer.GetComponent<PickUpBomb>().heldObj == null)
        {
            NetworkIdentity targetPlayerNetId = targetPlayer.GetComponent<NetworkIdentity>();
            RpcPassBomb(targetPlayerNetId);
        }
    }

    [ClientRpc]
    private void RpcPassBomb(NetworkIdentity targetPlayerNetId)
    {
        PickUpBomb targetPickUpBomb = targetPlayerNetId.GetComponent<PickUpBomb>();
        targetPickUpBomb.CmdPickUpObject(heldObj);

        heldObj = null;
        heldObjRb = null;

        StartCoroutine(PassCooldown());
    }


    private IEnumerator PassCooldown()
    {
        canPassBomb = false;
        yield return new WaitForSeconds(passCooldown);
        canPassBomb = true;
    }

    private IEnumerator RoundTimer()
    {
        while (roundTimeRemaining > 0 && !isRoundOver)
        {
            roundTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        if (roundTimeRemaining <= 0)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        isRoundOver = true;
        // add kayo logic to find the winner (the last player standing)
        if (heldObj != null)
        {
            CmdAwardPoint();
        }

        // Reset round?

    }

    [Command]
    private void CmdAwardPoint()
    {
        playerScore++;
        RpcUpdateScore(playerScore);
    }

    [ClientRpc]
    private void RpcUpdateScore(int newScore)
    {
        playerScore = newScore;
        UpdateScoreUI(newScore);  // You should implement this method to update your UI
    }

    private void UpdateScoreUI(int newScore)
    {
        // gawa kayo ng function para mag add ng score sa UI
    }
}
