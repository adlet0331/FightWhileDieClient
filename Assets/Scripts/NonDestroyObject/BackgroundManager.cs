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
}
