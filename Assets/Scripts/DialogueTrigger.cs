using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] dialogueLines; // Liste de phrases éditables dans l'inspecteur

    public bool playOnlyOnce = false;
    private bool hasBeenPlayed = false;
    private bool isInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnlyOnce && hasBeenPlayed) return;

            isInside = true;
            DialogueManager.Instance.ShowDialogue(dialogueLines);
            hasBeenPlayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = false;
            DialogueManager.Instance.HideDialogue();
        }
    }

    private void Update()
    {
        // Si on appuie sur le bouton de la souris (ou une touche) pendant le dialogue
        if (isInside && Input.GetMouseButtonDown(0))
        {
            DialogueManager.Instance.DisplayNextLine();
        }
    }
}