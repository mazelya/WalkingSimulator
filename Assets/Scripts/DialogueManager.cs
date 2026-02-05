using UnityEngine;
using UnityEngine.UI; // Nécessaire pour le composant Image
using TMPro;
using StarterAssets;

// Structure pour configurer chaque réplique
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
    public Image dialogueBackground;    // L'image de fond qui va changer
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;    // Le texte qui affichera le nom

    [Header("Sprites (Images importées)")]
    public Sprite playerBubbleSprite; // Ton image avec nom à droite
    public Sprite npcBubbleSprite;    // Ton image avec nom à gauche

    public FirstPersonController playerMovement;

    private DialogueLine[] currentLines;
    private int currentIndex = 0;
    private string currentNpcName; // Stocke le nom du NPC actuel

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
            dialogueBackground.sprite = playerBubbleSprite;
            nameText.text = "Moi"; // Ou ton nom de joueur
            nameText.alignment = TextAlignmentOptions.Right; // Aligne le nom à droite
        }
        else
        {
            dialogueBackground.sprite = npcBubbleSprite;
            nameText.text = currentNpcName;
            nameText.alignment = TextAlignmentOptions.Left; // Aligne le nom à gauche
        }
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }
}