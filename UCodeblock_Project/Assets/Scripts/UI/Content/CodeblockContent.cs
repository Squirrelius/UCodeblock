using UnityEngine;
using UnityEngine.UI;

namespace UCodeblock.UI
{
    /// <summary>
    /// Base class for content inside a codeblock.
    /// </summary>
    public abstract class CodeblockContent : MonoBehaviour, IBlockContent
    {
        /// <summary>
        /// Gets or sets the preferred size for a codeblock content.
        /// </summary>
        public Vector2 PreferredSize
        {
            get { return new Vector2(_layout.preferredWidth, _layout.preferredHeight); }
            set { _layout.preferredWidth = value.x; _layout.preferredHeight = value.y; }
        }

        private LayoutElement _layout;

        private void Start()
        {
            _layout = GetComponent<LayoutElement>();
            InitializeContent();
        }

        protected abstract void InitializeContent();
    }
}