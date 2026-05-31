using UnityEngine;

public class LeerEleccionJuego : MonoBehaviour
{
    // Índices de los carros (mismo orden que en MenuSeleccionManager.carros[])
    public const int FERRARI      = 0;
    public const int REDBULL      = 1;
    public const int MERCEDES     = 2;
    public const int MCLAREN      = 3;
    public const int RACINGBULLS  = 4;
    public const int ASTONMARTIN  = 5;
    public const int ALPINE       = 6;
    public const int CADILLAC     = 7;

    // ── Propiedades públicas ──────────────────────────────────────────────
    public int Jugadores => PlayerPrefs.GetInt("Jugadores", 1);
    public int CarroP1   => PlayerPrefs.GetInt("CarroP1",   0);
    public int CarroP2   => PlayerPrefs.GetInt("CarroP2",   3);

    // ── Ejemplo de uso en Start() ─────────────────────────────────────────
    void Start()
    {
        Debug.Log($"[LeerEleccion] Modo: {Jugadores}P | " +
                  $"P1: {NombreCarro(CarroP1)} | " +
                  $"P2: {NombreCarro(CarroP2)}");

        // Aquí tu compañera puede inicializar el juego según los valores:
        //
        //   if (Jugadores == 2) ActivarSegundoJugador();
        //   SpriteCarro(0).sprite = spritesCarros[CarroP1];
        //   SpriteCarro(1).sprite = spritesCarros[CarroP2];
    }

    public static string NombreCarro(int idx)
    {
        string[] nombres = {
            "Ferrari", "Red Bull", "Mercedes", "McLaren",
            "Racing Bulls", "Aston Martin", "Alpine", "Cadillac"
        };
        return idx >= 0 && idx < nombres.Length ? nombres[idx] : "?";
    }
}
