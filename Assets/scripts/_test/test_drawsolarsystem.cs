using System.Collections.Generic;
using UnityEngine;

public class test_drawsolarsystem : MonoBehaviour
{
    public cb_solarsystem ss;
    public List<Plotter> plotters;
    [Header("CONFIG")]
    public int focusIndex;
    public bool showOrbitLines;
    private bool orbitLinesShowing;

    public float timeScale;
    public float scaleFactor;
    public bool regenerate;

    public bool playSimulation;
    public float time;

    void Start()
    {
        focusIndex = 0;
        MakeSolarSystem();
    }

    void MakeSolarSystem()
    {
        ss.Generate();
    }

    void DrawSolarSystem()
    {
        Vector3[] p = ss.GetBodyPositions(scaleFactor);

        for (int i = 0; i < p.Length; i++)
        {
            ss.monoBodies[i].transform.position = p[i] - p[focusIndex];
        }
    }

    void Update()
    {
        if (playSimulation)
        {
            time += Time.deltaTime * timeScale;
        }

        if (regenerate)
        {
            regenerate = false;
            MakeSolarSystem();
        }

        if (showOrbitLines != orbitLinesShowing)
        {
            orbitLinesShowing = showOrbitLines;
            for (int i = 0; i < plotters.Count; i++)
            {
                plotters[i].isShowing = orbitLinesShowing;
            }
        }

        ss.SetTimeOffset(time);
        DrawSolarSystem();
    }
}
