using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace UCodeblock.UI
{
    /// <summary>
    /// Represents a block that is visualized in the UI.
    /// </summary>
    public class UICodeblock : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// The source codeblock item. Null if this is an entry block.
        /// </summary>
        public CodeblockItem Source { get; set; }
        /// <summary>
        /// Is this an entry block?
        /// </summary>
        public bool IsEntryBlock { get; private set; }

        private RectTransform _transform;
        private TextMeshProUGUI _content;
        private TMP_InputField _input;

        private static readonly Vector2 DefaultBlockSize = new Vector2(600, 60);
        
        public void OnDrag(PointerEventData eventData)
        {
            Drag(eventData.delta);
        }
        public void OnPointerDown(PointerEventData eventData)
        {

        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsEntryBlock)
            {
                if (Source is IExecuteableCodeblock)
                {
                    UICodeblock dropped = CodeblockInspectionStructure.Instance.GetOverlappingDragArea(GetBlockRect());
                    if (dropped != null && dropped != this)
                    {
                        CodeblockInspectionStructure.Instance.InsertItem(this, dropped);
                    }
                    if (dropped == null)
                    {
                        CodeblockInspectionStructure.Instance.DetachItem(this);
                    }
                }
                else
                {

                }
            }
        }

        public void Drag(Vector3 delta)
        {
            _transform.position += delta;
            Redraw();
        }
        public void Redraw()
        {

        }

        private void OnDrawGizmos()
        {
            Rect rect = GetDropRect();

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(rect.position + rect.size / 2, rect.size);
        }

        public Rect GetDropRect ()
        {
            Rect block = GetBlockRect();

            // Modify the position and size to be thinner and below the block
            block.position -= new Vector2(0, block.size.y * 0.6f);
            block.size = Vector2.Scale(block.size, new Vector2(1, 0.6f));

            return block;
        }
        public Rect GetBlockRect ()
        {
            Vector3[] corners = new Vector3[4];
            _transform.GetWorldCorners(corners);
            Vector2 topLeft = corners[0];

            Vector2 size = _transform.rect.size;
            Vector2 scaledSize = size * CodeblockInspectionStructure.CanvasScaleFactor;

            return new Rect(topLeft, scaledSize);
        }

        /// <summary>
        /// Dynamically generates a <see cref="UICodeblock"/>.
        /// </summary>
        /// <param name="source">The source of the codeblock.</param>
        public static UICodeblock Generate(CodeblockItem source)
        {
            GameObject block = new GameObject($"Dynamic UI Codeblock ({ source.Identity.ID }");

            UICodeblock codeblock = block.AddComponent<UICodeblock>();
            codeblock.Source = source;
            codeblock.IsEntryBlock = false;

            codeblock.GenerateVisual();

            return codeblock;
        }
        /// <summary>
        /// Dynamically generates an entry block.
        /// </summary>
        /// <returns></returns>
        public static UICodeblock GenerateEntryBlock ()
        {
            GameObject block = new GameObject($"UI Entry Codeblock");

            UICodeblock codeblock = block.AddComponent<UICodeblock>();
            codeblock.Source = null;
            codeblock.IsEntryBlock = true;

            codeblock.GenerateVisual();

            return codeblock;
        }

        protected void GenerateVisual ()
        {
            if (IsEntryBlock)
            {
                // Entry Block
                SetupBlockTransform(DefaultBlockSize);
                GenerateMainPart(Color.green);

                return;
            }
            if (Source is IExecuteableCodeblock)
            {
                if (Source is IControlFlowBlock)
                {

                }
                else
                {
                    // Regular function codeblock
                    SetupBlockTransform(DefaultBlockSize);
                    GenerateMainPart(Color.red);
                    GenerateBlockContent();
                }
            }
            if (Source is IDynamicEvaluateableCodeblock)
            {
                SetupBlockTransform(DefaultBlockSize);
                GenerateMainPart(Color.red);
            }
        }

        private RectTransform SetupBlockTransform (Vector2 size)
        {
            _transform = gameObject.AddComponent<RectTransform>();
            RectTransform rt = gameObject.GetComponent<RectTransform>();

            rt.anchorMin = Vector2.up;
            rt.anchorMax = Vector2.up;
            rt.pivot = Vector2.up;

            rt.sizeDelta = size;

            return rt;
        }

        private RectTransform GenerateMainPart (Color color)
        {
            GameObject part = new GameObject("body");
            RectTransform rt = GenerateFillTransform(part);

            // Add the visual
            Image img = part.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>("Codeblock_Base");
            img.type = Image.Type.Sliced;
            img.color = color;

            return rt;
        }

        private GameObject GenerateBlockContent ()
        {
            ContentResolver resolver = new ContentResolver(Source);
            GameObject content = resolver.Build();

            content.transform.SetParent(transform, false);

            return content;
            
        }
        private TMP_InputField GenerateInputField ()
        {
            GameObject content = new GameObject("content");
            GameObject inputFieldPrefab = Resources.Load<GameObject>("Codeblock_InputField");

            GameObject inputField = Instantiate(inputFieldPrefab, transform);
            TMP_InputField field = inputField.GetComponent<TMP_InputField>();

            _input = field;

            return field;
        }

        private RectTransform GenerateFillTransform (GameObject g)
        {
            RectTransform rt = g.GetComponent<RectTransform>();
            if (rt == null) rt = g.AddComponent<RectTransform>();

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = Vector2.one / 2;

            rt.SetParent(transform);

            // Set the mode to fill
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            return rt;
        }
    }
}