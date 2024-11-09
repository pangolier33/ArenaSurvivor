using Bones.Gameplay.Entities;
using UnityEditor;
using UnityEngine;

namespace Railcar.Installers
{
    [CustomEditor(typeof(EnemyInstaller))]
    public class FlocksDebugger : Editor
    {
        private SerializedProperty _clusterSizeProperty;
        private SerializedProperty _containerSizeProperty;
    
        private void OnSceneGUI()
        {
            return;
            _clusterSizeProperty ??= serializedObject.FindProperty("_clusterSize");
            _containerSizeProperty ??= serializedObject.FindProperty("_extents");

            var clusterSize = _clusterSizeProperty.floatValue;
            var containerSize = _containerSizeProperty.vector2IntValue;

            if (Mathf.Approximately(clusterSize, 0))
                return;
        
            var startX = (containerSize.x + 0.5f) * -clusterSize;
            var endX = -startX;
        
            var startY = (containerSize.y + 0.5f) * -clusterSize;
            var endY = -startY;

            var defaultColor = Handles.color;
            Handles.color = Color.magenta;
            for (var x = startX; x <= endX; x += clusterSize)
            {
                Handles.DrawLine(new Vector3(x, startY), new Vector2(x, endY), 1);
            }

            for (var y = startY; y <= endY; y += clusterSize)
            {
                Handles.DrawLine(new Vector3(startX, y), new Vector2(endX, y), 1);
            }

            Handles.color = defaultColor;
        }
    }
}
