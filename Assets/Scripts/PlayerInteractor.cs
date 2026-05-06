using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [SerializeField] private Transform interactionPoint; // Un Empty GameObject frente al jugador
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private LayerMask interactableLayer; // Capa exclusiva para objetos interactuables

    private IInteractable currentInteractable;

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking) return;

        ScanForInteractables();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        //Debug.Log("TryInteract");

        Collider2D hitCollider = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);

        if (hitCollider != null)
        {
            // Usamos TryGetComponent (optimizado) para ver si el objeto firmó el contrato IInteractable
            if (hitCollider.TryGetComponent<IInteractable>(out IInteractable interactableObject))
            {
                interactableObject.Interact();
            }
        }
    }



    private void ScanForInteractables()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);

        if (hitCollider != null && hitCollider.TryGetComponent<IInteractable>(out IInteractable newInteractable))
        {
            // Si encontramos un objeto NUEVO, apagamos el anterior y prendemos el nuevo
            if (currentInteractable != newInteractable)
            {
                currentInteractable?.ToggleHighlight(false);
                currentInteractable = newInteractable;
                currentInteractable.ToggleHighlight(true);
            }
        }
        else
        {
            // Si no tocamos nada, apagamos el que teníamos guardado
            if (currentInteractable != null)
            {
                currentInteractable.ToggleHighlight(false);
                currentInteractable = null;
            }
        }
    }




    private void OnDrawGizmosSelected()
    {
        // Esto te ayuda a visualizar el área de interacción en el editor
        if (interactionPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
        }
    }
}
