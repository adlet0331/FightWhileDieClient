using NonDestroyObject;
using UnityEngine;

namespace Utils
{
    public static class ResourcesLoad
    {
        public static Sprite LoadEquipmentSprite(int rare, int option)
        {
            var spriteName = DataManager.Instance.staticDataManager.GetEquipItemInfo(rare, option).spriteName;
            Debug.Log(spriteName);
            spriteName = "01";
            return Resources.Load<Sprite>("Sprites\\ItemEquipment\\" + spriteName);
        }
    }
}