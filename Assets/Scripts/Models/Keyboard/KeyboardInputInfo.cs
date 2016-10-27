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
using System.Text;
using UnityEngine;

public class KeyboardInputInfo
{
    
    public string Name;
    public KeyboardInputModifier Modifier;
    public KeyCode[] Keys;
    
    public KeyboardInputInfo(string Name, KeyboardInputModifier Modifier, params KeyCode[] args)
    {
        this.Name = Name;
        this.Modifier = Modifier;
        this.Keys = args;

        string Data = JsonUtility.ToJson(this);
        Data = Data + "\n";

        File.AppendAllText(Path.Combine(Application.streamingAssetsPath, Path.Combine("Settings", "KeyBindings.json")), Data);
    }

}