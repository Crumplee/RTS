using UnityEngine;
using System.Collections;
using RTS;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GUISkin mySkin;
    public Texture2D header;

    protected string[] buttons;
    protected string[] inputs;

    protected virtual void Start()
    {
        SetButtons();
        SetInputs();
    }

    protected virtual void OnGUI()
    {
        DrawMenu();
    }

    protected virtual void DrawMenu()
    {
        GUI.skin = mySkin;
        float menuHeight = GetMenuHeight();

        float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
        float groupTop = Screen.height / 2 - menuHeight / 2;
        GUI.BeginGroup(new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, menuHeight));


        GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, menuHeight), "");

        GUI.DrawTexture(new Rect(ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);

        float leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
        float topPos = 2 * ResourceManager.Padding + header.height;

        if (inputs != null)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
                GUI.TextField(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.InputHeight), inputs[i]);
                
               

            }
            topPos += ResourceManager.InputHeight + ResourceManager.Padding;
        }

        //menu buttons
        if (buttons != null)
        {
            
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
                if (GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), buttons[i]))
                {
                    HandleButton(buttons[i]);
                }
                Debug.Log(topPos);
            }
        }

        

        GUI.EndGroup();
    }

    protected virtual void SetButtons()
    {

    }

    protected virtual void SetInputs()
    {

    }

    protected virtual void HandleButton(string text)
    {

    }

    protected virtual float GetMenuHeight()
    {
        float buttonHeight = 0;
        float inputHeight = 0;
        float paddingHeight = 2 * ResourceManager.Padding;

        if (buttons != null) buttonHeight = buttons.Length * ResourceManager.ButtonHeight;
        if (buttons != null) paddingHeight += buttons.Length * ResourceManager.Padding;
        if (inputs != null) inputHeight += inputs.Length * (ResourceManager.ButtonHeight / 2);
        if (inputs != null) paddingHeight += inputs.Length * ResourceManager.InputHeight;
        return ResourceManager.HeaderHeight + buttonHeight + inputHeight + paddingHeight;
    }

    protected void ExitGame()
    {
        Application.Quit();
    }
}