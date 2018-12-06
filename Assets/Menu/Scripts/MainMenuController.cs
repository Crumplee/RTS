using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class MainMenuController : MonoBehaviour {

    public NetworkManager networkManager;
    public InputField input;
    public Button host, join, exit;

    public string ip;

    void Awake()
    {
        networkManager = this.GetComponent<NetworkManager>();
        host.onClick.AddListener(HostGame);
        join.onClick.AddListener(JoinGame);
        exit.onClick.AddListener(ExitGame);
    }

    void HostGame()
    {
        networkManager.StartHost();
    }

    void JoinGame()
    {
        string ip = input.text;

        Match match = Regex.Match(ip, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

        if (match.Success)
        {
            networkManager.networkAddress = ip;
            networkManager.StartClient();
        }
    }

    void ExitGame()
    {
        Application.Quit();
    }
    
}
