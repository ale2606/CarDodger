using UnityEngine;

public class ScrollingBG : MonoBehaviour
{
    public Renderer bgRenderer;

    void Update()
    {
        if (GameManager.Instance == null) return;

        float speed = GameManager.Instance.GetSpeed() * 0.03f;
        bgRenderer.material.mainTextureOffset += new Vector2(0, -speed * Time.deltaTime);
    }
}