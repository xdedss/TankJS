using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotSelect : MonoBehaviour
{
    public static List<BotSelect> instances = new List<BotSelect>();

    public static void Add(BotSelect original)
    {
        var copy = Instantiate(original.gameObject);
        copy.transform.SetParent(original.transform.parent);
        copy.transform.SetSiblingIndex(copy.transform.GetSiblingIndex() - 1);
        foreach (var b in instances)
        {
            b.RefreshInfo();
        }
    }

    public static void Del(BotSelect target)
    {
        if(instances.Count > 1)
        {
            instances.Remove(target);
            Destroy(target.gameObject);
            foreach (var b in instances)
            {
                b.RefreshInfo();
            }
        }
    }

    public InputField tankNameInput;
    public Dropdown dropdown;
    public GameObject[] additionalInfo;
    public Button addButton;
    public Button delButton;

    [Space]
    public Dropdown keySetDropdown;
    public InputField fileNameInput;

    [Space]
    public string tankName;


    public void OnNameInputEdited()
    {
        tankName = tankNameInput.text;
    }

    public void OnDropdownChanged(int value)
    {
        RefreshInfo();
    }

    public void OnAddButtonClicked()
    {
        Add(this);
    }

    public void OnDelButtonClicked()
    {
        Del(this);
    }

    private void RefreshInfo()
    {
        for(int i = 0; i < additionalInfo.Length; i++)
        {
            if (additionalInfo[i])
            {
                additionalInfo[i].SetActive(i == dropdown.value);
            }
        }
        delButton.interactable = instances.Count > 2;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public IBot GetBot()
    {
        switch (dropdown.value)
        {
            case 0:
                return new ScriptBot(Configurations.BotFolder + fileNameInput.text + ".js");
            case 1:
                return new InteractiveBot((InteractiveBot.KeySet)keySetDropdown.value);
        }
        return new NoBot();
    }

    void Awake()
    {
        instances.Add(this);
    }

    void Start()
    {
        //Debug.Log(System.Environment.CurrentDirectory);
        RefreshInfo();
    }
    
    void Update()
    {
        
    }
}
