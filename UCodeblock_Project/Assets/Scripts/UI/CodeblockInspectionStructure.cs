﻿using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Provides methods for displaying a <see cref="CodeblockSystem"/> in the UI.
    /// </summary>
    public class CodeblockInspectionStructure : MonoBehaviour, IInitializePotentialDragHandler
    {
        public static CodeblockInspectionStructure Instance { get { return _instance; } }
        private static CodeblockInspectionStructure _instance;

        /// <summary>
        /// The value that the canvas elements should be scaled to be constant in size across screen resolutions.
        /// </summary>
        public static float CanvasScaleFactor { get; private set; }

        /// <summary>
        /// The system that is currently being inspected.
        /// </summary>
        public CodeblockSystem CurrentSystem { get; set; }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            // for testing only
            CurrentSystem = new CodeblockSystem();
        }
        private void Start()
        {
            CanvasScaleFactor = CalculateCanvasScaleFactor();
        }

        /// <summary>
        /// Calculates the scale the child canvas elements should have.
        /// </summary>
        /// <returns></returns>
        private float CalculateCanvasScaleFactor ()
        {
            Canvas parent = GetComponentInParent<Canvas>();
            Transform t = parent.GetComponent<RectTransform>();
            Vector3 scale = t.localScale;
            float avg = (scale.x + scale.y + scale.z) / 3f;
            return avg;
        }

        private void Update()
        {
            // for testing only
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StandardContext context = new StandardContext(this) { Delay = 0.5f };
                StartCoroutine(CurrentSystem.Blocks.ExecuteCodeblocks(context));
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// Gets the <see cref="UICodeblock"/> of which the drop area overlaps the given rect.
        /// </summary>
        public UICodeblock GetOverlappingDragArea (Rect check)
        {
            return FindObjectsOfType<UICodeblock>().FirstOrDefault(block => block.GetDropRect().Overlaps(check));
        }

        /// <summary>
        /// Inserts an item into the codeblock collection, given a previous sibling as a parent. Pass null to set the item as the entry block.
        /// </summary>
        public void InsertItem (UICodeblock item, UICodeblock previousSibling)
        {
            // Update the codeblock collection
            List<UICodeblock> blocks = new List<UICodeblock>() { item };
            blocks.AddRange(item.GetComponentsInChildren<UICodeblock>());
            CurrentSystem.Blocks.InsertItemRange(blocks.Select(b => b.Source).ToArray(), previousSibling.Source);

            // Update the transform hierarchy
            PlaceUnderCodeblock(item, previousSibling);

            UICodeblock[] children = previousSibling.GetComponentsInChildren<UICodeblock>().Skip(1).ToArray(); // Skip the first element, since that will be the previous sibling codeblock
            // If the previous sibling has any children, insert the item between the previous sibling and its children
            if (children.Length > 0) 
                PlaceUnderCodeblock(children.First(), item);
        }
        /// <summary>
        /// Detaches an item from its parent, inside the collection and in the UI.
        /// </summary>
        /// <param name="item"></param>
        public void DetachItem (UICodeblock item)
        {
            // Update the codeblock collection
            List<UICodeblock> blocks = new List<UICodeblock>() { item };
            blocks.AddRange(item.GetComponentsInChildren<UICodeblock>());
            CurrentSystem.Blocks.DetachItemRange(blocks.Select(b => b.Source).ToArray());

            // Update the transform hierarchy
            item.transform.SetParent(transform);
        }

        /// <summary>
        /// Places a <see cref="UICodeblock"/> under another one.
        /// </summary>
        private void PlaceUnderCodeblock (UICodeblock item, UICodeblock parent)
        {
            item.transform.SetParent(parent.transform);
            item.transform.localPosition = new Vector3(0, -parent.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}