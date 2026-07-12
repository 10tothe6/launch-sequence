using System.Collections.Generic;
using UnityEngine;

public class test_showcanistercontents : MonoBehaviour
{
    public List<crft_resourcecompartement> compartements;

    void Start()
    {
        GetComponent<ui_canisterwidget>().BuildWidget("Test Canister", compartements);
    }
}
