using UnityEngine;
using UnityEngine.UI;

namespace UCodeblock.UI
{
    public static class UICanvasExtensions
    {
        /// <summary>
        /// Returns the rectangle that this <see cref="RectTransform"/> takes up (in world space).
        /// </summary>
        public static Rect GetWorldRect (this RectTransform rect)
        {
            // Calculate the top left corner
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector2 topLeft = corners[0];

            Vector2 size = rect.rect.size;
            Vector2 scaledSize = size * rect.lossyScale;

            return new Rect(topLeft, scaledSize);
        }
    }
}