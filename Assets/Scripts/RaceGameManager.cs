using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RaceGameManager : MonoBehaviour
{
    public static RaceGameManager Instance;


    [Header("Nombres y Avatares")]
    public TextMeshProUGUI txtNombreJ1;
    public TextMeshProUGUI txtNombreJ2;
    public Image imgAvatarJ1;
    public Image imgAvatarJ2;
    public Sprite[] avatarSprites;


    [Header("Referencias de jugadores")]
    public CarController player1;
    public CarController player2;

    [Header("UI - Jugador 1")]
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI levelTextP1;
    public GameObject deathOverlayP1; // panel semitransparente "PERDISTE"

    [Header("UI - Jugador 2")]
    public TextMeshProUGUI scoreTextP2;
    public TextMeshProUGUI levelTextP2;
    public GameObject deathOverlayP2;

    [Header("UI General")]
    public TextMeshProUGUI gameOverText; // Cuando ambos pierden
    public TextMeshProUGUI levelAnnouncerText; // "¡NIVEL 2!" centrado

    [Header("Configuración de niveles")]
    public int carsToNextLevel = 20; // cada 20 autos pasan de nivel


    [Header("Fondos")]
    public RoadScroller BG_J1_A;
    public RoadScroller BG_J1_B;
    public RoadScroller BG_J2_A;
    public RoadScroller BG_J2_B;

    // Contadores de autos que pasaron por jugador
    private int carsPassed_P1 = 0;
    private int carsPassed_P2 = 0;

    private int levelP1 = 1;
    private int levelP2 = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (MusicaManager.Instance != null)
            MusicaManager.Instance.PausarMusica();

        if (deathOverlayP1) deathOverlayP1.SetActive(false);
        if (deathOverlayP2) deathOverlayP2.SetActive(false);
        if (gameOverText) gameOverText.gameObject.SetActive(false);
        if (levelAnnouncerText) levelAnnouncerText.gameObject.SetActive(false);
        // Mostrar nombre y avatar de cada jugador
        if (DataManager.Instance != null)
        {
            PlayerData j1 = DataManager.Instance.data.jugador1;
            PlayerData j2 = DataManager.Instance.data.jugador2;

            if (txtNombreJ1) txtNombreJ1.text = j1.nombre != "" ? j1.nombre : "J1";
            if (txtNombreJ2) txtNombreJ2.text = j2.nombre != "" ? j2.nombre : "J2";

            if (imgAvatarJ1 && avatarSprites.Length > j1.avatarIndex)
                imgAvatarJ1.sprite = avatarSprites[j1.avatarIndex];
            if (imgAvatarJ2 && avatarSprites.Length > j2.avatarIndex)
                imgAvatarJ2.sprite = avatarSprites[j2.avatarIndex];
        }

        if (DataManager.Instance != null)
            Debug.Log("J2 avatar index: " + DataManager.Instance.data.jugador2.avatarIndex);
        UpdateUI();
    }

    public CarController GetPlayer(int number)
    {
        return number == 1 ? player1 : player2;
    }

    /// <summary>
    /// Llamado desde EnemyCar cuando un auto pasa sin chocar.
    /// </summary>
    public void OnCarPassed(int playerOwner)
    {
        if (playerOwner == 1)
        {
            carsPassed_P1++;
            CheckLevelUp(1);
        }
        else
        {
            carsPassed_P2++;
            CheckLevelUp(2);
        }
    }

    void CheckLevelUp(int playerOwner)
    {
        int passed = playerOwner == 1 ? carsPassed_P1 : carsPassed_P2;
        int currentLevel = playerOwner == 1 ? levelP1 : levelP2;

        int targetLevel = 1;
        if (passed >= 40) targetLevel = 3;
        else if (passed >= 20) targetLevel = 2;

        Debug.Log($"J{playerOwner} passed:{passed} currentLevel:{currentLevel} targetLevel:{targetLevel} globalLevel antes:{Mathf.Max(levelP1, levelP2)}");

        if (targetLevel > currentLevel)
        {
            if (playerOwner == 1) levelP1 = targetLevel;
            else levelP2 = targetLevel;

            int globalLevel = 1;
            if (player1.isAlive && player2.isAlive)
                globalLevel = Mathf.Max(levelP1, levelP2);
            else if (player1.isAlive)
                globalLevel = levelP1;
            else
                globalLevel = levelP2;

            Debug.Log($"SetLevel llamado con: {globalLevel}");
            EnemySpawner.Instance.SetLevel(globalLevel);

            float velocidadFondo = globalLevel == 1 ? 3f : globalLevel == 2 ? 6f : 10f;
            if (BG_J1_A != null) BG_J1_A.SetSpeed(velocidadFondo);
            if (BG_J1_B != null) BG_J1_B.SetSpeed(velocidadFondo);
            if (BG_J2_A != null) BG_J2_A.SetSpeed(velocidadFondo);
            if (BG_J2_B != null) BG_J2_B.SetSpeed(velocidadFondo);

            ShowLevelAnnouncement(playerOwner, targetLevel);
            UpdateUI();
        }
    }

    void ShowLevelAnnouncement(int playerOwner, int level)
    {
        if (levelAnnouncerText == null) return;
        levelAnnouncerText.gameObject.SetActive(true);
        levelAnnouncerText.text = $"¡JUGADOR {playerOwner} - NIVEL {level}!";
        Invoke(nameof(HideLevelAnnouncer), 2f);
    }

    void HideLevelAnnouncer()
    {
        if (levelAnnouncerText) levelAnnouncerText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Llamado desde CarController cuando un jugador muere.
    /// </summary>
    public void OnPlayerDied(int playerNumber)
    {
        if (playerNumber == 1 && deathOverlayP1)
            deathOverlayP1.SetActive(true);

        if (playerNumber == 2 && deathOverlayP2)
            deathOverlayP2.SetActive(true);

        // Pausar todos los enemigos del jugador que murió
        EnemyCar[] allEnemies = FindObjectsByType<EnemyCar>(FindObjectsInactive.Exclude);
        foreach (EnemyCar enemy in allEnemies)
        {
            if (enemy.playerOwner == playerNumber)
                enemy.PauseThisCar();
        }
        if (playerNumber == 1)
        {
            if (BG_J1_A != null) BG_J1_A.PauseScroll();
            if (BG_J1_B != null) BG_J1_B.PauseScroll();
        }
        else
        {
            if (BG_J2_A != null) BG_J2_A.PauseScroll();
            if (BG_J2_B != null) BG_J2_B.PauseScroll();
        }

        bool bothDead = !player1.isAlive && !player2.isAlive;
        if (bothDead)
        {
            OnGameOver();
        }
    }

    void OnGameOver()
    {
        if (MusicaManager.Instance != null)
            MusicaManager.Instance.ReanudarMusica();

        EnemySpawner.Instance.StopSpawning();

        // Guardar scores en DataManager
        if (DataManager.Instance != null)
        {
            DataManager.Instance.data.jugador1.score = player1.score;
            DataManager.Instance.data.jugador2.score = player2.score;
            DataManager.Instance.Save();
        }

        StartCoroutine(IrAlPodio());
    }

    System.Collections.IEnumerator IrAlPodio()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Podio");
    }

    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (scoreTextP1) scoreTextP1.text = $"Score: {player1.score}";
        if (scoreTextP2) scoreTextP2.text = $"Score: {player2.score}";
        if (levelTextP1) levelTextP1.text = $"Nivel {levelP1}";
        if (levelTextP2) levelTextP2.text = $"Nivel {levelP2}";
    }
}