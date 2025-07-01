using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PopupSelectDifficultAI : BasePopup
{
    [SerializeField] private Vector2 offSet;
    [SerializeField] private TMP_Dropdown difficultDropdown;
    private List<AIDepthLevel> depthOptions;


    private void Start()
    {
        SetOffSet();
        InitDropdown();
    }

    private void InitDropdown()
    {
        if (difficultDropdown != null)
        {
            depthOptions = Enum.GetValues(typeof(AIDepthLevel))
                      .Cast<AIDepthLevel>()
                      .Skip(1)
                      .ToList();

            List<string> optionNames = depthOptions.Select(e => e.ToString()).ToList();
            difficultDropdown.ClearOptions();
            difficultDropdown.AddOptions(optionNames);
        }
        difficultDropdown.onValueChanged.AddListener(OnClickSelectDifficule);
    }    
    private void SetOffSet()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if(rectTransform != null)
        {
            rectTransform.anchoredPosition = offSet;
        } 
            
    }    
    private void OnClickSelectDifficule(int index)
    {
        AIDepthLevel selected = depthOptions[index];
        Debug.Log($"?ã ch?n: {selected} - Giá tr?: {(int)selected}");
        if (GameManager.HasInstance)
        {
            GameManager.Instance.SetAIDepthLevel(selected);
        }
    }    
}
