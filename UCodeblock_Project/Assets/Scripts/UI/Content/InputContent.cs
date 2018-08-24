using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

namespace UCodeblock.UI
{
    /// <summary>
    /// Resembles a <see cref="IBlockContent"/>, where evaluateable codeblocks or user input can be handled.
    /// </summary>
    public class InputContent : CodeblockContent, IBlockContent
    {
        /// <summary>
        /// The target property.
        /// </summary>
        public PropertyInfo Property { get; set; }
        /// <summary>
        /// The target reference object.
        /// </summary>
        public object ReferenceObject { get; set; }

        /// <summary>
        /// The drop area of the <see cref="InputContent"/>.
        /// </summary>
        public Transform DropArea { get; private set; }

        /// <summary>
        /// Gets the assigned <see cref="UICodeblock"/> of the drop area.
        /// </summary>
        public object AssignedCodeblock
            => GetComponentInChildren<UICodeblock>()?.Source;
        /// <summary>
        /// Gets the current operator for handling input.
        /// </summary>
        public object CurrentInputOperator
        {
            get { return _currentInputOperator; }
            set
            {
                _currentInputOperator = value;
                UpdateEvaluateableProperty();
            }
        }
        private object _currentInputOperator;

        /// <summary>
        /// Returns the type of the property the input content is targeted at.
        /// </summary>
        public Type PropertyType => Property.PropertyType;

        private TMP_InputField _input;
        private TMP_Dropdown _dropdown;
        private Action _updateInputOperator;

        protected override void InitializeContent()
        {
            Type genericParameter;
            if (IsEvaluateableCodeblock(PropertyType, out genericParameter)) // Since the codeblock can be evaluated to a known type, find the known type and prepare the input of that type
            {
                PrepareInputOfType(genericParameter);
            }
            else if (IsDynamicEvaluateableCodeblock(PropertyType)) // Since the type is unknown here, handle it as a string, to allow any input
            {
                PrepareInputOfType(typeof(string));
            }
            else
            {
                throw new Exception($"You can't use the type { PropertyType } as a content property. Please use a property of type IEvaluateableCodeblock<T> or IDynamicEvaluateableCodeblock instead.");
            }
        }

        /// <summary>
        /// Updates the way that the input content should evaluate input.
        /// </summary>
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

        /// <summary>
        /// Returns the rectangle that defines the area a <see cref="CodeblockItem"/> can be dropped into.
        /// </summary>
        public Rect GetDropRect ()
        {
            return GetComponent<RectTransform>().GetWorldRect();
        }   
        /// <summary>
        /// Checks if the specified <see cref="CodeblockItem"/> may be dropped into this input content.
        /// </summary>
        public bool AllowBlockDrop (CodeblockItem item)
        {
            return item.GetType() == PropertyType;
        }

        private static bool IsEvaluateableCodeblock (Type type, out Type genericArgument)
        {
            // Iterates over each interface and checks if its IEvaluateableCodeblock<>
            Type[] interfaces = type.IsInterface ? new Type[] { type } : type.GetInterfaces();
            foreach(Type interfaceType in interfaces)
            {
                bool isEvaluateableCodeblock = interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEvaluateableCodeblock<>);
                genericArgument = isEvaluateableCodeblock ? interfaceType.GetGenericArguments().First() : null;
                if (isEvaluateableCodeblock)
                    return true;
            }
            genericArgument = null;
            return false;
        }
        private static bool IsDynamicEvaluateableCodeblock (Type type)
        {
            // Iterates over each interface and checks if its IDynamicEvaluateableCodeblock
            Type[] interfaces = type.IsInterface ? new Type[] { type } : type.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                bool isDynamicEvaluateableCodeblock = type == typeof(IDynamicEvaluateableCodeblock);
                if (isDynamicEvaluateableCodeblock)
                    return true;
            }
            return false;
        }

        private void PrepareInputOfType(Type type)
        {
            _input = GetComponentInChildren<TMP_InputField>();
            if (IsInputFieldType(type))
            {
                _input.onValueChanged.AddListener(OnTextChanged);
                
                if (type == typeof(string))
                {
                    var input = new DynamicInputOperator<string>();

                    _input.contentType = TMP_InputField.ContentType.Standard;
                    _input.text = "";

                    _updateInputOperator = () => input.Value = _input.text;
                    CurrentInputOperator = input;
                }
                if (type == typeof(int))
                {
                    var input = new DynamicInputOperator<int>();

                    _input.contentType = TMP_InputField.ContentType.IntegerNumber;
                    _input.text = "0";

                    _updateInputOperator = () => input.Value = int.Parse(_input.text);
                    CurrentInputOperator = input;
                }
                if (type == typeof(float))
                {
                    var input = new DynamicInputOperator<float>();

                    _input.contentType = TMP_InputField.ContentType.DecimalNumber;
                    _input.text = "0";

                    _updateInputOperator = () => input.Value = float.Parse(_input.text);
                    CurrentInputOperator = input;
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
                    var input = new DynamicInputOperator<bool>();

                    _dropdown.options = new List<TMP_Dropdown.OptionData>(2) { new TMP_Dropdown.OptionData("true"), new TMP_Dropdown.OptionData("false") };
                    _dropdown.value = 0;

                    _updateInputOperator = () => input.Value = _dropdown.value == 0;
                    CurrentInputOperator = input;
                }
                if (type.IsEnum)
                {
                    Type elementType = type;
                    Type operatorType = typeof(DynamicInputOperator<>).MakeGenericType(elementType);
                    var input = Activator.CreateInstance(operatorType);

                    var displayValues = Enum.GetValues(type).OfType<object>().Select(v => v.ToString()).ToArray().Select(v => new TMP_Dropdown.OptionData(v)).ToList();
                    _dropdown.options = displayValues;
                    _dropdown.value = 0;

                    _updateInputOperator = () =>
                    {
                        // This is a bit complex, because we don't know the type of the enum at runtime.
                        // So we use reflection to apply the value you get to the "Value" property of the input operator.
                        object value = (int)Enum.GetValues(type).OfType<object>().Select(e => Convert.ChangeType(e, type)).ElementAt(_dropdown.value);
                        input.GetType().GetProperty("Value").SetValue(input, value);
                    };
                    CurrentInputOperator = input;
                }

                OnDropdownChanged(_dropdown.value);
            }
            else
            {
                _dropdown.gameObject.SetActive(false);
            }

            DropArea = transform.GetChild(2);
        }

        private static bool IsInputFieldType(Type type)
        {
            return type == typeof(string)
                || type == typeof(int)
                || type == typeof(float);
        }
        private static bool IsDropdownType(Type type)
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
            if (_updateInputOperator == null) return;
            _updateInputOperator();
        }
    }
}