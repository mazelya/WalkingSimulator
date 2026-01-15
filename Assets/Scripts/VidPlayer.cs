using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private string videoUrl = "https://mazelya.github.io/VideoHost/videoplayback.mp4";
    private VideoPlayer videoPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if(videoPlayer)
        {
            videoPlayer.url = videoUrl;
            videoPlayer.playOnAwake = false;
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    // Update is called once per frame
    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.Play(); 
    }
}
