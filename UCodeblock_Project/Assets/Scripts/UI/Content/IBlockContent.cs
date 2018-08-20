using UnityEngine;
using UnityEngine.UI;

namespace UCodeblock.UI
{
    public interface IBlockContent
    {
        Vector2 PreferredSize { get; set; }
    }
}