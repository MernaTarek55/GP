using System.Collections;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance;
    [SerializeField] private GameObject[] memoriesArray;
    private int currentIndex = 0;
    private bool hasStartedLoading = false;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string gameSceneName = "Real Level 2";

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
                StartCoroutine(WaitAndLoadScene());
            }

            return true;
        }

        return false;
    }

    

    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1;
        sceneLoader.LoadScene(gameSceneName);
    }
}