using UnityEngine;

namespace UI
{
    public class Popup : MonoBehaviour
    {
        public void Open()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}