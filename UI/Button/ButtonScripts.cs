using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScripts : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    public void OnDeselect(BaseEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuSFX_Picker.instance.OnButtonSelected();
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuSFX_Picker.instance.OnButtonSelected();
    }
}
