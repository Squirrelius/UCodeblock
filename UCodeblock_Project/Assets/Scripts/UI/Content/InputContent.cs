using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

namespace UCodeblock.UI
{
    public class InputContent : CodeblockContent, IBlockContent
    {
        public PropertyInfo Property { get; set; }
        public object ReferenceObject { get; set; }

        public Type PropertyType => Property.PropertyType;

        private TMP_InputField _input;
        private TMP_Dropdown _dropdown;
        private GameObject _dropArea;

        private Func<object> _evaluateInputMethod;

        protected override void InitializeContent()
        {
            PrepareInputOfType(PropertyType);
        }

        private void PrepareInputOfType(Type type)
        {
            _input = GetComponentInChildren<TMP_InputField>();
            if (IsInputFieldType(type))
            {
                _input.onValueChanged.AddListener(OnTextChanged);

                if (type == typeof(string))
                {
                    _input.contentType = TMP_InputField.ContentType.Standard;
                    _input.text = "";

                    _evaluateInputMethod = () => _input.text;
                }
                if (type == typeof(int))
                {
                    _input.contentType = TMP_InputField.ContentType.IntegerNumber;
                    _input.text = "0";

                    _evaluateInputMethod = () => int.Parse(_input.text);
                }
                if (type == typeof(float))
                {
                    _input.contentType = TMP_InputField.ContentType.DecimalNumber;
                    _input.text = "0";

                    _evaluateInputMethod = () => float.Parse(_input.text);
                }
            }
            else
            {
                _input.gameObject.SetActive(false);
            }

            _dropdown = GetComponentInChildren<TMP_Dropdown>();
            if (IsDropdownType(type))
            {
                _dropdown.onValueChanged.AddListener(OnDropdownChanged);

                if (type == typeof(bool))
                {
                    _dropdown.options = GenerateOptionsFromStrings("true", "false");
                    _dropdown.value = 0;

                    _evaluateInputMethod = () => _dropdown.value == 0;
                }
                if (type.IsEnum)
                {
                    var displayValues = GenerateOptionsFromStrings(Enum.GetValues(type).OfType<object>().Select(v => v.ToString()).ToArray());
                    _dropdown.options = displayValues;
                    _dropdown.value = 0;

                    _evaluateInputMethod = () => Enum.GetValues(type).OfType<object>().Select(e => Convert.ChangeType(e, type)).ElementAt(_dropdown.value);
                }

                OnDropdownChanged(_dropdown.value);
            }
            else
            {
                _dropdown.gameObject.SetActive(false);
            }
        }

        private bool IsInputFieldType(Type type)
        {
            return type == typeof(string)
                || type == typeof(int)
                || type == typeof(float);
        }
        private bool IsDropdownType(Type type)
        {
            return type == typeof(bool)
                || type.IsEnum;
        }
        private bool IsCodeblockDropType(Type type)
        {
            return false;
        }

        private void OnTextChanged (string value)
        {
            UpdateProperty();
        }
        private void OnDropdownChanged (int index)
        {
            UpdateProperty();
        }

        private void UpdateProperty ()
        {
            if (_evaluateInputMethod == null) return;

            object evaluated = _evaluateInputMethod();
            Property.SetValue(ReferenceObject, evaluated);
        }

        private List<TMP_Dropdown.OptionData> GenerateOptionsFromStrings (params string[] values)
        {
            return values.Select(v => new TMP_Dropdown.OptionData(v)).ToList();
        }
    }
}