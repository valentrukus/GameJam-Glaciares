using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private CinemachineCamera virtualCamera; // Tu CM vcam1

    [Header("Configuración")]
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Empezamos con la pantalla transparente
        fadeGroup.alpha = 0;
        fadeGroup.blocksRaycasts = false;
    }

    public void TeleportToTime(Transform player, Vector3 targetPosition)
    {
        StartCoroutine(TeleportSequence(player, targetPosition));
    }

    private IEnumerator TeleportSequence(Transform player, Vector3 targetPosition)
    {
        // 1. Bloquear interacción y empezar fundido
        fadeGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(1f));

        // 2. EL SALTO (Mecánica)
        // Desactivamos el seguimiento de cámara un frame para evitar el "barrido"
        virtualCamera.OnTargetObjectWarped(player, targetPosition - player.position);
        player.position = targetPosition;

        // Esperamos un frame para que las físicas se asienten
        yield return new WaitForFixedUpdate();

        // 3. Quitar fundido
        yield return StartCoroutine(Fade(0f));
        fadeGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeGroup.alpha;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = targetAlpha;
    }
}
