using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class PortadaManager : MonoBehaviour
{
   

    [Header("Sonido")]
    public AudioClip sonidoBoton;
    private AudioSource audioSource;

    [Header("Luces de largada")]
    public Image[] luces;               // arrastrar Luz1…Luz5
    public Color   colorLuzApagada  = new Color(0.10f, 0f,   0.02f, 1f);
    public Color   colorLuzEncendida = new Color(0.91f, 0f,  0.11f, 1f);
    public float   intervaloLuz     = 0.5f;

    [Header("Carro central")]
    public RectTransform carroRect;     // ImgCarroCentral
    public float         floatAmp      = 10f;
    public float         floatSpeed    = 2f;

    [Header("Carros de fondo")]
    public Image[] carrosFondo;         // CarroTL, TR, BL, BR
    public float   pulseFondoSpeed     = 1f;

    [Header("Glitch del título")]
    public RectTransform tituloF1;      // TxtF1 RectTransform
    public float         glitchInterval = 6f;
    public float         glitchDuration = 0.18f;

    [Header("Parpadeo INSERT COIN")]
    public TextMeshProUGUI txtInsertCoin;
    public float           blinkInterval = 0.6f;

    [Header("Botón")]
    public Button btnIniciar;
    public string escenaMenu = "Registro";  // nombre de la Escena 1

    [Header("Transición")]
    public Image panelFlash;            // Image negro/blanco que cubre todo
    public float flashDuration = 0.4f;

    // ── internos ──────────────────────────────────────────────────────────
    float _time;
    float _glitchTimer;
    bool  _transitioning;
    float _carroBaseY;
    // ── Unity ─────────────────────────────────────────────────────────────
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Inicializar luces apagadas
        foreach (var l in luces)
            l.color = colorLuzApagada;

        // Ocultar flash
        if (panelFlash) panelFlash.color = Color.clear;

        // Conectar botón
        btnIniciar.onClick.AddListener(OnIniciar);

        // Iniciar secuencias
        StartCoroutine(SecuenciaLuces());
        StartCoroutine(ParpadeoInsertCoin());
        if (carroRect) _carroBaseY = carroRect.anchoredPosition.y;

        
    }

    void ReproducirSonido()
    {
        Debug.Log("AudioSource: " + audioSource + " Clip: " + sonidoBoton);
        if (audioSource != null && sonidoBoton != null)
            audioSource.PlayOneShot(sonidoBoton);
    }

    void Update()
    {
        if (_transitioning) return;
        _time += Time.deltaTime;

        AnimarCarroCentral();
        AnimarCarrosFondo();
        AnimarGlitch();
    }

    // ── Animaciones ───────────────────────────────────────────────────────

    // Flotación suave del carro central (equivale a @keyframes heroFloat)
    void AnimarCarroCentral()
    {
        if (!carroRect) return;
        float offsetY = Mathf.Sin(_time * floatSpeed * Mathf.PI * 2f) * floatAmp;
        Vector2 pos = carroRect.anchoredPosition;
        pos.y = _carroBaseY + offsetY;
        carroRect.anchoredPosition = pos;
    }

    // Pulsación de brillo en carros de fondo (equivale a @keyframes carDrift)
    void AnimarCarrosFondo()
    {
        if (carrosFondo == null) return;
        for (int i = 0; i < carrosFondo.Length; i++)
        {
            if (!carrosFondo[i]) continue;
            float pulse = 0.15f + 0.08f *
                Mathf.Sin((_time + i * 2f) * pulseFondoSpeed * Mathf.PI * 2f);
            Color c = carrosFondo[i].color;
            c.a = pulse;
            carrosFondo[i].color = c;
        }
    }

    // Glitch del título: desplaza el RectTransform brevemente
    // (equivale a @keyframes glitch con translate + text-shadow)
    void AnimarGlitch()
    {
        if (!tituloF1) return;
        _glitchTimer += Time.deltaTime;

        if (_glitchTimer >= glitchInterval)
        {
            _glitchTimer = 0f;
            StartCoroutine(EjecutarGlitch());
        }
    }

    IEnumerator EjecutarGlitch()
    {
        Vector2 original = tituloF1.anchoredPosition;

        // Frame 1: desplazar +3, -1
        tituloF1.anchoredPosition = original + new Vector2(3f, -1f);
        yield return new WaitForSeconds(glitchDuration * 0.33f);

        // Frame 2: desplazar -2, +1
        tituloF1.anchoredPosition = original + new Vector2(-2f, 1f);
        yield return new WaitForSeconds(glitchDuration * 0.33f);

        // Frame 3: volver
        tituloF1.anchoredPosition = original;
    }

    // ── Corrutinas ────────────────────────────────────────────────────────

    // Enciende las luces de una en una cada 0.5s (equivale a setInterval de JS)
    IEnumerator SecuenciaLuces()
    {
        yield return new WaitForSeconds(0.8f);   // espera inicial
        foreach (var luz in luces)
        {
            luz.color = colorLuzEncendida;
            yield return new WaitForSeconds(intervaloLuz);
        }
    }

    // Parpadeo de "INSERT COIN" (equivale a @keyframes blink)
    IEnumerator ParpadeoInsertCoin()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            if (txtInsertCoin)
                txtInsertCoin.alpha = txtInsertCoin.alpha > 0.5f ? 0f : 1f;
        }
    }

    // ── Botón INICIAR ─────────────────────────────────────────────────────


    void OnIniciar()
    {
        if (_transitioning) return;
        _transitioning = true;
        ReproducirSonido();
        StartCoroutine(TransicionFlash());
    }

    // Flash blanco pixelado y carga la siguiente escena
    // (equivale a goToMenu() con steps(4) en JS)
    IEnumerator TransicionFlash()
    {
        if (!panelFlash)
        {
            yield return new WaitForSeconds(0.3f);
            CargarMenu();
            yield break;
        }

        int pasos = 4;
        for (int i = pasos; i >= 0; i--)
        {
            panelFlash.color = new Color(1f, 1f, 1f, (float)i / pasos);
            yield return new WaitForSeconds(flashDuration / pasos);
        }
        CargarMenu();
    }

    void CargarMenu() => SceneManager.LoadScene(escenaMenu);
}
