using TMPro;
using UnityEngine;
using UnityEngine.UI;

// standardized class for dealing with the item icon itself, the background, and of course the item count display

public class ui_itemdisplay : MonoBehaviour
{
    public TextMeshProUGUI tx_itemCount;
    public Image i_itemBG;
    public Image i_itemIcon;

    public void Draw(inv_itemstack itemData)
    {
        tx_itemCount.text = "x" + itemData.itemCount;
        
        i_itemIcon.sprite = itemData.GetData().icon;
    }
}
