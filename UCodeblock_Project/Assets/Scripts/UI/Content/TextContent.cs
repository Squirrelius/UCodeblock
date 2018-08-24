using TMPro;

using UnityEngine;

namespace UCodeblock.UI
{
    /// <summary>
    /// Resembles a <see cref="IBlockContent"/>, where raw text is displayed.
    /// </summary>
    public class TextContent : CodeblockContent, IBlockContent
    {
        /// <summary>
        /// The string that should be displayed in the text.
        /// </summary>
        public string Display { get; set; }

        private TextMeshProUGUI _text;

        protected override void InitializeContent()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _text.SetText(Display);
            _text.ForceMeshUpdate();
            
            PreferredSize = _text.GetPreferredValues(Display);
        }
    }
}