using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace UCodeblock.UI
{
    /// <summary>
    /// Used to resolve the content strings of a <see cref="CodeblockItem"/>.
    /// </summary>
    public class ContentResolver
    {
        private readonly CodeblockItem _item;

        /// <summary>
        /// Generates a new instance of a <see cref="ContentResolver"/>, with the specified <see cref="CodeblockItem"/> as a target.
        /// </summary>
        public ContentResolver(CodeblockItem target)
        {
            _item = target;
        }

        /// <summary>
        /// Resolves the content string into the specified parent.
        /// </summary>
        public void ResolveInto (Transform parent)
        {
            // The string that will be resolved
            string content = _item.Content;
            // The property lookup, to match PropertyInfos with the content IDs
            var lookup = GeneratePropertyLookup(_item.GetType());

            // Go through the entire string, character by character, and parse text into text
            // and content IDs into input contents.
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

        /// <summary>
        /// Resolves a string into a <see cref="TextContent"/>.
        /// </summary>
        private GameObject ResolveString(string value)
        {
            //Debug.Log("Resolving string: " + value);

            GameObject prefab = Resources.Load<GameObject>("Codeblock_Content_Text");
            GameObject textObject = GameObject.Instantiate(prefab);

            TextContent content = textObject.GetComponent<TextContent>();
            content.Display = value;

            return textObject;
        }
        /// <summary>
        /// Resolves a property into a <see cref="InputContent"/>.
        /// </summary>
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

        /// <summary>
        /// Generates a lookup of content IDs and matching PropertyInfos.
        /// </summary>
        private static Dictionary<int, PropertyInfo> GeneratePropertyLookup (Type target)
        {
            return target.GetProperties()
                .Where(property => property.GetCustomAttribute<ContentPropertyAttribute>() != null)
                .ToDictionary(property => property.GetCustomAttribute<ContentPropertyAttribute>().ContentID, property => property);
        }
    }
}