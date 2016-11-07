using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingSet : MonoBehaviour {

    private bool pressed = false;
    private DialogBoxPromptOrInfo prompt = WorldController.Instance.dialogBoxManager.dialogBoxPromptOrInfo;
    private KeyCode keyPressed;

    public void OnButtonClick()
    {
        pressed = true;
        prompt.SetPrompt("Press any key to set");
        prompt.ShowDialog();
        
    }
    void GetKeyPressed()
    {
        if (Input.anyKeyDown)
        {
            for (int i=0; i<=Enum.GetValues(typeof(KeyCode)).Length; i++)
            {
                if(Input.GetKeyDown((KeyCode)i))
                {
                    keyPressed = (KeyCode)i;
                    prompt.CloseDialog();
                }
            }
        }
    }
}
