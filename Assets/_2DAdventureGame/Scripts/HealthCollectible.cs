using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int amount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.ChangeHealth(amount))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
