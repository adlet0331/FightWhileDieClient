using System;

namespace Data
{
    [Serializable]
    public enum ItemType
    {
        // 장비 가능한 아이템
        Equipment = 1,
        // 강화석
        ItemEnhancePiece = 2,
    }

    #region Equipment

    [Serializable]
    public enum EquipPart
    {
        Weapon = 1,
        Cloak = 2,
        Pendant = 3
    }
    public enum EquipmentOption
    {
        // 1. 오토
        AutoUnlockAndDecreaseDebuff = 1,
        // 2. 성장
        AtkAddValue = 2,
        AtkAddPercent = 3,
        BaseAtkGainAddPercent = 4,
        CoinGainAddValue = 5,
        CoinGainAddPercent = 6,
        // 3. 유틸
        DelayDecreasePercent = 7,
        RunningSpeedAddPercent = 8
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
