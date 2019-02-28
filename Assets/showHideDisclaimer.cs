using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showHideDisclaimer : MonoBehaviour
{
    public GameObject disclaimerGroup;

    public void onPress()
    {
        CanvasGroup cg = disclaimerGroup.GetComponent<CanvasGroup>();
        if (cg.GetComponent<CanvasGroup>().alpha == 1)
        {
            cg.interactable = false;
            cg.alpha = 0;
        } else
        {
            cg.interactable = true;
            cg.alpha = 1;
        }
    }
}
