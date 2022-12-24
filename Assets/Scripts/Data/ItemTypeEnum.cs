using System;

namespace Data
{
    [Serializable]
    public enum ItemType
    {
        // 장비 가능한 아이템
        Equipment = 1,
        // 장비 강화에 필요한 아이템 (강화재료)
        Ingredient = 2,
    }

    #region Equipment
    [Serializable]
    public enum EquipmentKind
    {
        AtkAddValue = 1,
        AtkAddPercent = 2,
        BaseAtkGainAddPercent = 3,
        CoinGainAddValue = 4,
        CoinGainAddPercent = 5,
        DelayDecreasePercent = 6,
        RunningSpeedAddPercent = 7
    }

    [Serializable]
    public enum Rare
    {
        Common = 1,
        Rare = 2,
        Epic = 3,
        Unique = 4,
        Legend = 5,
        God = 6,
    }
    #endregion
}
