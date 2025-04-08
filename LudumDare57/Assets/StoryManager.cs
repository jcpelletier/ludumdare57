using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public AudioSource[] songSources;
    public AudioSource[] audioSources;
    public GameObject[] storyArt;
    public GameObject[] text;

    private int currentIndex = 0;
    private bool waitingForInput = false;  // Flag to check if we're waiting for user input

    private bool isPlayingIntroSong = false;
    private bool isPlayingSecondSong = false;
    private bool textHidden = false;
    private float songTime = 0f;

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

    void Update()
    {
        // If we're on index 0 and waiting for input, check for a trigger (e.g., mouse click, touch, or key press)
        if (currentIndex == 0 && waitingForInput && Input.GetMouseButtonDown(0)) // Left mouse click or touch
        {
            waitingForInput = false;
            AdvanceToNextSlide();
        }

        if (currentIndex == 0 && !isPlayingIntroSong)
        {
            isPlayingIntroSong = true;
            songSources[0].Play();
        }

        if (currentIndex >= 1 && !textHidden)
        {
            textHidden = true;
            text[0].SetActive(false);
            text[1].SetActive(false);
            text[2].SetActive(false);
            text[3].SetActive(false);
        }

        if (currentIndex == 3 && isPlayingIntroSong)
        {
            isPlayingIntroSong = false;
            songSources[0].time = 0f;
            Destroy(songSources[0].gameObject);
        }

        if (currentIndex == 3 && !isPlayingSecondSong)
        {
            isPlayingSecondSong= true;
            songSources[1].Play();
        }
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

        if (currentIndex != 0 && currentIndex != 3 && currentIndex != 4)
        {
            
            if (currentIndex == 6)
            {
                audioSources[currentIndex + 1].Play();
                StartCoroutine(WaitForSecondsThenAdvance(3f));
                audioSources[currentIndex].Play();
            } else
            {
                audioSources[currentIndex].Play();
            }
        }

        // If this is index 0, wait for input instead of auto-advancing
        if (currentIndex == 0)
        {
            waitingForInput = true;
        }
        else if (currentIndex == 3)
        {
            StartCoroutine(WaitForSecondsThenAdvance(6f));
        }
        else if (currentIndex == 4)
        {
            StartCoroutine(WaitForSecondsThenAdvance(6f));
        }
        else if (currentIndex == 5)
        {
            StartCoroutine(WaitForSecondsThenAdvance(5f));
        }
        else if (currentIndex == 6)
        {
            songSources[1].Stop();
            songTime = songSources[1].time;
            StartCoroutine(WaitForSecondsThenAdvance(8f));
        }
        else
        {
            // For other slides, proceed as usual (time-based)
            StartCoroutine(WaitForAudio(audioSources[currentIndex]));
        }
    }

    // Coroutine for timed advances
    System.Collections.IEnumerator WaitForSecondsThenAdvance(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentIndex == 6)
        {
            songSources[1].time = songTime;
            songSources[1].Play();
        }
        AdvanceToNextSlide();
    }

    System.Collections.IEnumerator WaitForAudio(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        AdvanceToNextSlide();
    }

    void AdvanceToNextSlide()
    {
        currentIndex++;
        PlayCurrentAudio();
    }
}