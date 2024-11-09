using System;
using UnityEditor;
using UnityEngine;

namespace Plugins.Animated.Pulsating
{
    public sealed class PositionPulsatingUIWidth : PositionPulsatingUI
    {
        [SerializeField] private PositionsType type = default;


        [ContextMenu("Init")]
        public void Init()
        {
            switch (type)
            {
                case PositionsType.QuadWidth:
                    var width = -(target as RectTransform).rect.width ;
                    var pos = target.localPosition;
                    width /= 4;
                
                    MinPosition = new Vector3(width,pos.y,pos.z);
                    MinPosition = new Vector3(width,pos.y,pos.z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        public override void Reset()
        {
#if UNITY_EDITOR
            base.Reset();
        
            EditorUtility.SetDirty(this);
#endif
        }
    }
    public enum PositionsType
    {
        QuadWidth = 0,
    }
}