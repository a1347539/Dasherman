using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static string lastScene;

    public static void LoadScene(string currentScene, string newScene) {
        lastScene = currentScene;
        SceneManager.LoadScene(newScene);
    }
}
