using System.Linq;
using System.Reflection;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Entities.Stats;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Railcar.Installers
{
    
    [CustomEditor(typeof(Enemy), editorForChildClasses: true)]
    public class EnemyStatsDebugger : Editor
    {
        private SerializedProperty _statsProcessorProperty;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (target is not Enemy enemy)
            {
                EditorGUILayout.LabelField("Cannot treat target as Enemy");
                return;
            }

            /*if (enemy.StatsProcessor is not EnemyStatsProcessor statsProcessor)
            {
                EditorGUILayout.LabelField($"Editor bound to {nameof(EnemyStatsProcessor)}, but the requested processor has different type");
                return;
            }
        
            var statsField = statsProcessor.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name.Contains("Stats"));
            if (statsField is null)
            {
                EditorGUILayout.LabelField($"Stats processor doesn't have stats");
                return;
            }
            
            var stats = (IStats)statsField.GetValue(statsProcessor);
            EditorGUILayout.LabelField("Health", stats.Health.ToString("F2"));
            EditorGUILayout.LabelField("Armor", stats.Armor.ToString("F2"));
            EditorGUILayout.LabelField("Damage", stats.Damage.ToString("F2"));*/
        }
    }
}