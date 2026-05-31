using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerJ2 : MonoBehaviour
{
    public float speedH = 4f;
    public float speedV = 3f;

   

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;

        // Jugador 2 usa flechas
        if (kb.leftArrowKey.isPressed) h = -1f;
        if (kb.rightArrowKey.isPressed) h = 1f;
        if (kb.upArrowKey.isPressed) v = 1f;
        if (kb.downArrowKey.isPressed) v = -1f;

        Vector3 pos = transform.position;
        pos.x += h * speedH * Time.deltaTime;
        pos.y += v * speedV * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, 5.63f, 11.3f);
        pos.y = Mathf.Clamp(pos.y, -3.2f, 2.5f);
        transform.position = pos;
    }
}