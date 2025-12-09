using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject ExplosionEffectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        GameObject effectObject = Instantiate(ExplosionEffectPrefab);
        effectObject.transform.position = transform.position;

        Destroy(gameObject);
    }
}
