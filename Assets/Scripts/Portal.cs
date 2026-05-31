using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform otherPortal;
    float cooldown = 0f;

    void Update()
    {
        if (cooldown > 0) cooldown -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        if (cooldown > 0) return;

        Vector3 destino = new Vector3(
            otherPortal.position.x,
            otherPortal.position.y + 0.5f,
            0
        );

        Debug.Log("Teletransportando a: " + destino);
        col.transform.position = destino;
        cooldown = 1.2f;
    }
}