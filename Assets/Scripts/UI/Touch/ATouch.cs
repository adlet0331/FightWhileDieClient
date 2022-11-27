using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Touch
{
    public abstract class ATouch: MonoBehaviour, IPointerClickHandler
    {
        protected abstract void OnTouch();
        public void OnPointerClick(PointerEventData eventData)
        {
            OnTouch();
        }
    }
}