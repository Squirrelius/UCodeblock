using System;

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
    
    public ContentPropertyAttribute(int contentId)
    {
        ContentID = contentId;
    }
}