using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public GameObject[] signPrefabs;

    // Carriles J1 y J2 con distribucion uniforme
    float[] carrilesJ1 = { -5.69f, -3.63f, -1.34f, 0.65f };
    float[] carrilesJ2 = { 6.32f, 7.85f, 9.76f, 11.44f };

    float carTimer = 0f;
    float signTimer = 0f;

    float GetCarInterval() => Mathf.Max(0.6f, 1.3f - GetNivel() * 0.13f);
    float GetSignInterval() => Mathf.Max(1.0f, 2.1f - GetNivel() * 0.2f);

    int GetNivel() => Mathf.Min(GetCoches() / 20, 2);
    int GetCoches() => 0; // lo conectaremos al RaceGameManager

    int MaxCars() => Mathf.Min(2 + GetNivel(), 4);
    int MaxSigns() => Mathf.Min(1 + GetNivel(), 2);

    // Distribucion uniforme
    float GetCarrilAleatorio(float[] carriles) =>
        carriles[Random.Range(0, carriles.Length)];

    GameObject GetPrefabAleatorio(GameObject[] prefabs) =>
        prefabs[Random.Range(0, prefabs.Length)];

    float GetCarrilLibre(float[] carriles, string tagEvitar)
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
            foreach (var obj in GameObject.FindGameObjectsWithTag(tagEvitar))
                if (Mathf.Abs(obj.transform.position.x - x) < 0.5f)
                { ocupado = true; break; }
            foreach (var obj in GameObject.FindGameObjectsWithTag("Sign"))
                if (Mathf.Abs(obj.transform.position.x - x) < 0.5f)
                { ocupado = true; break; }
            if (!ocupado) return x;
        }
        return GetCarrilAleatorio(carriles);
    }

    void Update()
    {
        float speed = GetSpeed();

        // Spawn coches J1
        carTimer += Time.deltaTime;
        if (carTimer >= GetCarInterval())
        {
            carTimer = 0f;

            // Spawn en J1
            int activosJ1 = CountInRange(carrilesJ1, "EnemyCar");
            if (activosJ1 < MaxCars() && carPrefabs.Length > 0)
            {
                float x = GetCarrilLibre(carrilesJ1, "EnemyCar");
                var car = Instantiate(GetPrefabAleatorio(carPrefabs),
                    new Vector3(x, 8f, 0), Quaternion.identity);
                car.GetComponent<OncomingCar>().speed = speed;
            }

            // Spawn en J2
            int activosJ2 = CountInRange(carrilesJ2, "EnemyCar");
            if (activosJ2 < MaxCars() && carPrefabs.Length > 0)
            {
                float x = GetCarrilLibre(carrilesJ2, "EnemyCar");
                var car = Instantiate(GetPrefabAleatorio(carPrefabs),
                    new Vector3(x, 8f, 0), Quaternion.identity);
                car.GetComponent<OncomingCar>().speed = speed;
            }
        }

        // Spawn señales J1
        signTimer += Time.deltaTime;
        if (signTimer >= GetSignInterval())
        {
            signTimer = 0f;

            int activosJ1 = CountInRange(carrilesJ1, "Sign");
            if (activosJ1 < MaxSigns() && signPrefabs.Length > 0)
            {
                float x = GetCarrilLibre(carrilesJ1, "Sign");
                var sign = Instantiate(GetPrefabAleatorio(signPrefabs),
                    new Vector3(x, 8f, 0), Quaternion.identity);
                sign.GetComponent<FixedSign>().speed = speed;
            }

            int activosJ2 = CountInRange(carrilesJ2, "Sign");
            if (activosJ2 < MaxSigns() && signPrefabs.Length > 0)
            {
                float x = GetCarrilLibre(carrilesJ2, "Sign");
                var sign = Instantiate(GetPrefabAleatorio(signPrefabs),
                    new Vector3(x, 8f, 0), Quaternion.identity);
                sign.GetComponent<FixedSign>().speed = speed;
            }
        }
    }

    int CountInRange(float[] carriles, string tag)
    {
        int count = 0;
        foreach (var obj in GameObject.FindGameObjectsWithTag(tag))
            foreach (float x in carriles)
                if (Mathf.Abs(obj.transform.position.x - x) < 1f)
                { count++; break; }
        return count;
    }

    float GetSpeed()
    {
        float[] speeds = { 2f, 4f, 6f };
        return speeds[GetNivel()];
    }
}