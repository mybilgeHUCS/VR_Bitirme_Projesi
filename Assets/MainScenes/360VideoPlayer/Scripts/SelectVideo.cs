using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SelectVideo : MonoBehaviour
{
     VideoPlayer videoPlayer;
    [SerializeField] Material skyboxVideoMaterial;

    private void Awake()
    {
        RenderSettings.skybox = skyboxVideoMaterial;
    }
    private void Start()
    {
        videoPlayer = GameObject.FindObjectOfType<VideoPlayer>();
        if(videoPlayer == null)
        {
            Debug.LogError("Video Player couldn't found");
        }
        SelectVideoCoroutine();
    }

    public void SelectVideoCoroutine()
    {
        StartCoroutine(LoadVideo());
    }

    private IEnumerator LoadVideo()
    {
        string path = "";

#if UNITY_EDITOR
        path = UnityEditor.EditorUtility.OpenFilePanel("Select Video", "Assets/MainScenes/360VideoPlayer/Videos/", "mp4");
#elif UNITY_STANDALONE_WIN
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Title = "Select Video";
        fileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";
        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            path = fileDialog.FileName;
        }
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        // You need to implement the file dialog for macOS and Linux
        Debug.LogError("File selection not supported on this platform.");
        yield break;
#else
        // Handle file selection for other platforms
        Debug.LogError("File selection not supported on this platform.");
        yield break;
#endif

        if (!string.IsNullOrEmpty(path))
        {
            videoPlayer.url = "file://" + path;
            videoPlayer.Prepare();

            // Wait until video is prepared
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            RenderTexture videoRenderTexture = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt);
            videoPlayer.targetTexture = videoRenderTexture;
            skyboxVideoMaterial.mainTexture = videoRenderTexture;

            videoPlayer.Play();
            Debug.Log("Playing: " + Path.GetFileName(path));
        }
    }
}

