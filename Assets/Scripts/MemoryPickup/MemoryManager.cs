using System;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance;
    [SerializeField] GameObject[] memoriesArray;
    int Currentindex = 0;
    [SerializeField] GameObject endGamePanel;
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

        if (memoryObjects == memoriesArray[Currentindex])
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
        Time.timeScale = 0;
    }
}
