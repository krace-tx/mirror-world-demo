using Godot;

namespace MirrorWorldDemo.Script.UI.Inventory;

// 物品原型类 - 定义一类物品的静态属性
[GlobalClass]
public partial class ItemPrototype : Resource
{
    /// <summary>
    /// 物品的全局唯一标识符
    /// 推荐格式："类别_名称"，例如 "tool_pickaxe"、"seed_wheat"、"ore_iron"
    /// 用于物品的识别、查找、存档和数据同步
    /// </summary>
    [Export]
    public string QualifiedItemId { get; set; } = string.Empty;

    /// <summary>
    /// 物品的显示名称
    /// 在背包、商店、提示框等UI中展示
    /// </summary>
    [Export]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 物品的详细描述文本
    /// 支持多行文本编辑，用于在物品提示框中显示详细信息
    /// </summary>
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 物品的图标纹理
    /// 在背包格子、快捷栏、商店界面等位置显示
    /// </summary>
    [Export]
    public Texture2D Icon { get; set; }

    /// <summary>
    /// 物品的最大堆叠数量
    /// 范围为 1-999，默认为 1（不可堆叠）
    /// 例如：矿石可堆叠999个，工具只能堆叠1个
    /// </summary>
    [Export(PropertyHint.Range, "1,999,1")]
    public int MaxStackSize { get; set; } = 1;

    /// <summary>
    /// 物品的类型分类
    /// 用于业务逻辑判断（如装备限制、种植条件、使用效果等）
    /// </summary>
    [Export]
    public ItemType Type { get; set; } = ItemType.Misc;

    /// <summary>
    /// 物品是否可交易
    /// true = 可以在商店中买卖，false = 不可交易（如任务物品、绑定装备）
    /// </summary>
    [Export]
    public bool IsTradable { get; set; } = true;

    /// <summary>
    /// 物品是否可丢弃
    /// true = 可以丢弃到地面或垃圾桶，false = 不可丢弃（如重要任务物品）
    /// </summary>
    [Export]
    public bool IsDiscardable { get; set; } = true;
}