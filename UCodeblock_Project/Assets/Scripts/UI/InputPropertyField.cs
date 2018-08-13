using System.Reflection;
using UnityEngine;

namespace UCodeblock.UI
{
    public class InputPropertyField : MonoBehaviour
    {
        public static InputPropertyField Generate(PropertyInfo p)
        {
            GameObject holder = new GameObject($"Input Property Field ({ p.PropertyType.ToString() })");
            InputPropertyField field = holder.AddComponent<InputPropertyField>();

            RectTransform rt = holder.AddComponent<RectTransform>();
            rt.sizeDelta = Vector2.one * 200;

            return field;
        }
    }
}