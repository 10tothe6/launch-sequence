using UnityEngine.UI;
using UnityEngine;
using TMPro;

// doesn't have anything to do with actual parts,
// just 

public class crft_partpreview : MonoBehaviour
{   
    public Image i_partIcon;
    public TextMeshProUGUI tx_partName;

    public ui_button button;

    private string part_name;

    void Awake()
    {
        GetComponent<ui_instantiatable>().onDataUpdate.AddListener(DisplayPreview);
    }

    public void DisplayPreview(string part_name)
    {
        this.part_name = part_name;

        // first, the icon
        i_partIcon.sprite = PartManager.Instance.GetPartPrefabFromName(part_name).GetComponent<crft_genericpart>().part_icon;
        
        // a name is probably necessary too
        tx_partName.text = part_name;

        // button interactions, 
        // for starting the building process for whatever part we clicked:

        // this is okay for now, but we have to make this more modular later when we also make it a part of the ship editor
        button.onPress.AddListener(() => UIManager.Instance.buildWidget.StartBuildingPart(part_name));
    }
}
