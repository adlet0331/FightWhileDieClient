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
        [Header("Status")]
        [SerializeField] private bool opened;
        [Header("Need Init")]
        [SerializeField] private bool isOpenable;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image[] rareColorImages; 
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
        [SerializeField] private EquipItemObject equipItemObject;

        public void Initiate(bool openable, EquipItemObject info)
        {
            opened = false;
            
            isOpenable = openable;
            if (info != null)
            {
                equipItemObject = info;
                itemImage.sprite = ResourcesLoad.LoadEquipmentSprite(info.rare, info.option);
                foreach (var image in rareColorImages)
                {
                    image.color = DataManager.Instance.itemManager.RareColorList[equipItemObject.rare];
                }
            }
        }
        
        private void Start()
        {
            runtimeAnimatorController = animator.runtimeAnimatorController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isOpenable || opened) return;
            
            SoundManager.Instance.PlayClip(ClipName.GatchaOpen);
            OpenAndShowItem();
        }

        public void OpenAndShowItem()
        {
            opened = true;
            
            animator.SetInteger("TriggeredRare", equipItemObject.rare);
        }
    }
}
