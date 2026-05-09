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

    public bool IsTalking { get; private set; }

    private string[] currentLines;
    private int currentLineIndex;
    private Action onDialogueCompleteCallback;
    private Coroutine typingCoroutine;
    private bool isTyping;
    private bool autoCloseDialogue;
    private float autoCloseTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsTalking) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(string[] lines, Action onComplete, bool autoClose = false, float closeTime = 3f)
    {
        if (IsTalking) return;

        dialoguePanel.SetActive(true);

        currentLines = lines;
        onDialogueCompleteCallback = onComplete;
        currentLineIndex = 0;

        autoCloseDialogue = autoClose;
        autoCloseTime = closeTime;

        IsTalking = true;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        isTyping = false;
        IsTalking = false;

        dialogueText.text = "";

        dialoguePanel.SetActive(false);

        onDialogueCompleteCallback?.Invoke();
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
            EndDialogue();
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

        if (autoCloseDialogue)
        {
            yield return new WaitForSeconds(autoCloseTime);

            NextLine();
        }
    }
}