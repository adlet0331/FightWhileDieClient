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
        protected abstract void BeforeActivate();
        protected abstract void AfterDeActivate();

        public void Activate()
        {
            BeforeActivate();
            gameObject.SetActive(true);
        }

        public void DeActivate()
        {
            gameObject.SetActive(false);
            AfterDeActivate();
        }
    }
}