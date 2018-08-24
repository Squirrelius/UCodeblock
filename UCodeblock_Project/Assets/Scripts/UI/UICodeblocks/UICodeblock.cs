using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
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
    /// Represents a block that is visualized in the UI.
    /// </summary>
    public class UICodeblock : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// The source codeblock item. Null if this is an entry block.
        /// </summary>
        public CodeblockItem Source { get; set; }
        /// <summary>
        /// The position controller of the item.
        /// </summary>
        public LocalPositionController PositionController { get; set; }

        /// <summary>
        /// The type of the underlying <see cref="CodeblockItem"/>.
        /// </summary>
        public UIBlockType Type => GetBlockType(Source);

        protected RectTransform _transform;
        protected LayoutElement _layout;
        protected RectTransform _content;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _layout = GetComponent<LayoutElement>();
            _content = transform.Find("content").GetComponent<RectTransform>();

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
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }

        protected virtual void OnDrawGizmos()
        {
            if (!_transform) return;

            Rect rect = GetDropRect();

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(rect.position + rect.size / 2, rect.size);
        }

        /// <summary>
        /// Returns the rectangle in which siblings can be dropped into.
        /// </summary>
        public virtual Rect GetDropRect ()
        {
            Rect block = _content.GetWorldRect();

            // Modify the position and size to be thinner and below the block
            block.position -= new Vector2(0, block.size.y * 0.6f);
            block.size = Vector2.Scale(block.size, new Vector2(1, 0.6f));

            return block;
        }

        protected virtual void GenerateContent ()
        {
            // If the block doesnt have a source or any content, don't resolve any content
            if (Source == null || string.IsNullOrEmpty(Source.Content)) return;

            // Resolve the blocks content
            ContentResolver resolver = new ContentResolver(Source);
            resolver.ResolveInto(_content);
        }

        /// <summary>
        /// Dynamically generates a UI codeblock.
        /// </summary>
        public static UICodeblock Generate(CodeblockItem source)
        {
            UIBlockType type = GetBlockType(source);
            string name =
                type != UIBlockType.Entry
                ? $"Dynamic UI Codeblock [{ type.ToString() }] ({ source.Identity.ID })"
                : $"Entry Codeblock";

            string requiredPrefab = type.ToString();
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