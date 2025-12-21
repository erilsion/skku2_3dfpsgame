using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null)
            {
                coin.StartAttract(transform);
            }
        }
    }
}
