namespace MirrorWorldDemo.Script.UI.Inventory;

// 物品类型枚举
public enum ItemType
{
    // === 基础资源类 ===
    /// <summary>材料 - 用于合成的原材料（矿石、木材、纤维等）</summary>
    Material,
    
    /// <summary>零件 - 机械或电子组件（齿轮、弹簧、电路板等）</summary>
    Component,
    
    /// <summary>消耗品 - 一次性使用的物品（电池、燃料、急救包等）</summary>
    Consumable,
    
    // === 工具与设备类 ===
    /// <summary>工具 - 可重复使用的基础工具（锤子、螺丝刀、手电筒等）</summary>
    Tool,
    
    /// <summary>器械 - 需要放置或部署的装置（探测器、信号器、照相机等）</summary>
    Device,
    
    /// <summary>钥匙 - 用于解锁门、箱子或机关的特定物品</summary>
    Key,
    
    // === 创造与蓝图类 ===
    /// <summary>蓝图 - 解锁新配方或建造方案的设计图</summary>
    Blueprint,
    
    /// <summary>成品 - 合成产出的最终物品（可能用于推进剧情或破解谜题）</summary>
    Product,
    
    // === 探索与叙事类 ===
    /// <summary>日志 - 可阅读的文本记录（日记、笔记、信件、录音带等）</summary>
    Log,
    
    /// <summary>线索 - 推进剧情或解谜的关键信息物品（照片、证据、符号拓片等）</summary>
    Clue,
    
    /// <summary>遗物 - 具有特殊意义或背景故事的稀有物品（一般不消耗）</summary>
    Relic,
    
    // === 其他 ===
    /// <summary>杂项 - 暂时无法归类的物品</summary>
    Misc
}
