using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Mitaywalle.ProceduralUI
{
    /// <summary>
    /// Allow to generate ui based on plain C# class or struct, Support only primitive values (int float)
    /// Interpertate Custom Attributes for design: Range(1,3) create slider 
    /// </summary>
    public class UIGeneratedByFields
    {
        Func<object> getData;
        StackWidget stack;
        object data;

        public UIGeneratedByFields(Func<object> getData, StackWidget stack)
        {
            this.getData = getData;
            this.stack = stack;
            Generate();
        }

        public void Generate()
        {
            data = getData.Invoke();
            Type dataType = data.GetType();

            {
                FieldInfo[] fields = dataType.GetFields();

                foreach (FieldInfo field in fields)
                {
                    Debug.Log($"field '{field.Name}' | isSerialized {field.IsNotSerialized}");

                    if (field.IsNotSerialized) continue;
                    IEnumerable<Attribute> attributes = field.GetCustomAttributes();
                    CreateDecorators(attributes);
                    GenerateField(field, attributes);
                }
            }

            {
                PropertyInfo[] properties = dataType.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    IEnumerable<Attribute> attributes = property.GetCustomAttributes();

                    bool isSerialized = false;
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is SerializeField)
                        {
                            isSerialized = true;
                            break;
                        }
                    }

                    Debug.Log($"property '{property.Name}' | isSerialized {isSerialized}");

                    if (!isSerialized) continue;
                    CreateDecorators(attributes);
                    GenerateProperty(property);
                }
            }
        }

        private void GenerateProperty(PropertyInfo field)
        {
            Type type = field.PropertyType;
            object value = field.GetValue(data);

            switch (value)
            {
                case int intValue:
                {
                    break;
                }
            }
        }

        private void GenerateField(FieldInfo field, IEnumerable<Attribute> attributes)
        {
            Type type = field.FieldType;
            object concreteValue = field.GetValue(data);

            var typeSwitch = new TypeSwitch();
            typeSwitch.Case((byte value) => { CreateSlider(field, value, attributes); });
            typeSwitch.Case((Int16 value) => { CreateSlider(field, value, attributes); });
            typeSwitch.Case((int value) => { CreateSlider(field, value, attributes); });
            typeSwitch.Case((long value) => { CreateSlider(field, value, attributes); });
            typeSwitch.Case((double value) => { CreateSlider(field, value, attributes); });
            typeSwitch.Case((float value) => { CreateSlider(field, value, attributes); });
            typeSwitch.Switch(concreteValue);
        }

        private void CreateDecorators(IEnumerable<Attribute> attributes)
        {
            int i = 0;
            foreach (Attribute attribute in attributes)
            {
                #if ODIN_INSPECTOR
                {
                    if (attribute is TitleAttribute title)
                    {
                        stack.Widgets.CreateHeader(title.Title);
                        if (title.HorizontalLine)
                        {
                            stack.Widgets.CreateSeparator();
                        }
                    }
                }
                #endif

                {
                    if (attribute is HeaderAttribute title)
                    {
                        stack.Widgets.CreateHeader(title.header);
                    }
                }

                {
                    if (attribute is SpaceAttribute space)
                    {
                        Widget separator = stack.Widgets.CreateSeparator();
                        Vector2 size = separator.rectTransform.sizeDelta;
                        size.y = space.height;
                        separator.rectTransform.sizeDelta = size;
                        continue;
                    }
                }

                i++;
            }
        }

        private void CreateSlider(FieldInfo field, double value, IEnumerable<Attribute> attributes)
        {
            int i = 0;
            foreach (Attribute attribute in attributes)
            {
                #if ODIN_INSPECTOR
                {
                    if (attribute is PropertyRangeAttribute range)
                    {
                        CreateSlider(field, value, range.Min, range.Max);
                        continue;
                    }
                }

                #endif

                {
                    if (attribute is RangeAttribute range)
                    {
                        CreateSlider(field, value, range.min, range.max);
                    }
                }

                i++;
            }
        }

        private void CreateSlider(FieldInfo field, double value, double max, double min)
        {
        }
    }

    public class TypeSwitch
    {
        Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();

        public TypeSwitch Case<T>(Action<T> action)
        {
            matches.Add(typeof(T), (x) => action((T) x));
            return this;
        }

        public void Switch(object x)
        {
            matches[x.GetType()](x);
        }
    }
}