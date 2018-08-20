using TMPro;

namespace UCodeblock.UI
{
    public class TextContent : CodeblockContent, IBlockContent
    {
        public string Display;
        private TextMeshProUGUI _text;

        protected override void InitializeContent()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _text.SetText(Display);
        }
    }
}