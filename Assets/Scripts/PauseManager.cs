using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public GameObject player;

    [Header("Referencias de UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Sistema Anti-Softlock")]
    [Tooltip("El punto seguro al que el jugador volverá si se queda atascado.")]
    public Transform lastSafeCheckpoint;

    private bool isPaused;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void RescuePlayer()
    {
        if (lastSafeCheckpoint == null)
        {
            Debug.LogError("Error: No hay un Checkpoint seguro asignado en el PauseManager.");
            return;
        }

        ResumeGame();

        //GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && TimeManager.Instance != null)
        {
            Debug.Log("Rescatando al jugador del Softlock...");
            TimeManager.Instance.TeleportToTime(player.transform, lastSafeCheckpoint.position);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Time.timeScale = 1f;
        Application.Quit();
    }
}
