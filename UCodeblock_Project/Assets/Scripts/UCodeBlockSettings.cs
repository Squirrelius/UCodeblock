using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCodeblock
{
    public class UCodeBlockSettings : ScriptableObject
    {
        private static UCodeBlockSettings _instance;
        public static UCodeBlockSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<UCodeBlockSettings>("UCodeBlockSettings");
                return _instance;
            }
        }

        /// <summary>
        /// Determines the minimum size for executeable and entry blocks.
        /// </summary>
        public Vector2 MinBlockSize;
    }
}