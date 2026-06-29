using UnityEngine;

public class crft_drill : MonoBehaviour
{
    private crft_genericpart gp;
    public bool isDrillRunning;
    
    public float minDrillInterval;
    public float maxDrillInterval;
    private float currentDrillInterval;
    private float drillStartTime;

    void Awake() {
        gp = GetComponent<crft_genericpart>();
    }

    public void ToggleDrill()
    {
        if (isDrillRunning)
        {
            StopDrill();
        } else
        {
            StartDrill();
        }
    }

    public void StartDrill()
    {
        ResetDrillTimer();
        isDrillRunning = true;
    }

    public void StopDrill()
    {
        isDrillRunning = false;
    }

    void Update()
    {
        if (isDrillRunning)
        {
            if (Time.time > drillStartTime + currentDrillInterval)
            {
                ProduceItem();
            }
        }
    }

    void ResetDrillTimer()
    {
        currentDrillInterval = Random.Range(maxDrillInterval, minDrillInterval);
        drillStartTime = Time.time;
    }


    // an item was mined, so either add it to the crate if its connected or just spawn the item
    public void ProduceItem()
    {
        ResetDrillTimer();
    }
}
