using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PodioManager : MonoBehaviour
{

    [Header("Sonido")]
    public AudioClip sonidoBoton;
    private AudioSource audioSource;

    [Header("Ganador")]
    [SerializeField] TextMeshProUGUI txtNombreGanador;
    [SerializeField] TextMeshProUGUI txtScoreGanador;
    [SerializeField] Image imgAvatarGanador;

    [Header("Segundo lugar")]
    [SerializeField] TextMeshProUGUI txtNombreSegundo;
    [SerializeField] TextMeshProUGUI txtScoreSegundo;
    [SerializeField] Image imgAvatarSegundo;

    [Header("Sprites")]
    [SerializeField] Sprite[] avatarSprites;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (DataManager.Instance == null) return;

        PlayerData j1 = DataManager.Instance.data.jugador1;
        PlayerData j2 = DataManager.Instance.data.jugador2;

        PlayerData ganador, segundo;

        if (j1.score >= j2.score)
        {
            ganador = j1;
            segundo = j2;
        }
        else
        {
            ganador = j2;
            segundo = j1;
        }

        txtNombreGanador.text = ganador.nombre != "" ? ganador.nombre : "JUGADOR";
        txtScoreGanador.text = ganador.score + " pts";
        if (avatarSprites.Length > ganador.avatarIndex)
            imgAvatarGanador.sprite = avatarSprites[ganador.avatarIndex];

        txtNombreSegundo.text = segundo.nombre != "" ? segundo.nombre : "JUGADOR";
        txtScoreSegundo.text = segundo.score + " pts";
        if (avatarSprites.Length > segundo.avatarIndex)
            imgAvatarSegundo.sprite = avatarSprites[segundo.avatarIndex];

        DataManager.Instance.AddRecord2P(j1.nombre, j1.score, j1.nivel, j1.avatarIndex);
        DataManager.Instance.AddRecord2P(j2.nombre, j2.score, j2.nivel, j2.avatarIndex);
    }
    void ReproducirSonido()
    {
        if (audioSource != null && sonidoBoton != null)
            audioSource.PlayOneShot(sonidoBoton);
    }
    public void IrARecords()
    {
        ReproducirSonido();
        PlayerPrefs.SetInt("ModoJuego", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOver1P");
    }
    public void IrAMenu()
    {
        ReproducirSonido();
        SceneManager.LoadScene("MenuSeleccion");
    }
}