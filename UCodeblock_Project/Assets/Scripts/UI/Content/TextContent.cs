using TMPro;

using UnityEngine;

namespace UCodeblock.UI
{
    public class TextContent : CodeblockContent, IBlockContent
    {
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