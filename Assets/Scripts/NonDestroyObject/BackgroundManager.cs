using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundManager : NonDestroyObject.Singleton<BackgroundManager>
{
    [Serializable]
    protected struct TransformTpPosition
    {
        public Transform objectTransform;
        public int xposition;
        public float velocity;
    }
    [Header("Forest")]
    [SerializeField] private List<TransformTpPosition> ForestBackgrounds;
    
    private bool isPlaying = false;

    /// <summary>
    /// Start moving backgrounds to the left based on their velocities
    /// </summary>
    public void Play()
    {
        isPlaying = true;
    }

    /// <summary>
    /// Stop moving backgrounds
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
    }

    public void UpdateBackground(float deltaTime)
    {
        if (!isPlaying) return;

        // Move each background transform to the left based on velocity
        foreach (TransformTpPosition background in ForestBackgrounds)
        {
            if (background.objectTransform != null)
            {
                // Move left by velocity * deltaTime
                Vector3 position = background.objectTransform.position;
                position.x -= background.velocity * deltaTime;
                background.objectTransform.position = position;
            }
        }
    }
}
