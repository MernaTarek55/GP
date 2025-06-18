
using System.Collections;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance;
    [SerializeField] private GameObject[] memoriesArray;
    private int Currentindex = 0;
    private bool hasStartedLoading = false;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string gameSceneName = "Real Level 2";
    [SerializeField] private GameObject endGamePanel;
    private void Awake()
    {
        Instance = this;
    }

    public bool PickingUpMemory(GameObject memoryObjects)
    {
        if (Currentindex >= memoriesArray.Length)
        {
            return false;
        }

        GameObject expectedMemory = memoriesArray[Currentindex];

        if (memoryObjects.transform == expectedMemory.transform ||
            memoryObjects.transform.IsChildOf(expectedMemory.transform))
        {
            Debug.Log("Correct memory picked. Index is now: {currentMemoryIndex}");
            Currentindex++;

            if (Currentindex == memoriesArray.Length)
            {
                Debug.Log("All memories are picked");
                AllMemoriesPicked();
            }

            return true;
        }

        return false;
    }

    private void AllMemoriesPicked()
    {
        Debug.Log("All memories are picked");
        endGamePanel.SetActive(true);
        hasStartedLoading = true;
        StartCoroutine(WaitAndLoadScene());
        Time.timeScale = 0; 
    }

    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSecondsRealtime(15f); 
        Time.timeScale = 1; 
        sceneLoader.LoadScene(gameSceneName);
    }

}
