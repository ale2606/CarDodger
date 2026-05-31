using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VidasManager : MonoBehaviour
{
    public static VidasManager Instance;

    [SerializeField] public Image corazon1;
    [SerializeField] public Image corazon2;
    [SerializeField] public Image corazon3;

    [SerializeField] public Sprite corazonLleno;
    [SerializeField] public Sprite corazonVacio;

    public GameObject playerObj;

    int vidas = 3;
    bool invencible = false;

    void Awake() => Instance = this;

    void Start() => ActualizarCorazones();

    public void PerderVida()
    {
        if (GameManager.Instance.gameOver) return;
        if (invencible) return;

        vidas--;
        ActualizarCorazones();

        if (vidas <= 0)
        {
            GameManager.Instance.TriggerGameOver();
            return;
        }

        StartCoroutine(PausaYReposicion());
    }

    IEnumerator PausaYReposicion()
    {
        invencible = true;

        // Detener todo
        GameManager.Instance.gameOver = true;

        // Parpadear el jugador
        SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.15f);
        }
        sr.enabled = true;

        // Reposicionar al centro
        playerObj.transform.position = new Vector3(0, -2f, 0);

        // Reanudar juego
        GameManager.Instance.gameOver = false;

        yield return new WaitForSeconds(0.5f);
        invencible = false;
    }

    public void ReiniciarVidas()
    {
        vidas = 3;
        ActualizarCorazones();
    }

    void ActualizarCorazones()
    {
        corazon1.sprite = vidas >= 1 ? corazonLleno : corazonVacio;
        corazon2.sprite = vidas >= 2 ? corazonLleno : corazonVacio;
        corazon3.sprite = vidas >= 3 ? corazonLleno : corazonVacio;
    }
}