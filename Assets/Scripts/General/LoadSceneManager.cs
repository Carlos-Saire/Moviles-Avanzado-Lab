using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class LoadSceneManager : NetworkBehaviour
{
    private static string currentScene;
    public void LoadScene(string scene)
    {
        currentScene = scene;
        NetworkManager.Singleton.SceneManager.LoadScene(scene , LoadSceneMode.Single);
    }
    public void RebootScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }
    public void Close()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
