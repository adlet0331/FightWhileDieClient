using System.Collections.Generic;
using Data;
using NonDestroyObject;
using UI;
using UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : View
{
    [Header("Components")]
    [SerializeField] private List<ItemSlot> itemSlotList;
    [SerializeField] private Transform viewPortTransform;
    [SerializeField] private Toggle rareToggle;
    [SerializeField] private Toggle levelToggle;
    [SerializeField] private Toggle optionToggle;
    [SerializeField] private Button ascDescButton;

    [Header("Debugging")]
    [SerializeField] private bool isAsc;

    public void ChangeAscDesc()
    {
        isAsc = !isAsc;
        ascDescButton.transform.Rotate(0, 0, 180);
        Sort();
    }

    private event SlotClickHandler SlotClickHandler;
    /// <summary>
    /// Must Called Before View.Activate Function
    /// </summary>
    /// <param name="handler"></param>
    public void BindSlotClickHandler(SlotClickHandler handler)
    {
        SlotClickHandler = handler;
        SlotClickHandler += SlotClicked;
    }

    private void SlotClicked(int index)
    {
        
    }
    
    protected override void Init()
    {
        var itemList = DataManager.Instance.ItemManager.ItemEquipments;

        for (var i = viewPortTransform.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(viewPortTransform.transform.GetChild(i).gameObject);
        }
        
        itemSlotList = new List<ItemSlot>();
        var prefab = Resources.Load("Prefabs/UI/Inventory/ItemSlot") as GameObject;
        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject obj = Instantiate(prefab, viewPortTransform);
            obj.GetComponent<ItemSlot>().Init(i, itemList[i], SlotClickHandler);
            itemSlotList.Add(obj.GetComponent<ItemSlot>());
        }
        
        Sort();
    }
    protected override void Clean()
    {
        
    }

    public void Sort()
    {
        var rareSort = rareToggle.isOn;
        var levelSort = levelToggle.isOn;
        var optionSort = optionToggle.isOn;

        if (!rareSort && !levelSort && !optionSort) return;

        QuickSort(0, itemSlotList.Count - 1, rareSort, levelSort, optionSort);
    }

    private void ChangeObjectIndex(int small, int large)
    {
        itemSlotList[large].transform.SetSiblingIndex(small);
        itemSlotList[small].transform.SetSiblingIndex(large);
        (itemSlotList[small], itemSlotList[large]) = (itemSlotList[large], itemSlotList[small]);
        itemSlotList[small].IndexChanged(small);
        itemSlotList[large].IndexChanged(large);
    }
    private void QuickSort(int minIndex, int maxIndex, bool rareSort, bool levelSort, bool optionSort)
    {
        if (minIndex >= maxIndex)
            return;
        
        var asc = isAsc ? 1 : -1;
        
        var pivot = itemSlotList[minIndex];
        var low = minIndex;
        var large = maxIndex;
        while (low <= large)
        {
            while ((rareSort && 
                    asc * itemSlotList[low].ItemEquipmentInfo.rare < asc * pivot.ItemEquipmentInfo.rare) ||
                   (
                       levelSort &&
                       (!rareSort || itemSlotList[low].ItemEquipmentInfo.rare == pivot.ItemEquipmentInfo.rare) && 
                       asc * itemSlotList[low].ItemEquipmentInfo.level < asc * pivot.ItemEquipmentInfo.level) ||
                   (
                       optionSort &&
                       (!rareSort || itemSlotList[low].ItemEquipmentInfo.rare == pivot.ItemEquipmentInfo.rare) && 
                       (!levelSort || itemSlotList[low].ItemEquipmentInfo.level == pivot.ItemEquipmentInfo.level) &&
                       asc * itemSlotList[low].ItemEquipmentInfo.option < asc * pivot.ItemEquipmentInfo.option)
                   )
            {
                low++;
            }
        
            while ((rareSort && 
                    asc * itemSlotList[large].ItemEquipmentInfo.rare > asc * pivot.ItemEquipmentInfo.rare) ||
                   (
                       levelSort &&
                       (!rareSort || itemSlotList[large].ItemEquipmentInfo.rare == pivot.ItemEquipmentInfo.rare) && 
                       asc * itemSlotList[large].ItemEquipmentInfo.level > asc * pivot.ItemEquipmentInfo.level) ||
                   (
                       optionSort &&
                       (!rareSort || itemSlotList[large].ItemEquipmentInfo.rare == pivot.ItemEquipmentInfo.rare) && 
                       (!levelSort || itemSlotList[large].ItemEquipmentInfo.level == pivot.ItemEquipmentInfo.level) &&
                       asc * itemSlotList[large].ItemEquipmentInfo.option > asc * pivot.ItemEquipmentInfo.option)
                  )
            {
                large--;
            }

            if (low > large)
                continue;
            ChangeObjectIndex(low, large);
            low++;
            large--;
        }
        if (minIndex < large)
            QuickSort(minIndex, large, rareSort, levelSort, optionSort);
            
        if (low < maxIndex)
            QuickSort(low, maxIndex, rareSort, levelSort, optionSort);
    }
}
