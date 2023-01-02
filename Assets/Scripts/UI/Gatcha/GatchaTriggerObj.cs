using Cysharp.Threading.Tasks;
using Data;
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
        [SerializeField] private ItemEquipment itemEquipment;
        private readonly static int Clicked = Animator.StringToHash("TriggeredRare");

        public void Initiate(bool openable, ItemEquipment info)
        {
            isOpenable = openable;
            itemEquipment = info;
        }
        
        private void Start()
        {
            runtimeAnimatorController = animator.runtimeAnimatorController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isOpenable) return;
            
            animator.SetInteger(Clicked, (int)itemEquipment.rare);

            CoroutineUtils.WaitAndOperationIEnum(1.0f, () =>
            {
                //animator.SetInteger(Clicked, 0);
            });
        }

        public void OpenAndShowItem()
        {
            
        }
    }
}
