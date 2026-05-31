using UnityEngine;

public class ScrollingDualJ2 : MonoBehaviour
{
    public Transform bg1;
    public Transform bg2;
    public float height = 8f;
    public float speed = 3f;

    void Start()
    {
        bg1.position = new Vector3(bg1.position.x, 0, 0);
        bg2.position = new Vector3(bg2.position.x, height, 0);
    }

    void Update()
    {
        bg1.position += Vector3.down * speed * Time.deltaTime;
        bg2.position += Vector3.down * speed * Time.deltaTime;

        if (bg1.position.y <= -height)
            bg1.position = new Vector3(bg1.position.x, bg2.position.y + height, 0);
        if (bg2.position.y <= -height)
            bg2.position = new Vector3(bg2.position.x, bg1.position.y + height, 0);
    }
}