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
                    image.color = DataManager.Instance.staticDataManager.RareColorList[equipItemObject.rare];
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
            
            // Play animation directly based on rare value
            // Assumes animation states are named: "Rare0", "Rare1", "Rare2", etc.
            animator.Play($"Rare{equipItemObject.rare}");
        }
    }
}
