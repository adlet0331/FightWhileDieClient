using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public enum PlayerStatus
    {
        Idle = 0,
        Attack = 1,
    }
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] private Animator animator;

        public void SetAnimation(PlayerStatus status)
        {
            if (status == PlayerStatus.Idle)
            {
                animator.SetBool("Attack", false);
            }

            if (status == PlayerStatus.Attack)
            {
                animator.SetBool("Attack", true);
            }
        }
    }
}