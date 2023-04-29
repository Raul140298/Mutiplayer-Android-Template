using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameplaySceneName;

    public void OnHostButtonPressed()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    public void OnServerButtonPressed()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    public void OnClientButtonPressed()
    {
        Debug.Log("CLIENTE");
        NetworkManager.Singleton.StartClient();
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
