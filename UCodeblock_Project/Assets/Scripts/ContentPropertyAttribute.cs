using System;
using UnityEngine;

/// <summary>
/// Defines the ContentID of a property. This is needed to dynamically generate the GUI for a codeblock.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class ContentPropertyAttribute : Attribute
{
    /// <summary>
    /// The content ID of this property.
    /// </summary>
    public readonly int ContentID;

    public float PreferredWidth { get; set; }
    public float PreferredHeight { get; set; }
    
    public ContentPropertyAttribute(int contentId)
    {
        ContentID = contentId;
    }
}