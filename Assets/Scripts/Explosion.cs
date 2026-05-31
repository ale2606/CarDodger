using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float duracion = 1.2f;
    float timer = 0f;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duracion;

        float escala = Mathf.Lerp(0.3f, 0.8f, t);
        transform.localScale = new Vector3(escala, escala, 1f);

        if (sr)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
        }

        if (timer >= duracion) Destroy(gameObject);
    }
}