using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Tecla de Interacción")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private IInteractable currentInteractable;

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking) return;

        ScanForInteractables();

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);
        if (hitCollider != null)
        {
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
            if (currentInteractable != newInteractable)
            {
                currentInteractable?.ToggleHighlight(false);
                currentInteractable = newInteractable;
                currentInteractable.ToggleHighlight(true);
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.ToggleHighlight(false);
                currentInteractable = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
        }
    }
}