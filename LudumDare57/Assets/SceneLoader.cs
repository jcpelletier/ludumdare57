using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Array of GameObjects to disable (e.g., UI elements)
    public GameObject[] uiElementsToDisable;

    // Name of the scene to load
    public string sceneName;

    // This function is called by the UI Button on click.
    public void LoadScene()
    {
        // Disable all specified UI elements
        foreach (GameObject element in uiElementsToDisable)
        {
            if (element != null)
                element.SetActive(false);
        }

        // Start the asynchronous scene loading coroutine
        StartCoroutine(LoadSceneAsync());
    }

    // Coroutine to load the scene asynchronously
    private IEnumerator LoadSceneAsync()
    {
        // Begin to load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Optionally, you can add a progress bar update here

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
