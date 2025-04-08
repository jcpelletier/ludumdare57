// intro https://api-f.streamable.com/api/v1/videos/qhzgho/mp4
// slide 1 https://api-f.streamable.com/api/v1/videos/eayesm/mp4

using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(MeshRenderer))]
public class VideoStreamLoader : MonoBehaviour
{
    [Header("Video Settings")]
    public string videoURL;
    public bool autoPlay = true;
    public bool loop = true;
    public bool bidirectionalLoop = false;

    [Header("Playback Settings")]
    [Range(-1f, 1f)] public float playbackSpeed = 1f;
    public float startBuffer = 0.1f; // Prevents end-of-video glitches

    private VideoPlayer player;
    private RenderTexture renderTexture;
    private bool isReverse = false;
    private Coroutine playbackRoutine;

    void Start()
    {
        // Set up video player
        player = GetComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.source = VideoSource.Url;

        // Create render texture
        renderTexture = new RenderTexture(1920, 1080, 24);
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Texture"));
        GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
        player.targetTexture = renderTexture;

        if (autoPlay) StartVideo();
    }

    public void StartVideo()
    {
        if (playbackRoutine != null) StopCoroutine(playbackRoutine);
        playbackRoutine = StartCoroutine(VideoPlaybackRoutine());
    }

    IEnumerator VideoPlaybackRoutine()
    {
        player.url = videoURL;
        player.Prepare();

        while (!player.isPrepared)
            yield return null;

        do
        {
            // Set initial play values
            isReverse = bidirectionalLoop && !isReverse;
            player.playbackSpeed = isReverse ? -Mathf.Abs(playbackSpeed) : Mathf.Abs(playbackSpeed);
            player.time = isReverse ? player.length - startBuffer : startBuffer;

            player.Play();

            // Wait for playback to complete
            if (isReverse)
                yield return new WaitUntil(() => !player.isPlaying || player.time <= startBuffer);
            else
                yield return new WaitUntil(() => !player.isPlaying || player.time >= player.length - startBuffer);

        } while (loop && (!bidirectionalLoop || isReverse));
    }

    void OnDestroy()
    {
        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}