using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using RTS;

public class MainMenu : Menu
{

    protected override void SetButtons()
    {
        buttons = new string[] { "Join", "Host", "Quit Game" };
    }

    protected override void SetInputs()
    {
        inputs = new string[] { "IP address" };
    }

    protected override void HandleButton(string text)
    {
        switch (text)
        {
            case "Host":
                HostGame();
                break;
            case "Join":
                JoinGame();
                break;
            case "Quit Game":
                ExitGame();
                break;
            default:
                break;
        }
    }

    private void NewGame()
    {
        
        ResourceManager.MenuOpen = false;
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1.0f;
        

    }

    private void HostGame()
    {

    }

    private void JoinGame()
    {

    }
}