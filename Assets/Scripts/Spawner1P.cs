using UnityEngine;

public class Spawner1P : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public GameObject[] signPrefabs;

    // 4 carriles con distribucion uniforme
    float[] carriles = { -1.46f, -0.63f, 0.64f, 1.44f };

    float carTimer = 0f;
    float signTimer = 0f;

    int GetNivel() => GameManager.Instance != null ? GameManager.Instance.nivel : 0;
    float GetCarInterval() => Mathf.Max(0.6f, 1.8f - GetNivel() * 0.4f);
    float GetSignInterval() => Mathf.Max(1.2f, 3.0f - GetNivel() * 0.6f);
   
    int MaxCars()
    {
        int[] maxPorNivel = { 2, 4, 6 };
        return maxPorNivel[Mathf.Clamp(GetNivel(), 0, 2)];
    }
    int MaxSigns() => GetNivel() == 0 ? 1 : 2;

    // Distribucion uniforme — Fisher-Yates
    float GetCarrilLibre()
    {
        float[] mezclados = (float[])carriles.Clone();
        for (int i = mezclados.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            float tmp = mezclados[i];
            mezclados[i] = mezclados[j];
            mezclados[j] = tmp;
        }

        foreach (float x in mezclados)
        {
            bool ocupado = false;

            foreach (var car in GameObject.FindGameObjectsWithTag("EnemyCar"))
                if (Mathf.Abs(car.transform.position.x - x) < 0.5f)
                { ocupado = true; break; }

            if (!ocupado)
                foreach (var sign in GameObject.FindGameObjectsWithTag("Sign"))
                    if (Mathf.Abs(sign.transform.position.x - x) < 0.5f)
                    { ocupado = true; break; }

            if (!ocupado) return x;
        }

        return carriles[Random.Range(0, carriles.Length)];
    }

    GameObject GetPrefabAleatorio(GameObject[] prefabs) =>
        prefabs[Random.Range(0, prefabs.Length)];

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;

        float speed = GameManager.Instance.GetSpeed();

        // Spawn coches
        carTimer += Time.deltaTime;
        if (carTimer >= GetCarInterval())
        {
            carTimer = 0f;
            int activos = GameObject.FindGameObjectsWithTag("EnemyCar").Length;
            if (activos < MaxCars() && carPrefabs.Length > 0)
            {
                float x = GetCarrilLibre();
                GameObject car = Instantiate(
                    GetPrefabAleatorio(carPrefabs),
                    new Vector3(x, 6f, 0),
                    Quaternion.identity
                );
                car.GetComponent<OncomingCar>().speed = speed;
            }
        }

        // Spawn señales
        signTimer += Time.deltaTime;
        if (signTimer >= GetSignInterval())
        {
            signTimer = 0f;
            int activos = GameObject.FindGameObjectsWithTag("Sign").Length;
            if (activos < MaxSigns() && signPrefabs.Length > 0)
            {
                float x = GetCarrilLibre();
                GameObject sign = Instantiate(
                    GetPrefabAleatorio(signPrefabs),
                    new Vector3(x, 6f, 0),
                    Quaternion.identity
                );
                sign.GetComponent<FixedSign>().speed = speed;
            }
        }
    }
}