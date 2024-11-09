using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Sirenix.OdinInspector.Custom.Editor
{
    [CustomEditor(typeof(CustomBehaviour),true)]
    public class ObjectEditor : OdinEditor
    {
        protected static Dictionary<Type, MethodInfo[]> _methods = new();
        protected static Dictionary<MethodInfo, IEnumerable<Attribute>> _attributes = new();
        Object[] cachedTargets;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (targets == null || cachedTargets == null || targets.Length != cachedTargets.Length)
            {
                cachedTargets = targets.ToArray();
            }
            else
            {
                targets.CopyTo(cachedTargets,0);
            }
        }

        protected virtual void OnSceneGUI()
        {
            if (cachedTargets == null) return;
            if (cachedTargets?.Length == 0) return;

            foreach (Object inspectedObject in cachedTargets)
            {
                if (cachedTargets?.Length == 0) return;
                
                Type type = target.GetType();
                if (!_methods.ContainsKey(type))
                {
                    _methods.Add(type, type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance));
                }

                foreach (MethodInfo method in _methods[type])
                {
                    if (!_attributes.ContainsKey(method))
                    {
                        _attributes.Add(method, method.GetCustomAttributes());
                    }

                    foreach (Attribute attribute in _attributes[method])
                    {
                        if (attribute is OnSceneGUIAttribute)
                        {
                            method.Invoke(inspectedObject, null);
                        }
                    }
                }
            }
        }
    }
}