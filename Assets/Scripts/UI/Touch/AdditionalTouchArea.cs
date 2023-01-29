using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Touch
{
    public abstract class AdditionalTouchArea<T> : MonoBehaviour, IPointerClickHandler where T : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private T objectToSendClickEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            objectToSendClickEvent.OnPointerClick(eventData);
        }
    }
}