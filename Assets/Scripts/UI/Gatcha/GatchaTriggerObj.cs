using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.Gatcha
{
    public class GatchaTriggerObj : MonoBehaviour, IPointerClickHandler
    {
        [Header("Need Init")]
        [SerializeField] private bool isOpenable;
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
        private readonly static int Clicked = Animator.StringToHash("Clicked");

        private void Start()
        {
            runtimeAnimatorController = animator.runtimeAnimatorController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isOpenable) return;
            
            animator.SetBool(Clicked, true);
            var openAnimationTime = AnimatorUtil.GetAnimationTime("GatchaTrigger", runtimeAnimatorController);
        }
    }
}
