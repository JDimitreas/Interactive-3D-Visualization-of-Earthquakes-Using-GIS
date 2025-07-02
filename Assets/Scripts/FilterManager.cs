using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameObject[] popups;

    void Start()
    {
        // clear all popups when game starts
        HideAllPopups();

        // listen to dropdown changes
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        // Always hide everything first
        HideAllPopups();

        // If "None" is selected (assume it's option 0), do nothing else
        if (index == 0)
        {
            return;
        }

        // Otherwise, show the corresponding popup (shift by -1)
        int popupIndex = index - 1;
        if (popupIndex >= 0 && popupIndex < popups.Length)
        {
            popups[popupIndex].SetActive(true);
        }
    }

    void HideAllPopups()
    {
        foreach (var p in popups)
        {
            if (p != null) p.SetActive(false);
        }
    }
}
