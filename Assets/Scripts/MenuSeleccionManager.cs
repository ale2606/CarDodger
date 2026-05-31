using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuSeleccionManager : MonoBehaviour
{
    // ── Datos de carros ───────────────────────────────────────────────────
    [System.Serializable]
    public struct DatosCarro
    {
        public string nombre;
        public Sprite sprite;
        public Color  colorEquipo;
    }

    [Header("Sonido")]
    public AudioClip sonidoBoton;

    [Header("Carros (arrastrar en orden)")]
    public DatosCarro[] carros;   // 8 entradas: Ferrari … Cadillac

    // ── Desfile ───────────────────────────────────────────────────────────
    [Header("Desfile")]
    public RectTransform contenedorDesfile;
    public float         velocidadDesfile = 120f;
    float                _anchoDesfile;

    // ── Luces ─────────────────────────────────────────────────────────────
    [Header("Luces de largada")]
    public Image[] luces;
    public Color   colorApagada   = new Color(0.10f, 0f, 0.02f, 1f);
    public Color   colorEncendida = new Color(0.91f, 0f, 0.11f, 1f);
    public float   intervaloLuz   = 0.5f;

    // ── Modo ──────────────────────────────────────────────────────────────
    [Header("Botones de modo")]
    public Button btn1Player;
    public Button btn2Players;
    public Color  colorBtnActivo   = new Color(0.7f, 0f, 0.07f, 0.9f);
    public Color  colorBtnInactivo = new Color(0.08f, 0.02f, 0.03f, 0.9f);

    // ── Slots ─────────────────────────────────────────────────────────────
    [Header("Slot P1")]
    public GameObject    slotP1;
    public Image         imgCarroP1;
    public TextMeshProUGUI txtNombreP1;
    public Button        btnP1Prev;
    public Button        btnP1Next;

    [Header("Slot P2")]
    public GameObject    slotP2;
    public Image         imgCarroP2;
    public TextMeshProUGUI txtNombreP2;
    public Button        btnP2Prev;
    public Button        btnP2Next;

    [Header("VS badge")]
    public GameObject    objVS;
    public TextMeshProUGUI txtVS;

    // ── Start ─────────────────────────────────────────────────────────────
    [Header("Botón Start")]
    public Button          btnStart;
    public TextMeshProUGUI txtHint;
    public string          escenaJuego = "Juego";   // nombre de tu escena principal

    // ── Flash ─────────────────────────────────────────────────────────────
    [Header("Transición")]
    public Image panelFlash;
    public float flashDuration = 0.4f;

    // ── Estado ────────────────────────────────────────────────────────────
    int   _modo     = 0;          // 0=ninguno 1=1P 2=2P
    int[] _selected = { 0, 3 };   // índice del carro por jugador
    float _time;
    bool  _transitioning;

    // ── Unity ─────────────────────────────────────────────────────────────

    void ReproducirSonido()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && sonidoBoton != null)
            audio.PlayOneShot(sonidoBoton);
    }

    void Start()
    {
        // Luces
        foreach (var l in luces) l.color = colorApagada;
        StartCoroutine(SecuenciaLuces());

        // Flash oculto
        if (panelFlash) panelFlash.color = Color.clear;

        // Slots ocultos al inicio
        slotP1.SetActive(false);
        slotP2.SetActive(false);
        if (objVS) objVS.SetActive(false);
        btnStart.interactable = false;

        // Botones de modo
        btn1Player.onClick.AddListener(() => SeleccionarModo(1));
        btn2Players.onClick.AddListener(() => SeleccionarModo(2));

        // Botones de carro P1
        btnP1Prev.onClick.AddListener(() => CambiarCarro(0, -1));
        btnP1Next.onClick.AddListener(() => CambiarCarro(0, +1));

        // Botones de carro P2
        btnP2Prev.onClick.AddListener(() => CambiarCarro(1, -1));
        btnP2Next.onClick.AddListener(() => CambiarCarro(1, +1));

        // Botón start
        btnStart.onClick.AddListener(OnStart);

        // Desfile: guardar ancho total para el loop
        if (contenedorDesfile)
            _anchoDesfile = contenedorDesfile.rect.width * 0.5f;

        // Actualizar estado inicial
        ActualizarColorBotonesModo();

        // Hint inicial
        if (txtHint) txtHint.text = ">> SELECCIONA UN MODO PARA CONTINUAR <<";

        ActualizarSlot(0);
        ActualizarSlot(1);
    }

    void Update()
    {
        if (_transitioning) return;
        _time += Time.deltaTime;

        AnimarDesfile();
        AnimarVS();

        // Teclado P1
        if (_modo >= 1)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)  ||
                Input.GetKeyDown(KeyCode.A))
                CambiarCarro(0, -1);
            if (Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.D))
                CambiarCarro(0, +1);
        }
        // Teclado P2
        if (_modo == 2)
        {
            if (Input.GetKeyDown(KeyCode.J)) CambiarCarro(1, -1);
            if (Input.GetKeyDown(KeyCode.L)) CambiarCarro(1, +1);
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space))
            if (_modo >= 1) OnStart();
    }

    // ── Animaciones ───────────────────────────────────────────────────────

    // Mueve el contenedor del desfile hacia la izquierda en loop
    // (equivale a @keyframes parade con translateX(-50%))
    void AnimarDesfile()
    {
        
    }

    // Pulsación del texto VS (equivale al sinf en el JS del HTML)
    void AnimarVS()
    {
        if (!txtVS) return;
        float pulse = 0.85f + 0.15f * Mathf.Sin(_time * 3f);
        txtVS.color = new Color(0.91f * pulse, 0f, 0.11f * pulse, 1f);
    }

    // ── Lógica de selección ───────────────────────────────────────────────

    public void SeleccionarModo(int modo)
    {
        ReproducirSonido();
        Debug.Log("Modo seleccionado: " + modo);
        _modo = modo;

        slotP1.SetActive(true);
        ActualizarSlot(0);

        bool es2P = modo == 2;
        slotP2.SetActive(es2P);
        if (objVS) objVS.SetActive(es2P);
        if (es2P) ActualizarSlot(1);

        btnStart.interactable = true;
        ActualizarColorBotonesModo();

        if (txtHint)
            txtHint.text = _modo == 1
                ? "A / D para cambiar carro | ENTER para iniciar"
                : "P1: A/D   P2: J/L para cambiar | ENTER para iniciar";
    }

    // Cambia el carro seleccionado con módulo para hacer loop
    // (equivale a (selected + 1) % CAR_COUNT en JS)
    public void CambiarCarro(int jugador, int delta)
    {
        _selected[jugador] =
            (_selected[jugador] + delta + carros.Length) % carros.Length;
        Debug.Log("Carro seleccionado: " + _selected[jugador] + " de " + carros.Length);
        ActualizarSlot(jugador);
    }

    void ActualizarSlot(int jugador)
    {
        if (carros == null || carros.Length == 0) return;
        var c = carros[_selected[jugador]];

        if (jugador == 0)
        {
            if (imgCarroP1)   imgCarroP1.sprite = c.sprite;
            if (txtNombreP1) { txtNombreP1.text = c.nombre; txtNombreP1.color = Color.white; }
        }
        else
        {
            if (imgCarroP2)   imgCarroP2.sprite = c.sprite;
            if (txtNombreP2) { txtNombreP2.text = c.nombre; txtNombreP2.color = Color.white; }
        }
    }

    void ActualizarColorBotonesModo()
    {
        SetColorBoton(btn1Player,  _modo == 1);
        SetColorBoton(btn2Players, _modo == 2);
    }

    void SetColorBoton(Button btn, bool activo)
    {
        var img = btn.GetComponent<Image>();
        if (img) img.color = activo ? colorBtnActivo : colorBtnInactivo;
    }

    // ── Start ─────────────────────────────────────────────────────────────

    public void OnStart()
    {
        ReproducirSonido();
        if (_transitioning || _modo == 0) return;
        _transitioning = true;

        // Guardar elecciones para la escena del juego
        PlayerPrefs.SetInt("Jugadores", _modo);
        PlayerPrefs.SetInt("CarroP1",   _selected[0]);
        PlayerPrefs.SetInt("CarroP2",   _selected[1]);
        PlayerPrefs.Save();

        StartCoroutine(TransicionFlash());
    }

    IEnumerator TransicionFlash()
    {
        if (!panelFlash) { Cargar(); yield break; }
        int pasos = 4;
        for (int i = pasos; i >= 0; i--)
        {
            panelFlash.color = new Color(1f, 1f, 1f, (float)i / pasos);
            yield return new WaitForSeconds(flashDuration / pasos);
        }
        Cargar();
    }

    void Cargar()
    {
        if (_modo == 1)
            SceneManager.LoadScene("Game");
        else
            SceneManager.LoadScene("Registro2P");
    }

    // ── Corrutinas ────────────────────────────────────────────────────────

    IEnumerator SecuenciaLuces()
    {
        yield return new WaitForSeconds(0.8f);
        foreach (var luz in luces)
        {
            luz.color = colorEncendida;
            yield return new WaitForSeconds(intervaloLuz);
        }
    }

    public void P1Prev() { CambiarCarro(0, -1); }
    public void P1Next() { CambiarCarro(0, +1); }
    public void P2Prev() { CambiarCarro(1, -1); }
    public void P2Next() { CambiarCarro(1, +1); }
}
