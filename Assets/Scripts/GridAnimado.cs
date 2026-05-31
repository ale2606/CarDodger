using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class GridAnimado : MonoBehaviour
{
    [Header("Parámetros")]
    [Tooltip("Velocidad de scroll en UV por segundo")]
    public float velocidad = 0.05f;

    [Tooltip("Generar textura de rejilla por código (no necesita archivo externo)")]
    public bool  generarPorCodigo = true;

    [Tooltip("Tamaño de celda en píxeles de la textura generada")]
    public int   tamCelda = 32;

    [Tooltip("Color de la línea de la rejilla")]
    public Color colorLinea = new Color(0.91f, 0f, 0.11f, 0.06f);

    RawImage _img;
    float    _offsetV;

    void Awake()
    {
        _img = GetComponent<RawImage>();
        if (generarPorCodigo)
            _img.texture = GenerarTexturaRejilla();
    }

    void Update()
    {
        // Desplazar la textura verticalmente (scroll down = rejilla baja)
        _offsetV += velocidad * Time.deltaTime;
        if (_offsetV > 1f) _offsetV -= 1f;

        Rect uvRect = _img.uvRect;
        uvRect.y    = _offsetV;
        _img.uvRect = uvRect;
    }

  
    Texture2D GenerarTexturaRejilla()
    {
        int  size = tamCelda;
        var  tex  = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode   = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        Color transparente = Color.clear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Pintar píxeles en la primera fila y primera columna = rejilla
                bool esLinea = (x == 0) || (y == 0);
                tex.SetPixel(x, y, esLinea ? colorLinea : transparente);
            }
        }
        tex.Apply();
        return tex;
    }
}
