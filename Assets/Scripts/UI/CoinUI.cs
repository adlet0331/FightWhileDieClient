using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinValueText;

    public void SetCoinValue(int val)
    {
        coinValueText.text = val.ToString();
    }
}
