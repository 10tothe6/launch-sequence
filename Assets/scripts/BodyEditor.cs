using UnityEngine;

// this is a dev tool, because I like dev tools
// it IS a part of the main scene, to make things faster

public class BodyEditor : MonoBehaviour
{
    private static BodyEditor _instance;

    public static BodyEditor Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
        comp = CameraController.cam_main.GetComponent<cbr_applyatmosphere>();
    }

    private cbr_applyatmosphere comp;
    private bool isActive;

    [Header("BODY CONFIG")]
    public Vector3 waveLengths;
    public float scatterStrength;

    // using the inspector to allow the user to edit settings
    // (FOR NOW)
    public cbr_atmosphererenderingdata atmosphereData;

    public void SetupEditor()
    {
        CameraController.SetControlMode(CameraControlMode.BodyEditor);
        isActive = true;
        comp.enabled = true;
    }

    void Update()
    {
        if (isActive)
        {
            CameraController.Instance.UpdateCamera();

            UpdateScatterCoefficients();

            // basically, we're hijacking the camera's atmosphere rendering shader in order to render a test atmosphere
            comp.ApplyAtmosphereRenderingData(new cbr_atmosphererenderingdata[] {atmosphereData});
            // updating the shader vars will run automatically from Graphics.Blit()
        }
    }

    void UpdateScatterCoefficients()
    {
        Vector3 result = new Vector3(
            Mathf.Pow(400f / waveLengths.x, 4) * scatterStrength,
            Mathf.Pow(400f / waveLengths.y, 4) * scatterStrength,
            Mathf.Pow(400f / waveLengths.z, 4) * scatterStrength);

        atmosphereData.scatterCoefficients = result;
    }
}
