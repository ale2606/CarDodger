using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Registro2PManager : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoBoton;
    private AudioSource audioSource;

    [Header("Panel J1 — solo muestra datos")]
    [SerializeField] public TextMeshProUGUI txtNombreJ1;
    [SerializeField] public Image imgAvatarJ1;

    [Header("Panel J2 — ingresa datos")]
    [SerializeField] public TMP_InputField inputNombreJ2;
    [SerializeField] public Image imgAvatarJ2;
    [SerializeField] public Button[] botonesAvatar;
    [SerializeField] public GameObject errorTxt;

    [Header("Sprites")]
    [SerializeField] public Sprite[] avatarSprites;

    int avatarSeleccionado = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Limpiar listeners anteriores
        for (int i = 0; i < botonesAvatar.Length; i++)
        {
            botonesAvatar[i].onClick.RemoveAllListeners();
        }

        // Conectar botones por codigo
        for (int i = 0; i < botonesAvatar.Length; i++)
        {
            int index = i;
            botonesAvatar[i].onClick.AddListener(() => SeleccionarAvatar(index));
        }

        if (DataManager.Instance == null) return;

        PlayerData j1 = DataManager.Instance.data.jugador1;
        txtNombreJ1.text = j1.nombre != "" ? j1.nombre : "JUGADOR 1";
        if (avatarSprites.Length > j1.avatarIndex)
            imgAvatarJ1.sprite = avatarSprites[j1.avatarIndex];

        ResaltarAvatar(0);
        if (avatarSprites.Length > 0)
            imgAvatarJ2.sprite = avatarSprites[0];
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
        Debug.Log("Avatar seleccionado: " + index);
        ResaltarAvatar(index);
        if (index < avatarSprites.Length && imgAvatarJ2 != null)
            imgAvatarJ2.sprite = avatarSprites[index];
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
        string nombre = inputNombreJ2.text.Trim();

        if (nombre.Length < 2)
        {
            if (errorTxt) errorTxt.SetActive(true);
            return;
        }

        if (DataManager.Instance != null)
        {
            DataManager.Instance.data.jugador2.nombre = nombre;
            DataManager.Instance.data.jugador2.avatarIndex = avatarSeleccionado;
            DataManager.Instance.Save();

            Debug.Log("Guardando J2 avatar: " + avatarSeleccionado);
        }

        SceneManager.LoadScene("Game2P");
    }
}