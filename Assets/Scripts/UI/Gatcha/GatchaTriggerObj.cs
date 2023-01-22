using Cysharp.Threading.Tasks;
using Data;
using NonDestroyObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI.Gatcha
{
    public class GatchaTriggerObj : MonoBehaviour, IPointerClickHandler
    {
        [Header("Need Init")]
        [SerializeField] private bool isOpenable;
        [SerializeField] private Image[] images; 
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
        [SerializeField] private EquipItemObject equipItemObject;
        private readonly static int Clicked = Animator.StringToHash("TriggeredRare");

        public void Initiate(bool openable, EquipItemObject info)
        {
            isOpenable = openable;
            equipItemObject = info;
            
            foreach (var image in images)
            {
                image.color = DataManager.Instance.itemManager.RareColorList[equipItemObject.rare];
            }
        }
        
        private void Start()
        {
            runtimeAnimatorController = animator.runtimeAnimatorController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked");
            
            if (!isOpenable) return;

            animator.SetInteger(Clicked, equipItemObject.rare);
        }

        public void OpenAndShowItem()
        {
            
        }
    }
}
