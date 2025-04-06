using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [Header("Audio and Art References")]
    public AudioSource[] audioSources;
    public GameObject[] storyArt;

    private int currentIndex = 0;

    void Awake()
    {
        if (audioSources.Length == 0 || storyArt.Length == 0)
        {
            Debug.LogWarning("StoryManager is missing audio sources or story art.");
            return;
        }

        // Disable all story art at start
        foreach (GameObject art in storyArt)
        {
            art.SetActive(false);
        }

        // Begin the sequence
        PlayCurrentAudio();
    }

    void PlayCurrentAudio()
    {
        if (currentIndex >= audioSources.Length)
        {
            Debug.Log("Story completed.");
            return;
        }

        // Enable corresponding art
        for (int i = 0; i < storyArt.Length; i++)
        {
            storyArt[i].SetActive(i == currentIndex);
        }

        // Subscribe to completion
        audioSources[currentIndex].Play();
        StartCoroutine(WaitForAudio(audioSources[currentIndex]));
    }

    System.Collections.IEnumerator WaitForAudio(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        currentIndex++;
        PlayCurrentAudio();
    }
}
