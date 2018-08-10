using System;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class ContentPropertyAttribute : Attribute
{
    public readonly int ContentID;
    
    public ContentPropertyAttribute(int contentId)
    {
        ContentID = contentId;
    }
}