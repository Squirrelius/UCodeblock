using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UCodeblock.UI
{
    public class ContentResolver
    {
        private readonly CodeblockItem _source;
        private readonly Type _sourceType;
        private readonly string _contentString;

        public ContentResolver(CodeblockItem block)
        {
            _source = block;
            _sourceType = block.GetType();
            _contentString = block.Content;
        }

        public GameObject Build ()
        {
            var a = _sourceType.GetProperties();
            var b = a.Where(property => property.GetCustomAttribute<ContentPropertyAttribute>() != null);
            var contentProperties = b.ToDictionary(property => property.GetCustomAttribute<ContentPropertyAttribute>().ContentID, property => property);

            GameObject holder = new GameObject("content");
            RectTransform rt = EnsureComponent<RectTransform>(holder);

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = Vector2.one / 2;

            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            HorizontalLayoutGroup hlg = holder.AddComponent<HorizontalLayoutGroup>();

            hlg.spacing = 10;
            hlg.padding = new RectOffset(10, 0, 0, 0);
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandHeight = hlg.childForceExpandWidth = false;

            string m = "";
            string content = _contentString;
            int l = _contentString.Length;
            for (int i = 0; i < l; i++)
            {
                char c = content[i];
                if (c == '{')
                {
                    i++;

                    string num = "";
                    char n;
                    while ((n = content[i]) != '}')
                    {
                        num += n;
                        i++;
                        if (i > l) throw new FormatException();
                    }
                    int id = int.Parse(num);
                    PropertyInfo p = contentProperties[id];
                    BuildFieldForProperty(p).SetParent(rt);
                }

                m += c;
                if (i + 1 < l && content[i + 1] == '{')
                {
                    BuildFieldForString(m).SetParent(rt);
                }
            }

            return holder;
        }

        private static T EnsureComponent<T>(GameObject m) where T : Component
        {
            T t = m.GetComponent<T>();
            if (t != null) return t;
            else return m.AddComponent<T>();
        }

        private static RectTransform BuildFieldForProperty (PropertyInfo p)
        {
            InputPropertyField input = InputPropertyField.Generate(p);

            return input.GetComponent<RectTransform>();
        }
        private static RectTransform BuildFieldForString (string s)
        {
            GameObject textField = new GameObject($"textField ({ s })");
            RectTransform rt = EnsureComponent<RectTransform>(textField);

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(0, 0.5f);

            TextMeshProUGUI tmp = textField.AddComponent<TextMeshProUGUI>();

            tmp.fontSize = 40;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.SetText(s);

            return rt;
        }
    }
}