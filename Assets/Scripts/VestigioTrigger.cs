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

    private bool hasActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasActivated)
                return;

            hasActivated = true;

            //StartCoroutine(ShowDialogueTemporarily());
            DialogueManager.Instance.StartDialogue(dialogueLines, null, true, 4f);
        }
    }

    private IEnumerator ShowDialogueTemporarily()
    {
        // Mostrar diálogo
        DialogueManager.Instance.StartDialogue(dialogueLines, null);

        // Esperar unos segundos
        yield return new WaitForSeconds(dialogueDuration);

        // Cerrar diálogo automáticamente
        DialogueManager.Instance.EndDialogue();
    }
}