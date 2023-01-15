using UnityEngine;
using UnityEngine.PlayerLoop;

namespace UI
{
    /// <summary>
    /// Used inside of Popup.
    /// Smaller Component than Popup
    /// </summary>
    public abstract class View : MonoBehaviour
    {
        protected abstract void Init();
        protected abstract void Clean();

        public void Activate()
        {
            gameObject.SetActive(true);
            Init();
        }

        public void DeActivate()
        {
            gameObject.SetActive(false);
        }
    }
}