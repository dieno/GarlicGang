using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.RPG;
using UnityEngine.UI;

namespace CommonCore.UI
{

    public class InventoryPanelController : PanelController
    {
        public GameObject ItemTemplatePrefab;
        public RectTransform ScrollContent;

        public Text SelectedItemText;

        private int SelectedItem;
        private List<string> ItemLookupTable;

        public override void SignalPaint()
        {
            SelectedItem = -1;
            PaintInventoryList();
            ClearDetailPane();
        }

        private void PaintInventoryList()
        {
            foreach(Transform t in ScrollContent)
            {
                Destroy(t.gameObject);
            }
            ScrollContent.DetachChildren();

            List<InventoryItemInstance> itemList = GameState.Instance.Player.GetInventoryModelActual().GetItemsListActual(); //ARE YOU ABSOLUTELY SURE?
            
            ItemLookupTable = new List<string>(itemList.Count);

            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                GameObject itemGO = Instantiate<GameObject>(ItemTemplatePrefab, ScrollContent);
                itemGO.GetComponentInChildren<Text>().text = item.ItemModel.Name; //for now
                Button b = itemGO.GetComponent<Button>();
                int lexI = i;
                b.onClick.AddListener(delegate { OnItemSelected(lexI); }); //scoping is weird here
                ItemLookupTable.Add(item.ItemModel.Name);
            }
        }

        public void OnItemSelected(int i)
        {
            Debug.Log(i);
            SelectedItem = i;
            PaintSelectedItem();
        }

        private void PaintSelectedItem()
        {
            SelectedItemText.text = ItemLookupTable[SelectedItem];
        }

        private void ClearDetailPane()
        {
            SelectedItemText.text = string.Empty;
        }
    }
}