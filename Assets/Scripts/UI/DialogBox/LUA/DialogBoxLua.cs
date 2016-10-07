﻿#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software, 
// and you are welcome to redistribute it under certain conditions; See 
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;

[MoonSharpUserData]
public class DialogBoxLua : DialogBox
{
    public EventActions events;

    private string title;

    private List<object> extraData;

    public Transform Content { get; protected set; }

    public DialogBoxResult Result { get; set; }

    /// <summary>
    /// Gets or sets the title of the DialogBox.
    /// </summary>
    public string Title
    {
        get
        {
            return title;
        }

        protected set
        {
            title = value;
            transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = value;
        }
    }

    public override void ShowDialog()
    {
        base.ShowDialog();
        if (events.HasEvent("OnShow") == true)
        {
            events.Trigger("OnShow", this);
        }
    }

    public void YesButtonClicked()
    {
        Result = DialogBoxResult.Yes;
        CloseDialog();
    }

    public void NoButtonClicked()
    {
        Result = DialogBoxResult.No;
        CloseDialog();
    }

    public void CancelButtonClicked()
    {
        Result = DialogBoxResult.Cancel;
        CloseDialog();
    }

    public void OkButtonClicked()
    {
        Result = DialogBoxResult.Okay;
        CloseDialog();
    }

    public override void CloseDialog()
    {
        foreach (DialogControl control in Content.GetComponentsInChildren<DialogControl>())
        {            
            extraData.Add(control.result);
        }

        if (events.HasEvent("OnClosed") == true)
        {
            events.Trigger("OnClosed", this, Result, extraData);
        }

        base.CloseDialog();
    }

    /// <summary>
    /// Loads the LUA Dialog Box from the XML file.
    /// </summary>
    /// <param name="file">The FileInfo object that references to the XML file.</param>
    public void LoadFromXML(FileInfo file)
    {
        // TODO: Find a better way to do this. Not user friendly/Expansible.
        // DialogBoxLua -> Dialog Background
        //                 |-> Title
        //                 |-> Content
        Content = transform.GetChild(0).GetChild(1);

        XmlSerializer serializer = new XmlSerializer(typeof(DialogBoxLuaInformation));
        
        try
        {
            DialogBoxLuaInformation dialogBoxInfo = (DialogBoxLuaInformation)serializer.Deserialize(file.OpenRead());
            Title = dialogBoxInfo.title;
            foreach (DialogComponent gameObjectInfo in dialogBoxInfo.content)
            {
                // Implement new DialogComponents in here.
                switch (gameObjectInfo.ObjectType)
                {
                    case "Text":
                        GameObject textObject = (GameObject)Instantiate(Resources.Load("Prefab/DialogBoxPrefabs/DialogText"), Content);
                        textObject.GetComponent<Text>().text = (string)gameObjectInfo.data;
                        textObject.GetComponent<RectTransform>().anchoredPosition = gameObjectInfo.position;
                        break;
                    case "Input":
                        GameObject inputObject = (GameObject)Instantiate(Resources.Load("Prefab/DialogBoxPrefabs/DialogInputComponent"), Content);
                        inputObject.GetComponent<RectTransform>().anchoredPosition = gameObjectInfo.position;
                        inputObject.GetComponent<RectTransform>().sizeDelta = gameObjectInfo.size;
                        break;
                    case "Image":
                        GameObject imageObject = (GameObject)Instantiate(Resources.Load("Prefab/DialogBoxPrefabs/DialogImage"), Content);
                        Texture2D imageTexture = new Texture2D((int)gameObjectInfo.size.x, (int)gameObjectInfo.size.y);
                        try
                        {
                            imageTexture.LoadImage(File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, (string)gameObjectInfo.data)));
                            Sprite imageSprite = Sprite.Create(imageTexture, new Rect(0, 0, gameObjectInfo.size.x, gameObjectInfo.size.y), Vector2.zero);

                            imageObject.GetComponent<Image>().sprite = imageSprite;
                            imageObject.GetComponent<RectTransform>().anchoredPosition = gameObjectInfo.position;
                            imageObject.GetComponent<RectTransform>().sizeDelta = gameObjectInfo.size;
                        }
                        catch (System.Exception error)
                        {
                            Debug.ULogErrorChannel("DialogBoxLua", "Error converting image:" + error.Message);
                            return;                            
                        }

                        break;
                    case "Button":
                        GameObject buttonObject = (GameObject)Instantiate(Resources.Load("Prefab/DialogBoxPrefabs/DialogButton"));
                        buttonObject.GetComponent<RectTransform>().anchoredPosition = gameObjectInfo.position;
                        buttonObject.GetComponent<RectTransform>().sizeDelta = gameObjectInfo.size;
                        buttonObject.GetComponentInChildren<Text>().text = (string)gameObjectInfo.data;
                        break;
                }
            }

            // Enable dialog buttons from the list of buttons.
            foreach (DialogBoxResult buttons in dialogBoxInfo.buttons)
            {
                switch (buttons)
                {
                    case DialogBoxResult.Yes:
                        gameObject.transform.GetChild(0).transform.Find("Buttons/btnYes").gameObject.SetActive(true);
                        break;
                    case DialogBoxResult.No:
                        gameObject.transform.GetChild(0).transform.Find("Buttons/btnNo").gameObject.SetActive(true);
                        break;
                    case DialogBoxResult.Cancel:
                        gameObject.transform.GetChild(0).transform.Find("Buttons/btnCancel").gameObject.SetActive(true);
                        break;
                    case DialogBoxResult.Okay:
                        gameObject.transform.GetChild(0).transform.Find("Buttons/btnOK").gameObject.SetActive(true);
                        break;
                }
            }

            events = dialogBoxInfo.events;
            FunctionsManager.Get("DialogBoxLua").RegisterGlobal(typeof(DialogBoxLua));
            extraData = new List<object>();
        }
        catch (System.Exception error)
        {
            Debug.ULogErrorChannel("DialogBoxLua", "Error deserializing data:" + error.Message);
        }
    }
}
