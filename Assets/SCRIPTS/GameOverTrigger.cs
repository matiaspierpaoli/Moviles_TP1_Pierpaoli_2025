using UnityEngine;

using static GameSignals;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag;

    [SerializeField] private int player1Id;
    [SerializeField] private int player2Id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == playerTag)
        {
            if (other.gameObject.GetComponent<Player>().IdPlayer == player1Id)
            {
                RaisePlayerFinished(player1Id);
            }
            else if (other.gameObject.GetComponent<Player>().IdPlayer == player2Id)
            {
                RaisePlayerFinished(player2Id);
            }
        }      
    }
}
