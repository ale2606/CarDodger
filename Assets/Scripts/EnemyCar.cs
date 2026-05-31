using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    [HideInInspector] public float speed = 2f;
    [HideInInspector] public int laneIndex = 0;
    [HideInInspector] public int playerOwner = 1;
    [HideInInspector] public float fixedX = 0f;

    private bool paused = false;
    private float destroyY = -12f;
    private bool playerPassed = false;

    void Update()
    {
        if (paused) return;

        // Movimiento hacia abajo manteniendo carril
        Vector3 pos = transform.position;
        pos.x = fixedX;
        pos.y -= speed * Time.deltaTime;
        transform.position = pos;

        // Sumar puntos cuando el jugador pasa al auto
        if (!playerPassed)
        {
            CarController player = RaceGameManager.Instance.GetPlayer(playerOwner);
            if (player != null && player.isAlive)
            {
                if (transform.position.y < player.transform.position.y)
                {
                    playerPassed = true;
                    player.AddScore(10);
                }
            }
        }

        if (transform.position.y < destroyY)
        {
            EnemySpawner.Instance.OnCarPassed(playerOwner);
            Destroy(gameObject);
        }
    }

    public void UpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void PauseThisCar()
    {
        paused = true;
    }
}