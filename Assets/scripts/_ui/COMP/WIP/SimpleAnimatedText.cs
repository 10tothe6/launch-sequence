using TMPro;
using UnityEngine;

// component for writing text in a cool way, used mainly for dialog
public class SimpleAnimatedText : MonoBehaviour
{
    [Header("References")]
    public string textValue;
    private bool isRunning;

    private float lastStep;
    private int stepCount;

    [Header("Config")]
    public float stepInterval;
    public float spacing;
    public float fontSize;
    public bool playAudio;

    public GameObject mainText;

    private float unpauseTime;
    private bool isPausing;
    public bool isFinished;

    public bool hasSkipped;
    public Color textColor;

    void Awake()
    {
        mainText.GetComponent<TMP_Text>().text = "";

        hasSkipped = false;
    }
    
    void Update() {
        //if (UIManager.Instance.hasContinueKeyBeenPressed && isRunning) { UIManager.Instance.hasContinueKeyBeenPressed = false; isFinished = true; hasSkipped = true;  }

        if (isPausing)
        {
            if (unpauseTime < Time.time)
            {
                isPausing = false;
                mainText.GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        if (isRunning && lastStep + stepInterval < Time.time && !isPausing)
        {
            mainText.GetComponent<TextMeshProUGUI>().text += textValue[stepCount].ToString();

            lastStep = Time.time;
            stepCount++;

            if (playAudio)
            {
                //AudioManager.Instance.PlaySound(1, true);
            }

            if (stepCount >= textValue.Length)
            {
                isRunning = false;
                isFinished = true;
            }
        }
    }

    void Pause(float amt)
    {
        isPausing = true;
        unpauseTime = Time.time + amt;
    }

    public void Clear()
    {
        isRunning = false;
        mainText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void AddToMain(string toAdd)
    {
        mainText.GetComponent<TextMeshProUGUI>().text += toAdd;
    }

    public void AnimateIn(string text)
    {
        textValue = text;
        AnimateIn();
    }

    public void AnimateIn()
    {
        hasSkipped = false;

        mainText.GetComponent<TMP_Text>().text = "";

        isPausing = false;

        isRunning = true;
        lastStep = Time.time;
        stepCount = 0;
        isFinished = false;

        mainText.GetComponent<TMP_Text>().color = textColor;

        mainText.GetComponent<TMP_Text>().fontSize = fontSize;
    }
}
