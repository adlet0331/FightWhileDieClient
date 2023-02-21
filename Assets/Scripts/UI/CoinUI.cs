using TMPro;
using UnityEngine;
using Utils;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinValueText;

    public void SetCoinValue(int val)
    {
        coinValueText.text = IntToUnitString.ToString(val);
    }
}
