using UnityEngine;
using UnityEngine.UI;

namespace UCodeblock.UI
{
    public abstract class CodeblockContent : MonoBehaviour, IBlockContent
    {
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