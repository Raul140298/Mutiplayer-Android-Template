using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnHostButtonPressed()
    {
        ServerManager.Instance.StartHost();
    }

    public void OnServerButtonPressed()
    {
        ServerManager.Instance.StartServer();
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
