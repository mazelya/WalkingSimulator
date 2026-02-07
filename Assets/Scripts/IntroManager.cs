using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets; // Pour réactiver le mouvement du joueur après

public class IntroManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup introCanvasGroup; // Le groupe pour le fondu
    public TextMeshProUGUI introText;    // Le texte de contexte

    [Header("Réglages Timing")]
    public float fadeDuration = 2f;      // Durée des fondus
    public float displayDuration = 4f;   // Temps où le texte reste visible

    [Header("Références Joueur")]
    public FirstPersonController playerController;

    private void Start()
    {
        // 1. Bloquer le joueur au début
        if (playerController != null)
            playerController.enabled = false;

        // 2. Lancer la séquence
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Assure-toi que tout est noir au début
        introCanvasGroup.alpha = 1f;

        // Optionnel : Tu peux faire apparaître le texte progressivement ici
        // On attend un peu avant de commencer le fondu vers le jeu
        yield return new WaitForSeconds(displayDuration);

        // 3. Fondu vers le jeu (Alpha de 1 à 0)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            introCanvasGroup.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }

        introCanvasGroup.alpha = 0f;

        // 4. Réactiver le joueur et nettoyer
        if (playerController != null)
            playerController.enabled = true;

        // Désactive l'intro pour ne pas consommer de ressources
        gameObject.SetActive(false);
    }
}