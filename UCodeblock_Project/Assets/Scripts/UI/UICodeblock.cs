using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Represents a block that is visualized in the UI.
    /// </summary>
    public class UICodeblock : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Indicates the type of a <see cref="UICodeblock"/>.
        /// </summary>
        public enum UIBlockType
        {
            Executable,
            Evaluateable,
            Entry,
            ControlFlow,
            Unknown
        }

        /// <summary>
        /// The source codeblock item. Null if this is an entry block.
        /// </summary>
        public CodeblockItem Source { get; set; }

        public LocalPositionController PositionController { get; set; }

        /// <summary>
        /// The type of the underlying <see cref="CodeblockItem"/>.
        /// </summary>
        public UIBlockType Type => GetBlockType(Source);

        protected RectTransform _transform;
        protected LayoutElement _layout;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _layout = GetComponent<LayoutElement>();

            PositionController = new LocalPositionController(transform, 0.1f);

            GenerateContent();
        }

        private void Update()
        {
            PositionController.UpdatePosition();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                PositionController.LocalPosition += (Vector3)eventData.delta;
                PositionController.TargetLocalPosition = PositionController.LocalPosition;
            }
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Type == UIBlockType.Evaluateable)
                {
                    CodeblockInspectionStructure.Instance.RemoveFromContent(this);
                }
            }
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Type != UIBlockType.Entry)
                {
                    if (Type == UIBlockType.Executable)
                    {
                        UICodeblock[] blocksInDropArea = CodeblockInspectionStructure.Instance.GetBlocksInDropArea(_transform.GetWorldRect());
                        UICodeblock validPreviousSibling = blocksInDropArea.FirstOrDefault(b => b.Type == UIBlockType.Executable || b.Type == UIBlockType.Entry || b.Type == UIBlockType.ControlFlow);

                        if (validPreviousSibling != null)
                        {
                            if (validPreviousSibling != this)
                            {
                                CodeblockInspectionStructure.Instance.InsertItem(this, validPreviousSibling);
                            }
                        }
                        else
                        {
                            CodeblockInspectionStructure.Instance.DetachItem(this);
                        }
                    }
                    if (Type == UIBlockType.Evaluateable)
                    {
                        InputContent[] contentsInDropArea = CodeblockInspectionStructure.Instance.GetContentsInDropArea(_transform.GetWorldRect());
                        InputContent leftmostInput = contentsInDropArea.OrderBy(c => c.transform.position.x).FirstOrDefault();

                        if (leftmostInput != null)
                        {
                            CodeblockInspectionStructure.Instance.InsertIntoInputContent(this, leftmostInput);
                        }
                    }
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (!_transform) return;

            Rect rect = GetDropRect();

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(rect.position + rect.size / 2, rect.size);
        }

        public virtual Rect GetDropRect ()
        {
            Rect block = _transform.GetWorldRect();

            // Modify the position and size to be thinner and below the block
            block.position -= new Vector2(0, block.size.y * 0.6f);
            block.size = Vector2.Scale(block.size, new Vector2(1, 0.6f));

            return block;
        }

        protected void GenerateContent ()
        {
            Transform content = _transform.Find("content");

            if (Type == UIBlockType.Executable)
            {
                // Apply the minimum width and height to the block
                Vector2 minSize = UCodeBlockSettings.Instance.MinBlockSize;

                LayoutElement layout = content.GetComponent<LayoutElement>();
                layout.minWidth = minSize.x;
                layout.minHeight = minSize.y;
            }
            else if (Type == UIBlockType.Entry)
            {
                // Apply the minimum width to the block
                Vector2 minSize = UCodeBlockSettings.Instance.MinBlockSize;
                transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minSize.x);
            }

            // If the block doesnt have a source, don't resolve any content
            if (Source == null) return;

            // Resolve the blocks content
            ContentResolver resolver = new ContentResolver(Source);
            resolver.ResolveInto(content);
        }

        /// <summary>
        /// Dynamically generates a UI codeblock.
        /// </summary>
        public static UICodeblock Generate(CodeblockItem source)
        {
            UIBlockType type = GetBlockType(source);
            string name =
                source != null
                ? $"Dynamic UI Codeblock [{ type.ToString() }] ({ source.Identity.ID })"
                : $"Entry Codeblock";

            string requiredPrefab = System.Enum.GetName(typeof(UIBlockType), (int)type);
            GameObject prefab = Resources.Load<GameObject>($"Codeblock_{requiredPrefab}");

            GameObject blockObject = Instantiate(prefab);
            blockObject.name = name;
            
            UICodeblock codeblock = blockObject.GetComponent<UICodeblock>();
            codeblock.Source = source;

            return codeblock;
        }

        /// <summary>
        /// Dynamically generates an entry block.
        /// </summary>
        public static UICodeblock GenerateEntryBlock ()
        {
            return Generate(null);
        }

        /// <summary>
        /// Returns the type that a codeblock with the specified source item should have.
        /// </summary>
        protected static UIBlockType GetBlockType (CodeblockItem source)
        {
            if (source == null)
                return UIBlockType.Entry;

            if (source is IDynamicEvaluateableCodeblock)
                return UIBlockType.Evaluateable;

            if (source is IExecuteableCodeblock)
            {
                if (source is IControlFlowBlock)
                    return UIBlockType.ControlFlow;
                else
                    return UIBlockType.Executable;
            }

            return UIBlockType.Unknown;
        }
    }
}