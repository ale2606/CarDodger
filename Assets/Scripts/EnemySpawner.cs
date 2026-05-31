using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Carriles Jugador 1")]
    public float[] lanesP1 = { -4.57f, -3.11f, -0.91f, 0.72f };
    public float spawnYP1 = 8f;

    [Header("Carriles Jugador 2")]
    public float[] lanesP2 = { 6.49f, 7.95f, 9.64f, 11.16f };
    public float spawnYP2 = 8f;

    [Header("Velocidad por nivel")]
    public float[] speedPerLevel = { 2f, 4f, 6.5f };

    [Header("Intervalo de spawn por nivel")]
    public float[] spawnIntervalPerLevel = { 3f, 2f, 1f };

    private int currentLevel = 1;
    private bool spawning = true;

    // Cooldown carriles
    private float[] cooldownP1;
    private float[] cooldownP2;

    private float laneCooldown = 1.2f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cooldownP1 = new float[lanesP1.Length];
        cooldownP2 = new float[lanesP2.Length];

        StartCoroutine(SpawnLoop(1));
        StartCoroutine(SpawnLoop(2));

        Debug.Log("Spawner iniciado");
    }

    void Update()
    {
        for (int i = 0; i < cooldownP1.Length; i++)
        {
            cooldownP1[i] =
                Mathf.Max(
                    0,
                    cooldownP1[i] - Time.deltaTime
                );
        }

        for (int i = 0; i < cooldownP2.Length; i++)
        {
            cooldownP2[i] =
                Mathf.Max(
                    0,
                    cooldownP2[i] - Time.deltaTime
                );
        }
    }

    IEnumerator SpawnLoop(int playerOwner)
    {
        float initialWait =
            playerOwner == 1 ? 2f : 3f;

        yield return new WaitForSeconds(initialWait);

        while (spawning)
        {
            CarController player =
                RaceGameManager.Instance.GetPlayer(playerOwner);

            if (player != null && !player.isAlive)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // TIEMPO ENTRE SPAWNS
            float baseInterval =
                spawnIntervalPerLevel[currentLevel - 1];

            float variacion =
                Random.Range(-0.2f, 0.2f);

            // ENTRE MÁS NIVEL MÁS RÁPIDO APARECEN
            float waitTime =
                Mathf.Max(
                    0.3f,
                    baseInterval + variacion
                );

            yield return new WaitForSeconds(waitTime);

            if (spawning)
            {
                SpawnForPlayer(playerOwner);
            }
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    void SpawnForPlayer(int playerOwner)
    {
        float[] lanes =
            playerOwner == 1
            ? lanesP1
            : lanesP2;

        float[] cooldowns =
            playerOwner == 1
            ? cooldownP1
            : cooldownP2;

        float spawnY =
            playerOwner == 1
            ? spawnYP1
            : spawnYP2;

        List<int> available =
            new List<int>();

        // BUSCAR CARRILES DISPONIBLES
        for (int i = 0; i < lanes.Length; i++)
        {
            if (cooldowns[i] <= 0f)
            {
                available.Add(i);
            }
        }

        if (available.Count == 0)
            return;

        // ELEGIR CARRIL
        int laneIdx =
            available[
                Random.Range(
                    0,
                    available.Count
                )
            ];

        cooldowns[laneIdx] = laneCooldown;

        // ELEGIR PREFAB
        GameObject prefab =
            enemyPrefabs[
                Random.Range(
                    0,
                    enemyPrefabs.Length
                )
            ];

        Vector3 spawnPos =
            new Vector3(
                lanes[laneIdx],
                spawnY,
                0f
            );

        GameObject car =
            Instantiate(
                prefab,
                spawnPos,
                Quaternion.identity
            );

        car.transform.localScale =
            new Vector3(
                0.4212368f,
                0.4426252f,
                1f
            );

        EnemyCar ec =
            car.GetComponent<EnemyCar>();

        if (ec != null)
        {
            // VELOCIDAD SEGÚN NIVEL
            float velocidadActual =
                speedPerLevel[currentLevel - 1];

            ec.speed = velocidadActual;

            ec.laneIndex = laneIdx;
            ec.playerOwner = playerOwner;
            ec.fixedX = lanes[laneIdx];

            Debug.Log(
                "Enemy Spawned | Nivel: " +
                currentLevel +
                " | Speed: " +
                velocidadActual
            );
        }
    }

    // CAMBIAR NIVEL
    public void SetLevel(int level)
    {
        currentLevel = level;

        // EVITAR ERRORES
        if (currentLevel < 1)
            currentLevel = 1;

        if (currentLevel > speedPerLevel.Length)
            currentLevel = speedPerLevel.Length;

        float nuevaVelocidad =
            speedPerLevel[currentLevel - 1];

        // ACTUALIZAR AUTOS EXISTENTES
        EnemyCar[] todosLosAutos =
            FindObjectsByType<EnemyCar>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );

        foreach (EnemyCar auto in todosLosAutos)
        {
            auto.UpdateSpeed(nuevaVelocidad);
        }

        Debug.Log(
            "==============="
        );

        Debug.Log(
            "NIVEL CAMBIADO"
        );

        Debug.Log(
            "Nivel actual: " +
            currentLevel
        );

        Debug.Log(
            "Nueva velocidad: " +
            nuevaVelocidad
        );

        Debug.Log(
            "==============="
        );
    }

    public void OnCarPassed(int playerOwner)
    {
        RaceGameManager.Instance.OnCarPassed(playerOwner);
    }

    public void StopSpawning()
    {
        spawning = false;
    }
}