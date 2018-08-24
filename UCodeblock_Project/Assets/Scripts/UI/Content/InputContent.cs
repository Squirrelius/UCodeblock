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

        public Transform DropArea => _dropArea;

        public object AssignedCodeblock
            => GetComponentInChildren<UICodeblock>()?.Source;
        public object CurrentInputOperator
        {
            get { return _currentInputOperator; }
            set
            {
                _currentInputOperator = value;
                UpdateEvaluateableProperty();
            }
        }

        private object _assignedCodeblock;
        private object _currentInputOperator;

        public Type PropertyType => Property.PropertyType;

        private TMP_InputField _input;
        private TMP_Dropdown _dropdown;
        private Transform _dropArea;

        private Action _updateInputOperator;
        private Func<object> _evaluateInputMethod;

        protected override void InitializeContent()
        {
            bool isEvaluateableCodeblock = PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition() == typeof(IEvaluateableCodeblock<>);
            bool isDynamicEvaluateableCodeblock = PropertyType == typeof(IDynamicEvaluateableCodeblock);

            if (isEvaluateableCodeblock) // Since the codeblock can be evaluated to a known type, find the known type and prepare the input of that type
            {
                var genericParameter = PropertyType.GetGenericArguments().First();
                PrepareInputOfType(genericParameter);
            }
            else if (isDynamicEvaluateableCodeblock) // Since the type is unknown here, handle it as a string, to allow any input
            {
                PrepareInputOfType(typeof(string));
            }
            else
            {
                throw new Exception($"You can't use the type { PropertyType } as a content property. Please use a property of type IEvaluateableCodeblock<> or IDynamicEvaluateableCodeblock instead.");
            }
        }

        public void UpdateEvaluateableProperty ()
        {
            if (AssignedCodeblock != null)
            {
                Property.SetValue(ReferenceObject, AssignedCodeblock);
            }
            else
            {
                if (CurrentInputOperator != null)
                {
                    Property.SetValue(ReferenceObject, CurrentInputOperator);
                }
                else
                {
                    throw new Exception($"Could not update property: No AssignedCodeblock and CurrentInputOperator were both null. (Conflicting Type: { PropertyType })");
                }
            }
        }

        public Rect GetDropRect ()
        {
            return GetComponent<RectTransform>().GetWorldRect();
        }   

        private void PrepareInputOfType(Type type)
        {
            _input = GetComponentInChildren<TMP_InputField>();
            if (IsInputFieldType(type))
            {
                _input.onValueChanged.AddListener(OnTextChanged);

                if (type == typeof(string))
                {
                    DynamicInputOperator<string> input = new DynamicInputOperator<string>();

                    _input.contentType = TMP_InputField.ContentType.Standard;
                    _input.text = "";

                    _evaluateInputMethod = () => input.Value = _input.text;
                    CurrentInputOperator = input;
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

            _dropArea = transform.GetChild(2);
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
        }

        private List<TMP_Dropdown.OptionData> GenerateOptionsFromStrings (params string[] values)
        {
            return values.Select(v => new TMP_Dropdown.OptionData(v)).ToList();
        }
    }
}