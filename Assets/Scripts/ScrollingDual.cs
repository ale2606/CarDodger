using UnityEngine;

public class ScrollingDual : MonoBehaviour
{
    public Transform bg1;
    public Transform bg2;
    public float height = 7f;

    void Start()
    {
        bg1.position = new Vector3(0, 0, 0);
        bg2.position = new Vector3(0, height, 0);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.gameOver) return; // ← agrega esta línea

        float speed = GameManager.Instance.GetSpeed() * 0.4f;

        bg1.position += Vector3.down * speed * Time.deltaTime;
        bg2.position += Vector3.down * speed * Time.deltaTime;

        if (bg1.position.y <= -height)
            bg1.position = new Vector3(0, bg2.position.y + height, 0);
        if (bg2.position.y <= -height)
            bg2.position = new Vector3(0, bg1.position.y + height, 0);
    }
}