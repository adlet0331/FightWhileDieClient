using UnityEngine;

namespace Utils
{
    public class AnimatorUtil
    {
        public static float GetAnimationTime(string animationName, RuntimeAnimatorController runtimeAnimatorController)
        {
            foreach (var t in runtimeAnimatorController.animationClips)
            {
                if(t.name == animationName) //If it has the same animationName as your clip
                {
                    return t.length; // Because For Delay
                }
            }

            Debug.LogAssertion("No Animation Named " + animationName);
            return 0.0f;
        }
    }
}