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
        private readonly Type _codeblockType;

        public ContentResolver(CodeblockItem block)
        {
            _codeblockType = block.GetType();
        }

        public Transform Build ()
        {
            var contentProperties = _codeblockType.GetProperties(BindingFlags.Instance)
                .Where(property => property.GetCustomAttribute<ContentPropertyAttribute>() != null)
                .ToDictionary(property => property.GetCustomAttribute<ContentPropertyAttribute>().ContentID, property => property);



            return null;
        }
    }
}