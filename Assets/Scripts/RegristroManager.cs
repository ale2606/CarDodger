using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RegristroManager : MonoBehaviour
{

    [Header("Sonido")]
    public AudioClip sonidoBoton;
    private AudioSource audioSource;
    
    [SerializeField] public TMP_InputField inputNombre;
    [SerializeField] public Button[] botonesAvatar;
    [SerializeField] public Sprite[] avatarSprites;
    [SerializeField] public Image imgAvatarSeleccionado;
    [SerializeField] public GameObject errorTxt;
    [SerializeField] public bool esJugador2 = false;

    int avatarSeleccionado = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Conectar botones por codigo
        for (int i = 0; i < botonesAvatar.Length; i++)
        {
            int index = i;
            botonesAvatar[i].onClick.AddListener(() => SeleccionarAvatar(index));
        }

        ResaltarAvatar(0);
        if (avatarSprites.Length > 0 && imgAvatarSeleccionado != null)
            imgAvatarSeleccionado.sprite = avatarSprites[0];
    }
    void ReproducirSonido()
    {
        if (audioSource != null && sonidoBoton != null)
            audioSource.PlayOneShot(sonidoBoton);
    }
    public void SeleccionarAvatar(int index)
    {
        ReproducirSonido();
        avatarSeleccionado = index;
        ResaltarAvatar(index);
        if (index < avatarSprites.Length && imgAvatarSeleccionado != null)
            imgAvatarSeleccionado.sprite = avatarSprites[index];
    }

    void ResaltarAvatar(int index)
    {
        for (int i = 0; i < botonesAvatar.Length; i++)
        {
            var img = botonesAvatar[i].GetComponent<Image>();
            img.color = (i == index) ? Color.red : Color.white;
        }
    }

    public void Continuar()
    {
        ReproducirSonido();
        string nombre = inputNombre.text.Trim();

        if (nombre.Length < 2)
        {
            if (errorTxt) errorTxt.SetActive(true);
            return;
        }

        if (DataManager.Instance != null)
        {
            if (esJugador2)
            {
                DataManager.Instance.data.jugador2.nombre = nombre;
                DataManager.Instance.data.jugador2.avatarIndex = avatarSeleccionado;
            }
            else
            {
                DataManager.Instance.data.jugador1.nombre = nombre;
                DataManager.Instance.data.jugador1.avatarIndex = avatarSeleccionado;
            }
            DataManager.Instance.Save();
        }

        SceneManager.LoadScene(esJugador2 ? "Game2P" : "MenuSeleccion");
    }
}