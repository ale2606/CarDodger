using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerJ1 : MonoBehaviour
{
    public float speedH = 4f;
    public float speedV = 3f;

    // Limites del carril izquierdo
    float minX = -5.69f;
    float maxX = 0.65f;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;

        // Jugador 1 usa WASD
        if (kb.aKey.isPressed) h = -1f;
        if (kb.dKey.isPressed) h = 1f;
        if (kb.wKey.isPressed) v = 1f;
        if (kb.sKey.isPressed) v = -1f;

        Vector3 pos = transform.position;
        pos.x += h * speedH * Time.deltaTime;
        pos.y += v * speedV * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, -3.2f, 2.5f);
        transform.position = pos;
    }
}