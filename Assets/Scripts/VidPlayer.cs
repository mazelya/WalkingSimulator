using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] private string VideoUrl = "https://mazelya.github.io/VideoHostPortfolio/MarioKart.mp4";
    [Range(0f, 1f)]
    public float volume = 1f; // Permet de régler le son dans l'inspecteur

    private VideoPlayer videoPlayer;
    private bool videoPrepared = false;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.url = VideoUrl;
            videoPlayer.playOnAwake = false;

            // Assurez-vous que la vidéo utilise un AudioSource
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            if (!videoPlayer.GetComponent<AudioSource>())
            {
                videoPlayer.gameObject.AddComponent<AudioSource>();
            }
            videoPlayer.SetTargetAudioSource(0, videoPlayer.GetComponent<AudioSource>());
            videoPlayer.GetTargetAudioSource(0).volume = volume;

            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPrepared = true; // La vidéo est prête mais ne se joue pas encore
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && videoPrepared)
        {
            videoPlayer.Play(); // Joue la vidéo quand le joueur entre
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && videoPrepared)
        {
            videoPlayer.Pause(); // Arrête la vidéo quand le joueur sort
        }
    }

    private void Update()
    {
        // Met à jour le volume en temps réel si besoin
        if (videoPlayer && videoPlayer.GetTargetAudioSource(0))
        {
            videoPlayer.GetTargetAudioSource(0).volume = volume;
        }
    }
}
