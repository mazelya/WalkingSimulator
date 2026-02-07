using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

[System.Serializable]
public enum Speaker { Player, NPC }

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;
    [TextArea(2, 5)]
    public string text;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Components")]
    public GameObject dialogueBox;
    public Image dialogueBackground;
    public TextMeshProUGUI dialogueText;

    [Header("Texts de Nom (Séparés)")]
    // Glisse ici les objets TextMeshPro qui sont placés à gauche et à droite
    public GameObject playerDisplayNameObj;
    public GameObject npcDisplayNameObj;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI npcNameText;

    [Header("Sprites (Images importées)")]
    public Sprite playerBubbleSprite;
    public Sprite npcBubbleSprite;

    public FirstPersonController playerMovement;

    private DialogueLine[] currentLines;
    private int currentIndex = 0;
    private string currentNpcName;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        dialogueBox.SetActive(false);
    }

    public void ShowDialogue(DialogueLine[] lines, string npcName)
    {
        currentLines = lines;
        currentNpcName = npcName;
        currentIndex = 0;

        dialogueBox.SetActive(true);

        DisplayLine();
    }

    public void DisplayNextLine()
    {
        currentIndex++;
        if (currentIndex < currentLines.Length)
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
        DialogueLine line = currentLines[currentIndex];
        dialogueText.text = line.text;

        if (line.speaker == Speaker.Player)
        {
            // Configuration Joueur
            dialogueBackground.sprite = playerBubbleSprite;

            playerDisplayNameObj.SetActive(true);
            npcDisplayNameObj.SetActive(false);

            playerNameText.text = "Moi";
        }
        else
        {
            // Configuration NPC
            dialogueBackground.sprite = npcBubbleSprite;

            playerDisplayNameObj.SetActive(false);
            npcDisplayNameObj.SetActive(true);

            npcNameText.text = currentNpcName;
        }
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }
}