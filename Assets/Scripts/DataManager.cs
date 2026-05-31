using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string nombre = "";
    public int avatarIndex = 0;
    public int highScore = 0;
    public int nivel = 0;
    public int score = 0;
}

[System.Serializable]
public class RecordEntry
{
    public string nombre = "";
    public int score = 0;
    public int nivel = 0;
    public string fecha = "";
    public string modo = "";
    public int avatarIndex = 0;
}

[System.Serializable]
public class GameData
{
    public PlayerData jugador1 = new PlayerData();
    public PlayerData jugador2 = new PlayerData();
    public List<RecordEntry> records1P = new List<RecordEntry>();
    public List<RecordEntry> records2P = new List<RecordEntry>();
    public List<RecordEntry> top5 = new List<RecordEntry>();
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public GameData data = new GameData();
    public string path;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        path = Application.persistentDataPath + "/gamedata.json";
        Load();
    }

    public void Save()
    {
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (File.Exists(path))
            data = JsonUtility.FromJson<GameData>(File.ReadAllText(path));
    }

    public void AddRecord1P(string nombre, int score, int nivel, int avatarIndex)
    {
        // Verificar si ya existe una entrada igual para evitar duplicados
        bool duplicado = data.records1P.Exists(r =>
            r.nombre == nombre && r.score == score && r.modo == "1P");
        if (duplicado) return;

        var entry = new RecordEntry
        {
            nombre = nombre,
            score = score,
            nivel = nivel,
            fecha = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            modo = "1P",
            avatarIndex = avatarIndex
        };
        data.records1P.Add(entry);
        data.records1P.Sort((a, b) => b.score.CompareTo(a.score));
        if (data.records1P.Count > 10)
            data.records1P.RemoveRange(10, data.records1P.Count - 10);

        ActualizarTop5(entry);
        Save();
    }

    public void AddRecord2P(string nombre, int score, int nivel, int avatarIndex)
    {
        // Verificar si ya existe una entrada igual para evitar duplicados
        bool duplicado = data.records2P.Exists(r =>
            r.nombre == nombre && r.score == score && r.modo == "2P");
        if (duplicado) return;

        var entry = new RecordEntry
        {
            nombre = nombre,
            score = score,
            nivel = nivel,
            fecha = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            modo = "2P",
            avatarIndex = avatarIndex
        };
        data.records2P.Add(entry);
        data.records2P.Sort((a, b) => b.score.CompareTo(a.score));
        if (data.records2P.Count > 10)
            data.records2P.RemoveRange(10, data.records2P.Count - 10);

        ActualizarTop5(entry);
        Save();
    }

    void ActualizarTop5(RecordEntry entry)
    {
        // Evitar duplicados en top5
        bool duplicado = data.top5.Exists(r =>
            r.nombre == entry.nombre && r.score == entry.score);
        if (duplicado) return;

        data.top5.Add(entry);
        data.top5.Sort((a, b) => b.score.CompareTo(a.score));
        if (data.top5.Count > 5)
            data.top5.RemoveRange(5, data.top5.Count - 5);
    }

    // Limpiar todos los records guardados
    public void LimpiarRecords()
    {
        data.records1P.Clear();
        data.records2P.Clear();
        data.top5.Clear();
        Save();
    }
}