using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nextSceneName;

    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No asignaste el nombre de la escena.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}