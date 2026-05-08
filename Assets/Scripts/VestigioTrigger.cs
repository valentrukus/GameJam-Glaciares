using System.Collections;
using UnityEngine;

public class VestigioTrigger : MonoBehaviour
{
    [Header("Narrativa del Vestigio")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    [Header("Configuración")]
    [SerializeField] private bool triggerOnlyOnce = true;

    [SerializeField] private float dialogueDuration = 4f;

    [Header("Final del Juego")]
    [SerializeField] private bool closeGameOnDialogueEnd = false;

    private bool hasActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasActivated)
                return;

            hasActivated = true;

            // Mostrar diálogo automático
            DialogueManager.Instance.StartDialogue(
                dialogueLines,
                OnDialogueFinished,
                true,
                dialogueDuration
            );
        }
    }

    private void OnDialogueFinished()
    {
        if (closeGameOnDialogueEnd)
        {
            Debug.Log("Cerrando juego...");

            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}