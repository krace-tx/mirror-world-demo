using System;
using Godot;

namespace MirrorWorldDemo.Script.UI.Inventory;




public class ItemInstance
{
    public ItemPrototype Prototype { get; }
    public string CustomData { get; private set; }

    public bool IsValid => Prototype != null && !string.IsNullOrWhiteSpace(Prototype.QualifiedItemId);
    public string QualifiedItemId => Prototype?.QualifiedItemId ?? string.Empty;

    public ItemInstance(ItemPrototype prototype, string customData = "")
    {
        Prototype = prototype;
        CustomData = customData ?? string.Empty;
    }

    public ItemInstance Clone()
    {
        return new ItemInstance(Prototype, CustomData);
    }

    public bool IsSameStackKey(ItemInstance other)
    {
        if (other == null || !IsValid || !other.IsValid)
        {
            return false;
        }

        return QualifiedItemId == other.QualifiedItemId
               && string.Equals(CustomData, other.CustomData, StringComparison.Ordinal);
    }
}