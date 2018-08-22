using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCodeblock
{
    public class UCodeBlockSettings : ScriptableObject
    {

        private static UCodeBlockSettings _myInstance;
        public static UCodeBlockSettings MyInstance
        {
            get
            {
                if (_myInstance == null)
                    _myInstance = Resources.Load<UCodeBlockSettings>("UCodeBlockSettings");
                return _myInstance;
            }
        }

        [Tooltip("Determines the minimum width of each block")]
        public int _minBlockWidth = 100;
    }
}