using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace UCodeblock.UI
{
    public class ContentResolver
    {
        private readonly CodeblockItem _item;

        public ContentResolver(CodeblockItem target)
        {
            _item = target;
        }

        public void ResolveInto (Transform parent)
        {
            string content = _item.Content;

            var lookup = GeneratePropertyLookup(_item.GetType());

            char c = '\0';
            int i = 0;
            int length = content.Length;
            string numstack = string.Empty;
            string charstack = string.Empty;
            bool numstackOpen = false;

            while (i < length)
            {
                c = content[i];
                switch (c)
                {
                    case '}':
                        if (!numstackOpen)
                        {
                            throw new FormatException();
                        }

                        int value;
                        if (int.TryParse(numstack, out value))
                        {
                            ResolveProperty(lookup[value]).transform.SetParent(parent);

                            numstackOpen = false;
                            numstack = string.Empty;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;

                    case '{':
                        if (numstackOpen)
                        {
                            throw new FormatException();
                        }

                        ResolveString(charstack).transform.SetParent(parent);

                        numstackOpen = true;
                        charstack = string.Empty;
                        break;

                    default:
                        if (numstackOpen)
                        {
                            numstack += c;
                        }
                        else
                        {
                            charstack += c;
                        }
                        break;
                }
                i++;
            }

            if (numstackOpen)
            {
                throw new FormatException();
            }
            if (charstack != string.Empty)
            {
                ResolveString(charstack).transform.SetParent(parent);
            }
        }

        private GameObject ResolveString(string value)
        {
            //Debug.Log("Resolving string: " + value);

            GameObject prefab = Resources.Load<GameObject>("Codeblock_Content_Text");
            GameObject textObject = GameObject.Instantiate(prefab);

            TextContent content = textObject.GetComponent<TextContent>();
            content.Display = value;

            return textObject;
        }
        private GameObject ResolveProperty(PropertyInfo property)
        {
            //Debug.Log("Resolving property: " + property.PropertyType);

            GameObject prefab = Resources.Load<GameObject>("Codeblock_Content_Input");
            GameObject inputObject = GameObject.Instantiate(prefab);

            InputContent content = inputObject.GetComponent<InputContent>();
            content.Property = property;
            content.ReferenceObject = _item;

            return inputObject;
        }

        public float MeasureContentWidth (Transform content)
        {
            return content.GetComponentsInChildren<CodeblockContent>().Sum(c => c.PreferredSize.x);
        }

        private static Dictionary<int, PropertyInfo> GeneratePropertyLookup (Type target)
        {
            return target.GetProperties()
                .Where(property => property.GetCustomAttribute<ContentPropertyAttribute>() != null)
                .ToDictionary(property => property.GetCustomAttribute<ContentPropertyAttribute>().ContentID, property => property);
        }
    }
}