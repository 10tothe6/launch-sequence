using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ui_button : MonoBehaviour
{
    public bool isClickable = true;
    private bool isPressed;

    [Space(6)]
    [Header("Interactions")]
    public UnityEvent onPress; // Click (runs once)
    public UnityEvent onDrag; // Click and then move mouse away (runs once)
    public UnityEvent whilePress; // Hold click (repeats)

    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent whileHover;
    private bool isHovering;

    [Space(6)]
    [Header("Color Settings")]
    public bool colorSwitch;
    public bool leaveAlpha = true; // don't change the alpha of the button (allows other scripts to control it)
    public Color defaultColor;
    public Color hoverColor;
    public Color pressedColor;

    private Image i;

    void Awake()
    {
        i = GetComponent<Image>();
    }

    void Update() {
        //if (!isClickable) {return;}

        if (Input.mouseButtonDownLeft && ui_canvasutils.IsCursorInteract(gameObject, true)) {
            isPressed = true;
        }

        if (isPressed) {
            whilePress.Invoke(); // Invoke the event that runs when the button is held

             if (colorSwitch) { SetColor(pressedColor); }
        }
        else if (ui_canvasutils.IsCursorInteract(gameObject, true)) {
            if (colorSwitch) { SetColor(hoverColor); }
            whileHover.Invoke();
            if (!isHovering) {onHoverEnter.Invoke(); isHovering = true;}
        }
        else {
            if (colorSwitch) { SetColor(defaultColor); }
            if (isHovering) {onHoverExit.Invoke(); isHovering = false;}
        }   

        if (!Input.mouseButtonDownLeft) {
            if (isPressed && ui_canvasutils.IsCursorInBounds(gameObject, true)) { onPress.Invoke(); }
            isPressed = false;
        }

        if (isPressed && !ui_canvasutils.IsCursorInteract(gameObject, true)) {
            onDrag.Invoke(); // Invoke the unity event that runs when you click and drag the button
            isPressed = false;
        }
    }

    public void SetColor(Color col)
    {
        if (leaveAlpha)
        {
            i.color = new Color(col.r, col.g, col.b, i.color.a);
        } else
        {
            i.color = col;
        }
    }

    public void TestButton() {
        Debug.Log("Test Succesful?");
    }
}