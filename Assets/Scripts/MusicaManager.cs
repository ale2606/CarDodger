using UnityEngine;

public class MusicaManager : MonoBehaviour
{
    public static MusicaManager Instance;

    [Header("Música")]
    public AudioClip musicaFondo;
    public float volumen = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        // Si ya existe una instancia no crear otra
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicaFondo;
        audioSource.loop = true;
        audioSource.volume = volumen;
        audioSource.Play();
    }

    public void CambiarVolumen(float nuevoVolumen)
    {
        volumen = nuevoVolumen;
        if (audioSource != null)
            audioSource.volume = nuevoVolumen;
    }

    public void PausarMusica()
    {
        if (audioSource != null)
            audioSource.Pause();
    }

    public void ReanudarMusica()
    {
        if (audioSource != null)
            audioSource.UnPause();
    }

    public void CambiarMusica(AudioClip nuevoClip)
    {
        if (audioSource != null)
        {
            audioSource.clip = nuevoClip;
            audioSource.Play();
        }
    }
}