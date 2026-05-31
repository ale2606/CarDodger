using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int coches = 0;
    public int nivel = 0;
    public bool gameOver = false;
    public GameObject explosionPrefab;

    int highScore = 0;
    string scorePath;
    string historyPath;

    public TMPro.TextMeshProUGUI txtScore;
    public TMPro.TextMeshProUGUI txtNivel;
    public TMPro.TextMeshProUGUI txtBest;
    public GameObject panelGameOver;
    public AudioSource musicSource;
    public AudioClip gameOverClip;

    void Awake()
    {
        Instance = this;
        scorePath = Application.persistentDataPath + "/Score.txt";
        historyPath = Application.persistentDataPath + "/Historial.txt";
        highScore = LoadHighScore();

        if (MusicaManager.Instance != null)
            MusicaManager.Instance.PausarMusica();
    }

    void Update()
    {
        if (gameOver) return;

        nivel = Mathf.Min(coches / 20, 1);

        if (txtScore) txtScore.text = "Score : " + score;
        if (txtNivel) txtNivel.text = "Nivel : " + (nivel + 1);
        if (txtBest) txtBest.text = "Best  : " + highScore;

        if (score > highScore)
        {
            highScore = score;
            SaveHighScore(highScore);
        }
    }

    public float GetSpeed()
    {
        float[] speeds = { 4f, 7f, 11f };
        return speeds[nivel];
    }

    public void AddScore(int amount)
    {
        score += amount;
        coches += 1;
    }

    public void DetenerJuego()
    {
        foreach (var car in GameObject.FindGameObjectsWithTag("EnemyCar"))
        {
            var oc = car.GetComponent<OncomingCar>();
            if (oc != null) oc.speed = 0f;
        }
        foreach (var sign in GameObject.FindGameObjectsWithTag("Sign"))
        {
            var fs = sign.GetComponent<FixedSign>();
            if (fs != null) fs.Detener();
        }
    }

    public void TriggerGameOver()
    {
        if (gameOver) return;
        gameOver = true;
        DetenerJuego();

        // Guardar datos del jugador
        if (DataManager.Instance != null)
        {
            DataManager.Instance.data.jugador1.highScore = highScore;
            DataManager.Instance.data.jugador1.nivel = nivel + 1;
            DataManager.Instance.Save();
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null && explosionPrefab != null)
        {
            Vector3 posJugador = jugador.transform.position;
            Vector3 posChoque = posJugador;
            float distMin = float.MaxValue;

            foreach (var car in GameObject.FindGameObjectsWithTag("EnemyCar"))
            {
                float dist = Vector3.Distance(posJugador, car.transform.position);
                if (dist < distMin)
                {
                    distMin = dist;
                    posChoque = car.transform.position;
                }
            }
            foreach (var sign in GameObject.FindGameObjectsWithTag("Sign"))
            {
                float dist = Vector3.Distance(posJugador, sign.transform.position);
                if (dist < distMin)
                {
                    distMin = dist;
                    posChoque = sign.transform.position;
                }
            }

            Vector3 puntoMedio = (posJugador + posChoque) / 2f;
            GameObject exp = Instantiate(explosionPrefab, puntoMedio, Quaternion.identity);
            Destroy(exp.GetComponent<Explosion>());
        }
        if (MusicaManager.Instance != null)
            MusicaManager.Instance.ReanudarMusica();

        if (musicSource) musicSource.Stop();
        if (gameOverClip)
            AudioSource.PlayClipAtPoint(gameOverClip, Vector3.zero);

        SaveHistory(score, nivel + 1);
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore(highScore);
        }
        PlayerPrefs.SetInt("ModoJuego", 1);
        PlayerPrefs.Save();

        StartCoroutine(IrAGameOver());

        if (DataManager.Instance != null)
        {
            Debug.Log("Guardando datos: " + DataManager.Instance.data.jugador1.nombre + " score: " + score);
            DataManager.Instance.data.jugador1.highScore = highScore;
            DataManager.Instance.data.jugador1.nivel = nivel + 1;
            DataManager.Instance.data.jugador1.score = score;
            DataManager.Instance.AddRecord1P(
                DataManager.Instance.data.jugador1.nombre,
                score,
                nivel + 1,
                DataManager.Instance.data.jugador1.avatarIndex
            );
        }
        else
        {
            Debug.Log("DataManager es NULL");
        }
    }

    IEnumerator IrAGameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOver1P");
    }

    int LoadHighScore()
    {
        try
        {
            if (File.Exists(scorePath))
                return int.Parse(File.ReadAllText(scorePath).Trim());
        }
        catch { }
        return 0;
    }

    void SaveHighScore(int s) =>
        File.WriteAllText(scorePath, s.ToString());

    void SaveHistory(int s, int niv)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  |  " +
                      $"Score: {s,6}  |  Nivel: {niv}\n";
        File.AppendAllText(historyPath, line);
    }
}