using UnityEngine;

public class OncomingCar : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        if (GameManager.Instance == null) return;

        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            GameManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            VidasManager.Instance.PerderVida();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            VidasManager.Instance.PerderVida();
        }
    }
}