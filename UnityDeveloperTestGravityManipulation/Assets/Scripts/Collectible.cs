using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.Collect();
            Destroy(gameObject);
        }
    }
}
