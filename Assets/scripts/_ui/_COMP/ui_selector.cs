using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// TODO: make all of the ushorts into ints?

// a ui element with two arrows, allowing you to select something
public class ui_selector : MonoBehaviour
{
    // TODO: change these to custom buttons after overhauling the UI
    public Button negativeArrow;
    public Button positiveArrow;

    public Sprite[] possibleIcons;
    public string[] possibleTexts;

    public ushort itemCount;
    public ushort currentIndex;


    public bool useText;
    public bool useSprite;
    public Image iconDisplay;
    public TextMeshProUGUI textDisplay;

    public UnityEvent<ushort> onValueChanged;

    private ui_instantiatable iComp;

    

    void Awake()
    {
        if (useText && useSprite)
        {
            itemCount = (ushort)Mathf.Max(possibleIcons.Length, possibleTexts.Length);

            // warning if the counts are different
            if (possibleIcons.Length != possibleTexts.Length)
            {
                cmd.LogError("Text and icon arrays are not the same length!");
            }
        } else if (useSprite)
        {
            itemCount = (ushort)possibleIcons.Length;
        } else if (useText) 
        {
            itemCount = (ushort)possibleTexts.Length;
        } else
        {
            // could in theory use neither ig
        }

        negativeArrow.onClick.AddListener(DecreaseValue);
        positiveArrow.onClick.AddListener(IncreaseValue);

        SetValue(0);

        iComp = GetComponent<ui_instantiatable>();
    }

    public void SetValue(ushort index)
    {
        currentIndex = index;

        UpdateDisplay();

        // any additional logic that runs as the component is changed
        onValueChanged.Invoke(currentIndex);

        if (iComp != null )
        {
            iComp.SetData(currentIndex.ToString());
        }
    }

    public void UpdateDisplay()
    {
        if (useSprite)
        {
            if (iconDisplay == null) {cmd.LogError("icon display is null");}

            else
            {
                iconDisplay.sprite = possibleIcons[currentIndex];
            }
        }
        if (useText)
        {
            if (textDisplay == null) {cmd.LogError("text display is null");}
            
            else
            {
                textDisplay.text = possibleTexts[currentIndex];
            }
        }
    }


    // below 2 functions handle wrapping as well

    public void IncreaseValue()
    {
        if (currentIndex < itemCount - 1)
        {
            SetValue((ushort)(currentIndex + 1));
        } else
        {
            SetValue(0);
        }
    }

    public void DecreaseValue()
    {
        if (currentIndex > 0)
        {
            SetValue((ushort)(currentIndex - 1));
        } else
        {
            SetValue((ushort)(itemCount - 1));
        }
    }
}
