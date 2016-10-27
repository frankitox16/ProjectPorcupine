#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software, 
// and you are welcome to redistribute it under certain conditions; See 
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager
{
    private static KeyboardManager instance;

    private Dictionary<string, KeyboadMappedInput> mapping;
  
    public KeyboardManager()
    {
        instance = this;
        mapping = new Dictionary<string, KeyboadMappedInput>();
        ModalInputFields = new List<InputField>();

        TimeManager.Instance.EveryFrameNotModal += (time) => Update();

        ReadJson();
    }

    public static KeyboardManager Instance
    {
        get
        {
            if (instance == null)
            {
                new KeyboardManager();
            }

            return instance;
        }

        set
        {
            instance = value;
        }
    }

    public List<InputField> ModalInputFields { get; set; }

    public void RegisterModalInputField(InputField filterField)
    {
        if (!ModalInputFields.Contains(filterField))
        {
            ModalInputFields.Add(filterField);
        }
    }

    public void ReadJson()
    {
        string keysPath = Path.Combine(Application.streamingAssetsPath, Path.Combine("Settings", "KeyBindings.json"));
        string[] KeysData = File.ReadAllLines(keysPath);

        foreach (string keyData in KeysData)
        {
            KeyboardInputInfo inputData = JsonUtility.FromJson<KeyboardInputInfo>(keyData);
            RegisterInputMapping(inputData);
        }

    }

    public void Update()
    {
        if (ModalInputFields.Any(f => f.isFocused))
        {
            return;
        }

        foreach (KeyboadMappedInput input in mapping.Values)
        {
            input.TriggerActionIfInputValid();
        }
    }

    public void RegisterInputAction(string inputName, KeyboardMappedInputType inputType, Action onTrigger)
    {
        if (mapping.ContainsKey(inputName))
        {
            mapping[inputName].OnTrigger = onTrigger;
            mapping[inputName].Type = inputType;
        }
        else
        {
            mapping.Add(
                inputName,
                new KeyboadMappedInput
                {
                    InputName = inputName,
                    OnTrigger = onTrigger,
                    Type = inputType
                });
        }
    }

    public void RegisterInputMapping(KeyboardInputInfo KeyInfo)
    {
        RegisterInputMapping(KeyInfo.Name, KeyInfo.Modifier, KeyInfo.Keys);
    }

    public void RegisterInputMapping(string inputName, KeyboardInputModifier inputModifiers, params KeyCode[] keyCodes)
    {
        if (mapping.ContainsKey(inputName))
        {
            mapping[inputName].Modifiers = inputModifiers;
            mapping[inputName].AddKeyCodes(keyCodes);
        }
        else
        {
            mapping.Add(
                inputName,
                new KeyboadMappedInput
                {
                    InputName = inputName,
                    Modifiers = inputModifiers,
                    KeyCodes = keyCodes.ToList()
                });
        }
    }

    /// <summary>
    /// Destroy this instance.
    /// </summary>
    public void Destroy()
    {
        instance = null;
    }
}