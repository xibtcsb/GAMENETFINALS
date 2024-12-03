using UnityEngine;
using Mirror;

public class BombCollider : NetworkBehaviour
{
    private BombGameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<BombGameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager != null && gameManager.bombHolder == gameObject)
            {
                gameManager.TransferBomb(collision.gameObject);
            }
        }
    }
}
