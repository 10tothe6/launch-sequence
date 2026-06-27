using UnityEngine;

public class ui_actuator : MonoBehaviour
{
    public Vector3[] positions;

    public int currentPositionIndex;

    public bool interpolate;
    public float interpolationSpeed;

    void Awake()
    {
        GoToPositionIndex(currentPositionIndex);
    }

    // handles wrapping
    // THIS IS THE FUNCTION FOR BUTTONS TO CALL
    public void Actuate()
    {
        if (currentPositionIndex < positions.Length - 1)
        {
            GoToPositionIndex(currentPositionIndex + 1);
        } else
        {
            GoToPositionIndex(0);
        }
    }

    public void GoToPositionIndex(int index)
    {
        currentPositionIndex = index;

        if (!interpolate)
        {
            transform.localPosition = positions[currentPositionIndex];
        } else
        {
            // handled in Update() for now
        }
    }

    void Update()
    {
        if (interpolate)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, positions[currentPositionIndex], interpolationSpeed * Time.deltaTime);
        }
    }
}
