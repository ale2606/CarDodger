using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float resetY = 10f;
    public float bottomY = -10f;

    private bool paused = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (paused) return;

        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        if (transform.position.y < bottomY)
            transform.position = new Vector3(
                startPosition.x,
                transform.position.y + (resetY * 2f),
                startPosition.z
            );
    }
    public void SetSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }

    public void PauseScroll() => paused = true;
    public void ResumeScroll() => paused = false;
}