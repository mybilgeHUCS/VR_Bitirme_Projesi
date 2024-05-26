using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SelectVideo : MonoBehaviour
{
    VideoPlayer videoPlayer;
    [SerializeField] Material skyboxVideoMaterial;
    public VideoClip[] clipList;

    private void Awake()
    {
        RenderSettings.skybox = skyboxVideoMaterial;
    }

    private void Start()
    {
        videoPlayer = GameObject.FindObjectOfType<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("Video Player couldn't be found");
            return;
        }
        SelectVideoCoroutine();
    }

    public void SelectVideoCoroutine()
    {
        StartCoroutine(LoadVideo());
    }

    private IEnumerator LoadVideo()
    {
        int selectedVideoIndex = PlayerPrefs.GetInt("SelectedVideoIndex", 0); // Default to the first video if not set
        if (selectedVideoIndex < 0 || selectedVideoIndex >= clipList.Length)
        {
            Debug.LogError("Selected video index is out of range");
            yield break;
        }

        VideoClip selectedClip = clipList[selectedVideoIndex];
        videoPlayer.clip = selectedClip;
        videoPlayer.Prepare();

        // Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        RenderTexture videoRenderTexture = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, 0);
        videoPlayer.targetTexture = videoRenderTexture;
        skyboxVideoMaterial.mainTexture = videoRenderTexture;

        videoPlayer.Play();
        Debug.Log("Playing: " + selectedClip.name);
    }
}