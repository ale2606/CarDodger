using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver1PManager : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoBoton;
    private AudioSource audioSource;

    [Header("Stats 1 jugador")]
    public GameObject panelStats1P;
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtScore;
    public TextMeshProUGUI txtNivel;
    public TextMeshProUGUI txtBest;

    [Header("Mejor jugador")]
    public TextMeshProUGUI txtNombreJugador;
    public Image imgMejorJugador;
    public Sprite[] avatarSprites;

    [Header("Tabla")]
    public TextMeshProUGUI[] filasTabla;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (DataManager.Instance == null)
        {
            Debug.Log("DataManager es NULL");
            return;
        }

        // Detectar modo de juego
        int modo = PlayerPrefs.GetInt("ModoJuego", 1);

        // Mostrar stats solo si viene del juego de 1 jugador
        if (panelStats1P != null)
            panelStats1P.SetActive(modo == 1);

        if (modo == 1)
        {
            PlayerData jugador = DataManager.Instance.data.jugador1;
            if (txtNombre) txtNombre.text = jugador.nombre != "" ? jugador.nombre : "JUGADOR";
            if (txtScore) txtScore.text = "SCORE : " + jugador.score;
            if (txtNivel) txtNivel.text = "NIVEL : " + jugador.nivel;
            if (txtBest) txtBest.text = "BEST  : " + jugador.highScore;
        }

        // Tabla top 5
        var top5 = DataManager.Instance.data.top5;
        for (int i = 0; i < filasTabla.Length; i++)
        {
            if (i < top5.Count)
                filasTabla[i].text = $"{i + 1}. {top5[i].nombre}   {top5[i].score} pts   {top5[i].modo}";
            else
                filasTabla[i].text = $"{i + 1}. ---";
        }

        // Mejor jugador
        if (top5.Count > 0)
        {
            if (txtNombreJugador != null)
                txtNombreJugador.text = top5[0].nombre;
            if (imgMejorJugador != null && avatarSprites.Length > top5[0].avatarIndex)
                imgMejorJugador.sprite = avatarSprites[top5[0].avatarIndex];
        }
    }
    void ReproducirSonido()
    {
        if (audioSource != null && sonidoBoton != null)
            audioSource.PlayOneShot(sonidoBoton);
    }
    public void JugarOtraVez()
    {
        ReproducirSonido();
        int modo = PlayerPrefs.GetInt("ModoJuego", 1);
        if (modo == 1)
            SceneManager.LoadScene("Game");
        else
            SceneManager.LoadScene("Game2P");
    }

    public void IrAlMenu()
    {
        ReproducirSonido();
        SceneManager.LoadScene("MenuSeleccion");
    }
}