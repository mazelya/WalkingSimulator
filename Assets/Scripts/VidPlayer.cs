using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] private string VideoUrl = "https://mazelya.github.io/VideoHostPortfolio/MarioKart.mp4";
    [Range(0f, 1f)]
    public float volume = 1f;

    private VideoPlayer videoPlayer;
    private bool videoPrepared = false;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.url = VideoUrl;
            videoPlayer.playOnAwake = false;

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
        videoPrepared = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && videoPrepared)
        {
            videoPlayer.Play();

            // --- BLOQUE LE GUIDE ---
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.isVideoPlaying = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && videoPrepared)
        {
            videoPlayer.Pause();

            // --- LIBÈRE LE GUIDE ---
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.isVideoPlaying = false;
        }
    }

    private void Update()
    {
        if (videoPlayer && videoPlayer.GetTargetAudioSource(0))
        {
            videoPlayer.GetTargetAudioSource(0).volume = volume;
        }
    }
}