using UnityEngine;

public class Vestigio : MonoBehaviour, IInteractable
{
    [Header("Narrativa del Vestigio")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines; // Aquí escribes tu texto en el Inspector

    //[Header("Configuración Temporal")]
    //[SerializeField] private int targetTimeLayer = 1;

    [Header("Teleport Point")]
    [Tooltip("Crea un objeto vacío en la otra zona y arrástralo aquí")]
    [SerializeField] private Transform teleportTarget;

    [Header("Feedback Visual")]
    [Tooltip("Arrastra aquí el GameObject hijo que tiene el sprite del contorno blanco")]
    [SerializeField] private GameObject highlightOutline;

    private void Start()
    {
        if (highlightOutline != null) highlightOutline.SetActive(false);
    }

    public void ToggleHighlight(bool isOn)
    {
        if (highlightOutline != null)
        {
            highlightOutline.SetActive(isOn);
        }
    }

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueLines, ExecuteTimeJump);
    }

    private void ExecuteTimeJump()
    {
        // Buscamos al jugador por Tag o referencia (Profesional: usar referencia)
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (teleportTarget != null)
        {
            if (PauseManager.Instance != null) PauseManager.Instance.lastSafeCheckpoint = teleportTarget;
            TimeManager.Instance.TeleportToTime(player.transform, teleportTarget.position);
        }
        else
        {
            Debug.LogError("Error: No asignaste un Teleport Target al vestigio.");
        }
    }
}
