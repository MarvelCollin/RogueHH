using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSkillUI : MonoBehaviour
{
    public Image skillImage { get; set; }
    public Image shadow { get; set; }
    public Text lockedText { get; set; }

    public abstract void UpdateUI();

    public void HideLockedTextAndShadow()
    {
        if (lockedText != null)
        {
            lockedText.gameObject.SetActive(false);
        }
        if (shadow != null)
        {
            shadow.gameObject.SetActive(false);
        }
    }

    public void ShowLockedTextAndShadow()
    {
        if (lockedText != null)
        {
            lockedText.gameObject.SetActive(true);
        }
        if (shadow != null)
        {
            shadow.gameObject.SetActive(true);
        }
    }
}