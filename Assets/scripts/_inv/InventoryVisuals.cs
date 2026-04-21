// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// // I want the inventory to look more polished, e.g. the selected cell scaling up a bit

// // since I won't want this functionality for every game, I'm keeping it in this script for now

// public class InventoryVisuals : MonoBehaviour
// {
//     public Sprite unheld;
//     public Sprite held;

//     private Inventory inv;
//     public Transform container;

//     // how big the selected cell should bee
//     public float selectedScaleMultiplier;
//     public float lerpSpeed;

//     // keeping track of width/height stuff behind the scenes
//     private float defaultWidth;
//     private float scaledWidth;

//     private bool isInitialized;

//     public OpacityController itemNameController;

//     private int oldSelectedIndex;

//     void Awake()
//     {
//         inv = GetComponent<Inventory>();
//     }

//     void Initialize()
//     {
//         defaultWidth = container.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
//         scaledWidth = defaultWidth * selectedScaleMultiplier;

//         isInitialized = true;

//         inv.onCellSelect.AddListener(ShowItemName);
//     }

//     public void ShowItemName()
//     {
//         if (inv.items[inv.selectedCell] == null) return;
//         itemNameController.textComp.text = inv.items[inv.selectedCell].GetClass().name;
//         itemNameController.Flash();
//     }

//     void FixedUpdate()
//     {
//         if (inv.isActiveAndEnabled && !isInitialized)
//         {
//             Initialize();
//         }

//         if (isInitialized)
//         {
//             int selected = inv.selectedCell;

//             for (int i = 0; i < transform.childCount; i++)
//             {
//                 Color c = transform.GetChild(i).GetChild(0).GetComponent<Image>().color;
//                 if (inv.items[i] != null)
//                 {
//                     transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1);
//                 } else {transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);}
//                 if (i == selected)
//                 {
//                     transform.GetChild(i).GetComponent<Image>().sprite = held;
//                     // transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta, new Vector2(scaledWidth, scaledWidth), lerpSpeed);
//                     // transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(transform.GetChild(i).GetComponent<RectTransform>().sizeDelta, new Vector2(scaledWidth, scaledWidth), lerpSpeed);
//                 }
//                 else
//                 {
//                     transform.GetChild(i).GetComponent<Image>().sprite = unheld;
//                     // transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta, new Vector2(defaultWidth, defaultWidth), lerpSpeed);
//                     // transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(transform.GetChild(i).GetComponent<RectTransform>().sizeDelta, new Vector2(defaultWidth, defaultWidth), lerpSpeed);
//                 }
//             }

//             oldSelectedIndex = selected;
//         }
//     }
// }
