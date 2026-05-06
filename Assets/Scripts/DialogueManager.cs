using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Referencias de UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Configuración")]
    [SerializeField] private float typingSpeed = 0.03f;

    // Estado global para que el jugador sepa si debe detenerse
    public bool IsTalking { get; private set; }

    private string[] currentLines;
    private int currentLineIndex;
    private Action onDialogueCompleteCallback;
    private Coroutine typingCoroutine;
    private bool isTyping;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsTalking) return;

        // Avanzar en el diálogo con la E o el Espacio
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Si está escribiendo, forzamos a que termine la frase al instante
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                // Si ya terminó de escribir, pasamos a la siguiente línea
                NextLine();
            }
        }
    }

    public void StartDialogue(string[] lines, Action onComplete)
    {
        if (IsTalking) return;
        dialoguePanel.SetActive(true);

        currentLines = lines;
        onDialogueCompleteCallback = onComplete;
        currentLineIndex = 0;

        IsTalking = true;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentLines.Length)
        {
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(EndDialogueRoutine());
            //IsTalking = false;
            //dialoguePanel.SetActive(false);
            //onDialogueCompleteCallback?.Invoke();
        }
    }

    private IEnumerator EndDialogueRoutine()
    {
        dialoguePanel.SetActive(false);

        onDialogueCompleteCallback?.Invoke();

        yield return null;

        IsTalking = false;
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in currentLines[currentLineIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
