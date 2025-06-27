using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance;
    [SerializeField] private GameObject[] memoriesArray;
    private int currentIndex = 0;
    private bool hasStartedLoading = false;
    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        Instance = this;
    }

    public bool PickingUpMemory(GameObject memoryObject)
    {
        if (currentIndex >= memoriesArray.Length)
        {
            return false;
        }

        GameObject expectedMemory = memoriesArray[currentIndex];

        if (memoryObject.transform == expectedMemory.transform ||
            memoryObject.transform.IsChildOf(expectedMemory.transform))
        {
            Debug.Log($"Correct memory picked. Index is now: {currentIndex}");
            currentIndex++;

            if (currentIndex == memoriesArray.Length)
            {
                Debug.Log("All memories are picked");
                StartCoroutine(WaitAndLoadNextScene());
            }

            return true;
        }
        return false;
    }

    IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1;

        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = activeSceneIndex + 1;

        Debug.Log("Current Scene Index: " + activeSceneIndex);
        Debug.Log("Next Scene Index: " + nextIndex);
        Debug.Log("Total Scenes: " + SceneManager.sceneCountInBuildSettings);

        // If it's the last scene, loop back to the first one
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextIndex = 0;
        }

        sceneLoader.LoadScene(nextIndex);
    }


}