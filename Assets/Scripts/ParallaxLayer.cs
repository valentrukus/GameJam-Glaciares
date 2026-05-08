using UnityEngine;

/// <summary>
/// ParallaxLayer — Adjuntalo a cada capa del fondo.
/// Funciona con cámara ortográfica o perspectiva en juegos 2D.
///
/// SETUP RÁPIDO:
///   1. Tenés varias capas de fondo (sprites/GameObjects).
///   2. A cada una le adjuntás este script.
///   3. Ajustás el paralaxFactor:
///        0.0 = se mueve igual que la cámara (fijo en pantalla)
///        0.5 = se mueve a la mitad de velocidad (efecto medio)
///        1.0 = no se mueve (fondo lejano/estático)
///      El cielo suele ir en ~0.9, las montañas ~0.6, arbustos ~0.3, etc.
///   4. Si tu fondo tiene que hacer loop (infinito), activá "enableLooping"
///      y asegurate de que el sprite sea lo suficientemente ancho
///      (al menos 2x el ancho de la cámara).
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax")]
    [Tooltip("Qué tan lento se mueve la capa respecto a la cámara.\n0 = pegada a la cámara | 1 = completamente fija en el mundo")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Tooltip("Activar también el parallax en el eje Y (útil si la cámara sube/baja)")]
    public bool parallaxY = false;

    [Header("Loop infinito")]
    [Tooltip("Repite el fondo horizontalmente para que nunca se corte")]
    public bool enableLooping = true;

    [Tooltip("Repite también verticalmente")]
    public bool loopY = false;

    // ── Internals ──
    private Transform   _cam;
    private Vector3     _lastCamPos;
    private float       _spriteWidth;
    private float       _spriteHeight;

    void Start()
    {
        // Busca la cámara principal
        _cam        = Camera.main.transform;
        _lastCamPos = _cam.position;

        // Calcula el tamaño del sprite para el loop
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            _spriteWidth  = sr.bounds.size.x;
            _spriteHeight = sr.bounds.size.y;
        }
        else
        {
            // Si no hay SpriteRenderer, intenta con el Renderer genérico
            var r = GetComponent<Renderer>();
            if (r != null)
            {
                _spriteWidth  = r.bounds.size.x;
                _spriteHeight = r.bounds.size.y;
            }
        }
    }

    void LateUpdate()
    {
        // Cuánto se movió la cámara desde el último frame
        Vector3 delta = _cam.position - _lastCamPos;

        // Desplazamiento de esta capa = movimiento de cámara * (1 - factor)
        // Con factor 1 → delta * 0 → no se mueve (estático)
        // Con factor 0 → delta * 1 → se mueve igual que la cámara (fijo en screen)
        float moveX = delta.x * (1f - parallaxFactor);
        float moveY = parallaxY ? delta.y * (1f - parallaxFactor) : 0f;

        transform.position += new Vector3(moveX, moveY, 0f);

        _lastCamPos = _cam.position;

        // ── Loop horizontal ──
        if (enableLooping && _spriteWidth > 0f)
        {
            float camX    = _cam.position.x;
            float offsetX = camX - transform.position.x;

            if (offsetX > _spriteWidth * 0.5f)
                transform.position += new Vector3(_spriteWidth, 0f, 0f);
            else if (offsetX < -_spriteWidth * 0.5f)
                transform.position -= new Vector3(_spriteWidth, 0f, 0f);
        }

        // ── Loop vertical ──
        if (loopY && _spriteHeight > 0f)
        {
            float camY    = _cam.position.y;
            float offsetY = camY - transform.position.y;

            if (offsetY > _spriteHeight * 0.5f)
                transform.position += new Vector3(0f, _spriteHeight, 0f);
            else if (offsetY < -_spriteHeight * 0.5f)
                transform.position -= new Vector3(0f, _spriteHeight, 0f);
        }
    }

    // ── Gizmo para ver el factor en Scene View ──
    void OnDrawGizmosSelected()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Gizmos.color = Color.Lerp(Color.red, Color.green, parallaxFactor);
        Gizmos.DrawWireCube(transform.position, sr.bounds.size);

#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * sr.bounds.extents.y + Vector3.up * 0.2f,
            $"Parallax: {parallaxFactor:F2}"
        );
#endif
    }
}
