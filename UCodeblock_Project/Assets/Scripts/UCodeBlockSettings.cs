using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCodeblock
{
    public class UCodeblockSettings : ScriptableObject
    {
        public static UCodeblockSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<UCodeblockSettings>("UCodeblockSettings");
                return _instance;
            }
        }
        private static UCodeblockSettings _instance;

        /// <summary>
        /// Determines the minimum size for executeable and entry blocks.
        /// </summary>
        public Vector2 MinBlockSize;
    }
}