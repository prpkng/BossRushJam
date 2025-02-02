using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipOnWindows : MonoBehaviour
{
    private void Awake()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}