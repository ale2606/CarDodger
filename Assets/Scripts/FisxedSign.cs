using UnityEngine;

public class FixedSign : MonoBehaviour
{
    public float speed = 3f;
    bool detenido = false;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (detenido) return;

        float velocidadFondo = GameManager.Instance.GetSpeed() * 0.4f;
        transform.Translate(Vector3.down * velocidadFondo * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            GameManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }

    public void Detener()
    {
        detenido = true;
    }
}