using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameController gameController;
    public RawImage rawImage;
    public Button skipButton; // Reference to the skip button

    private VideoScene currentVideoScene;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        rawImage.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        skipButton.onClick.AddListener(SkipVideo);
    }

    public void PlayVideo(VideoScene scene)
    {
        currentVideoScene = scene;
        videoPlayer.clip = scene.videoClip;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        rawImage.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true); // Make the skip button visible
        rawImage.texture = videoPlayer.texture; // Set the texture of the RawImage
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        rawImage.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false); // Hide the skip button
        if (currentVideoScene != null)
        {
            gameController.PlayScene(currentVideoScene.nextScene);
        }
    }

    private void SkipVideo()
    {
        videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false); // Hide the skip button
        if (currentVideoScene != null)
        {
            gameController.PlayScene(currentVideoScene.nextScene);
        }
    }
}
