using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ReminderCondition
{
    [Tooltip("L'ID du PNJ que le joueur n'a PAS encore rencontré")]
    public string targetNpcID;

    [Tooltip("Ce que le guide dit pour nous rappeler d'aller le voir")]
    public DialogueLine[] reminderLines;
}

public class GuideAutoTalker : MonoBehaviour
{
    public string guideName;

    [Header("Condition d'activation globale")]
    [Tooltip("L'ID du guide lui-même. Le joueur doit lui avoir parlé au moins une fois pour activer les rappels.")]
    public string guideID;

    [Header("Réglages du Timer")]
    public float silenceRequired = 5f;
    private float timer = 0f;

    [Header("Liste des Rappels")]
    public List<ReminderCondition> reminders;

    private bool isGuideSpeaking = false;

    private void Update()
    {
        // --- NOUVELLE CONDITION ---
        // Si le joueur n'a JAMAIS parlé au guide (premier contact), on ne fait rien du tout
        if (!DialogueManager.Instance.HasMetNpc(guideID))
        {
            return;
        }
        // --------------------------

        // 1. GESTION DU CLIC
        if (isGuideSpeaking && Input.GetMouseButtonDown(0))
        {
            DialogueManager.Instance.DisplayNextLine();

            if (!DialogueManager.Instance.dialogueBox.activeSelf)
            {
                isGuideSpeaking = false;
            }
            return;
        }

        // 2. GESTION DU TIMER
        // Le timer se remet à zéro si : la box est active OU si une vidéo joue
        if (DialogueManager.Instance.dialogueBox.activeSelf || DialogueManager.Instance.isVideoPlaying)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= silenceRequired)
        {
            CheckForForgottenNPCs();
        }
    }

    private void CheckForForgottenNPCs()
    {
        foreach (var item in reminders)
        {
            string reminderDoneKey = "ReminderSent_" + item.targetNpcID;

            if (!DialogueManager.Instance.HasMetNpc(item.targetNpcID) && !DialogueManager.Instance.HasMetNpc(reminderDoneKey))
            {
                isGuideSpeaking = true;

                DialogueManager.Instance.ShowDialogue(item.reminderLines, guideName);
                DialogueManager.Instance.MarkNpcAsMet(reminderDoneKey);

                timer = 0f;
                break;
            }
        }
    }
}