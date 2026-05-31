using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Transform portalLeft;
    public Transform portalRight;
    float portalCooldown = 0f;
    float cooldownGolpe = 0f;

    void Update()
    {
        if (portalCooldown > 0) portalCooldown -= Time.deltaTime;
        if (cooldownGolpe > 0) cooldownGolpe -= Time.deltaTime;

        if (GameManager.Instance.gameOver) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            new Vector2(0.8f, 1.0f),
            0f
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Portal") && portalCooldown <= 0)
            {
                if (hit.transform == portalLeft)
                    transform.position = new Vector3(portalRight.position.x, portalRight.position.y + 0.5f, 0);
                else
                    transform.position = new Vector3(portalLeft.position.x, portalLeft.position.y + 0.5f, 0);
                portalCooldown = 1.2f;
                return;
            }

            if ((hit.CompareTag("EnemyCar") || hit.CompareTag("Sign")) && cooldownGolpe <= 0)
            {
                Debug.Log("Golpe detectado con: " + hit.tag);
                cooldownGolpe = 1.5f;
                VidasManager.Instance.PerderVida();
                return;
            }
        }
    }
}