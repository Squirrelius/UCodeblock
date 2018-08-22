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
        /// <summary>
        /// Is this an entry block?
        /// </summary>
        public bool IsEntryBlock => Type == UIBlockType.Entry;

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
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!IsEntryBlock)
                {
                    if (Type == UIBlockType.Executable)
                    {
                        UICodeblock dropped = CodeblockInspectionStructure.Instance.GetBlockInDropArea(_transform.GetWorldRect());

                        if (dropped != null)
                        {
                            if (dropped != this)
                            {
                                if (dropped.Type == UIBlockType.Executable || dropped.Type == UIBlockType.Entry || dropped.Type == UIBlockType.ControlFlow)
                                {
                                    CodeblockInspectionStructure.Instance.InsertItem(this, dropped);
                                }
                            }
                        }
                        else
                        {
                            CodeblockInspectionStructure.Instance.DetachItem(this);
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
            if (Source == null) return;

            RectTransform content = _transform.Find("content").GetComponent<RectTransform>();

            ContentResolver resolver = new ContentResolver(Source);
            resolver.ResolveInto(content);
        }

        /// <summary>
        /// Dynamically generates a UI codeblock.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
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