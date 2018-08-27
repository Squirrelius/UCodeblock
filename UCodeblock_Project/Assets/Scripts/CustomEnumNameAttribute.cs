using System;

[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class CustomEnumNameAttribute : Attribute
{
    /// <summary>
    /// The name that should be used to display the enum.
    /// </summary>
    public readonly string CustomName;

    public CustomEnumNameAttribute(string name)
    {
        CustomName = name;
    }
}