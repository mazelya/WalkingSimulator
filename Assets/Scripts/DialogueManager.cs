using UnityEngine;
using TMPro;
using StarterAssets;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public FirstPersonController playerMovement;

    private string[] currentDialogues;
    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        dialogueBox.SetActive(false);
    }

    public void ShowDialogue(string[] lines) // Reçoit un tableau de phrases
    {
        currentDialogues = lines;
        currentIndex = 0;

        dialogueBox.SetActive(true);

        DisplayLine();
    }

    public void DisplayNextLine()
    {
        currentIndex++;

        if (currentIndex < currentDialogues.Length)
        {
            DisplayLine();
        }
        else
        {
            HideDialogue();
        }
    }

    private void DisplayLine()
    {
        dialogueText.text = currentDialogues[currentIndex];
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }
}