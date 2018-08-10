using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UCodeblock.UI
{
    public class GenericPropertyField<T> : CodeblockPropertyField
    {
        public DynamicInputOperator<T> Operator { get; set; }
        public Type Type => (typeof(T));

        public static void Generate<TIn>(DynamicInputOperator<TIn> source)
        {
            GameObject holder = new GameObject("Codeblock Property Field");
            GenericPropertyField<TIn> field = holder.AddComponent<GenericPropertyField<TIn>>();

            field.Operator = source;
            field.GenerateLayout();
        }

        private void UpdateOperatorValue()
        {
            if (typeof(T) == typeof(int))
            {

            }
            if (typeof(T) == typeof(string))
            {
                Operator.Value = (T)(object)_input.text;
            }
        }
    }
}