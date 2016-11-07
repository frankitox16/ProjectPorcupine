using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingsDialogBox : DialogBox
{
    public GameObject KeyBindingPrefab;
    public Transform KeyList;

    private Dictionary<string, KeyboadMappedInput> Changes = new Dictionary<string, KeyboadMappedInput>();

    public override void ShowDialog()
    {
        base.ShowDialog();
        Dictionary<string, KeyboadMappedInput> KeyboardMapping = KeyboardManager.Instance.GetKeyBindings();

        foreach(KeyValuePair<string,KeyboadMappedInput> keys in KeyboardMapping)
        {
            GameObject keyListItem = (GameObject)Instantiate(KeyBindingPrefab, KeyList);
            keyListItem.transform.FindChild("keyName").GetComponent<Text>().text = keys.Key;
            keyListItem.transform.FindChild("btnKey1").GetComponentInChildren<Text>().text = keys.Value.KeyCodes[0].ToString();
            if(keys.Value.KeyCodes.Count > 1)
            {
                keyListItem.transform.FindChild("btnKey2").GetComponentInChildren<Text>().text = keys.Value.KeyCodes[1].ToString();
            }        
        }
    }

    public void SaveChanges()
    {

        base.CloseDialog();
    }
}
