using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speedH = 4f;
    public float speedV = 3f;
    public float portalCooldown = 0f;
    public Sprite[] spritesCarros;

    void Start()
    {
        int carroIndex = PlayerPrefs.GetInt("CarroP1", 0);
        if (spritesCarros != null && spritesCarros.Length > carroIndex)
            GetComponent<SpriteRenderer>().sprite = spritesCarros[carroIndex];
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;

        if (portalCooldown > 0)
            portalCooldown -= Time.deltaTime;

        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;
        if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) h = -1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) h = 1f;
        if (kb.upArrowKey.isPressed || kb.wKey.isPressed) v = 1f;
        if (kb.downArrowKey.isPressed || kb.sKey.isPressed) v = -1f;

        Vector3 pos = transform.position;
        pos.x += h * speedH * Time.deltaTime;
        pos.y += v * speedV * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -1.84f, 1.89f);
        pos.y = Mathf.Clamp(pos.y, -3.2f, 2.5f);
        transform.position = pos;
    }
}