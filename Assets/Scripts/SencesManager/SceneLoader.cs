using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image rotatingImage; 
    [SerializeField] private TextMeshProUGUI loadingText;

    [SerializeField] private float rotationSpeed = 200f; 

    public void LoadScene(int sceneIndx)
    {
        StartCoroutine(LoadAsync(sceneIndx));
    }

    private IEnumerator LoadAsync(int sceneIndx)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndx);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            rotatingImage.transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);

            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";

            if (progress >= 1f)
            {
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
