using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UCodeblock.UI
{
    public class InputPropertyField : MonoBehaviour
    {
        private CodeblockItem _source;
        private PropertyInfo _property;
        private TMP_InputField _input;

        public static InputPropertyField Generate(CodeblockItem source, PropertyInfo p)
        {
            GameObject holder = new GameObject($"Input Property Field ({ p.PropertyType.ToString() })");
            InputPropertyField field = holder.AddComponent<InputPropertyField>();

            RectTransform rt = holder.AddComponent<RectTransform>();

            var inputField = Instantiate<GameObject>(Resources.Load<GameObject>("Codeblock_InputField"), holder.transform);
            var input = inputField.GetComponentInChildren<TMP_InputField>();

            field._source = source;
            field._property = p;
            field._input = input;

            input.contentType = GetContentTypeForType(p.PropertyType);
            input.onValueChanged.AddListener(field.UpdateProperty);

            return field;
        }

        private void UpdateProperty (string value)
        {
            Type property = _property.PropertyType;
            if (property == typeof(string))
            {
                _property.SetValue(_source, value);
            }
            if (property == typeof(int))
            {
                _property.SetValue(_source, int.Parse(value));
            }
        }

        private static TMP_InputField.ContentType GetContentTypeForType (Type t)
        {
            if (t == typeof(string))
                return TMP_InputField.ContentType.Standard;
            if (t == typeof(int))
                return TMP_InputField.ContentType.IntegerNumber;
            if (t == typeof(float))
                return TMP_InputField.ContentType.DecimalNumber;

            return TMP_InputField.ContentType.Standard;
        }
    }
}