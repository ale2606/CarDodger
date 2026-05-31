using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScanlineOverlay : MonoBehaviour
{
    [Header("Parámetros")]
    [Tooltip("Altura de la raya oscura en píxeles")]
    public int grosorRaya   = 1;

    [Tooltip("Separación entre rayas en píxeles")]
    public int separacion   = 4;

    [Tooltip("Opacidad de las rayas (0=invisible, 1=negro sólido)")]
    [Range(0f, 1f)]
    public float opacidad   = 0.18f;

    void Awake()
    {
        var img = GetComponent<RawImage>();

        // Hacer que el RawImage cubra toda la pantalla
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        img.texture = GenerarTexturaScanlines();
        img.color   = Color.white;

        // No interceptar clicks
        img.raycastTarget = false;
    }

    Texture2D GenerarTexturaScanlines()
    {
        int alturaPatron = separacion;
        var tex = new Texture2D(1, alturaPatron, TextureFormat.RGBA32, false);
        tex.wrapMode   = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < alturaPatron; y++)
        {
            // Las primeras 'grosorRaya' filas son oscuras, el resto transparente
            bool esRaya = y < grosorRaya;
            tex.SetPixel(0, y,
                esRaya
                    ? new Color(0f, 0f, 0f, opacidad)
                    : Color.clear);
        }
        tex.Apply();
        return tex;
    }
}
