using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class test_drawsolarsystem : MonoBehaviour
{
    public Material m_orbitLine;
    public float orbitLineWidth;
    public Texture2D[] tx_cbIcons;


    public cb_solarsystem ss;
    public List<Plotter> plotters;
    [Header("CONFIG")]
    public int focusIndex;
    public bool showOrbitLines;
    private bool orbitLinesShowing;

    public float timeScale;
    public float scaleFactor;
    private float oldScaleFactor;
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
        PlotOrbits();
    }

    void PlotOrbits()
    {
        for (int i = 2; i < ss.monoBodies.Count; i++)
        {
            Plotter comp = ss.monoBodies[i].transform.GetChild(0).GetComponent<Plotter>();
            
            comp.isShowing = true;
            comp.m_line = m_orbitLine;
            comp.useColor = false;

            comp.lineWidth = orbitLineWidth;

            comp.Plot(ss.monoBodies[i].data.pConfig.SampleFullOrbit(scaleFactor, 30));
        }
    }

    void DrawSolarSystem()
    {
        Vector3[] p = ss.GetBodyPositions(scaleFactor);

        for (int i = 0; i < p.Length; i++)
        {
            EditorGUIUtility.SetIconForObject(ss.monoBodies[i].gameObject, tx_cbIcons[ss.monoBodies[i].data.bodyType]);
            ss.monoBodies[i].transform.position = p[i] - p[focusIndex];

            if (i >= 2) 
            {ss.monoBodies[i].transform.GetChild(0).position = p[ss.monoBodies[i].data.pConfig.parentIndex] - p[focusIndex];}
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

        if (oldScaleFactor != scaleFactor)
        {
            oldScaleFactor = scaleFactor;
            PlotOrbits();
        }
    }
}
