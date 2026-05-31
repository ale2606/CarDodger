using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Tamaño")]
    public Vector3 tamanioFijo = new Vector3(0.18f, 0.19f, 1f);

    [Header("Configuracion")]
    public int playerNumber = 1;
    public float moveSpeedH = 5f;
    public float moveSpeedV = 4f;

    [Header("Limites")]
    public float minX = -5.69f;
    public float maxX = 0.65f;
    public float minY = -3.2f;
    public float maxY = 2.5f;

    [Header("Estado")]
    public bool isAlive = true;
    public int score = 0;


    public Sprite[] spritesCarros;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Cargar carro elegido en el menu
        int carroIndex = playerNumber == 1
            ? PlayerPrefs.GetInt("CarroP1", 0)
            : PlayerPrefs.GetInt("CarroP2", 3);

        if (spritesCarros != null && spritesCarros.Length > carroIndex)
            spriteRenderer.sprite = spritesCarros[carroIndex];

        // Normalizar tamaño basado en píxeles del sprite
        if (spriteRenderer.sprite != null)
        {
            float anchoSprite = spriteRenderer.sprite.rect.width;
            float tamanoBase = 500f;
            float factorNormalizacion = tamanoBase / anchoSprite;

            // Limitar el factor para que nunca sea muy pequeño
            factorNormalizacion = Mathf.Clamp(factorNormalizacion, 0.1f, 2f);

            transform.localScale = new Vector3(
                tamanioFijo.x * factorNormalizacion,
                tamanioFijo.y * factorNormalizacion,
                1f
            );

            Debug.Log($"Sprite: {spriteRenderer.sprite.name} Ancho: {anchoSprite} Factor: {factorNormalizacion} Scale: {transform.localScale}");
        }
        else
        {
            transform.localScale = tamanioFijo;
        }
    }
    void Update()
    {
        if (!isAlive) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;

        if (playerNumber == 1)
        {
            if (kb.aKey.isPressed) h = -1f;
            if (kb.dKey.isPressed) h = 1f;
            if (kb.wKey.isPressed) v = 1f;
            if (kb.sKey.isPressed) v = -1f;
        }
        else
        {
            if (kb.leftArrowKey.isPressed) h = -1f;
            if (kb.rightArrowKey.isPressed) h = 1f;
            if (kb.upArrowKey.isPressed) v = 1f;
            if (kb.downArrowKey.isPressed) v = -1f;
        }

        Vector3 pos = transform.position;
        pos.x += h * moveSpeedH * Time.deltaTime;
        pos.y += v * moveSpeedV * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    public void AddScore(int points)
    {
        if (!isAlive) return;
        score += points;
        RaceGameManager.Instance.UpdateUI();
    }

    public void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 0.6f);
        RaceGameManager.Instance.OnPlayerDied(playerNumber);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyCar"))
            Die();
    }
}