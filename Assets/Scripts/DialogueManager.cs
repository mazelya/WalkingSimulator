using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
using System.Collections.Generic;
using System.Collections;

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

    private HashSet<string> npcsMet = new HashSet<string>();

    [Header("UI Components")]
    public GameObject dialogueBox;
    public Image dialogueBackground;
    public TextMeshProUGUI dialogueText;

    [Header("Texts de Nom (Séparés)")]
    public GameObject playerDisplayNameObj;
    public GameObject npcDisplayNameObj;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI npcNameText;

    [Header("Sprites (Images importées)")]
    public Sprite playerBubbleSprite;
    public Sprite npcBubbleSprite;

    [Header("Compteur de Rencontres")]
    public GameObject counterDisplayObj;
    public TextMeshProUGUI counterText;

    [Header("Apparition Personnage Final")]
    public GameObject specialCharacter; // Glisse ici le personnage qui doit apparaître
    public int targetMetCount = 9;      // Le nombre cible (9 dans ton cas)

    public FirstPersonController playerMovement;

    private DialogueLine[] currentLines;
    private int currentIndex = 0;
    private string currentNpcName;
    private Coroutine counterCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        dialogueBox.SetActive(false);

        if (counterDisplayObj != null) counterDisplayObj.SetActive(false);

        // On s'assure que le personnage spécial est caché au début
        if (specialCharacter != null) specialCharacter.SetActive(false);
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
            playerDisplayNameObj.SetActive(true);
            npcDisplayNameObj.SetActive(false);
            playerNameText.text = "Moi";
        }
        else
        {
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

    public void MarkNpcAsMet(string npcID)
    {
        // On ne compte que les nouveaux PNJ (et on ignore les ID techniques avec "_")
        if (!npcsMet.Contains(npcID) && !npcID.Contains("_"))
        {
            npcsMet.Add(npcID);
            Debug.Log("PNJ mémorisé : " + npcID + " | Total : " + npcsMet.Count);

            UpdateAndShowCounter();

            // --- NOUVELLE LOGIQUE : APPARITION ---
            if (npcsMet.Count >= targetMetCount)
            {
                SpawnSpecialCharacter();
            }
        }
        else if (!npcsMet.Contains(npcID))
        {
            npcsMet.Add(npcID);
        }
    }

    private void SpawnSpecialCharacter()
    {
        if (specialCharacter != null)
        {
            specialCharacter.SetActive(true);
            Debug.Log("Le personnage final est apparu !");
        }
    }

    private void UpdateAndShowCounter()
    {
        if (counterDisplayObj != null && counterText != null)
        {
            counterText.text = "Discussion " + npcsMet.Count + "/10";
            if (counterCoroutine != null) StopCoroutine(counterCoroutine);
            counterCoroutine = StartCoroutine(ShowCounterRoutine());
        }
    }

    private IEnumerator ShowCounterRoutine()
    {
        counterDisplayObj.SetActive(true);
        yield return new WaitForSeconds(5f);
        counterDisplayObj.SetActive(false);
    }

    public bool HasMetNpc(string npcID)
    {
        return npcsMet.Contains(npcID);
    }
}