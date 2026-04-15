using Godot;

namespace MirrorWorldDemo.Script.Core;

public static class TileMapLayerUtil
{
	// --- 核心基础方法 ---



	/// <summary>
	/// 设置图层排序和 Z 指数
	/// </summary>
	public static void SetOrdering(this TileMapLayer layer, bool ySort, int zIndex)
	{
		if (layer == null) return;
		layer.YSortEnabled = ySort;
		layer.ZIndex = zIndex;
		layer.SetYSortOrigin(layer.TileSet.TileSize.Y / 2); 
	}

	/// <summary>
	/// 实现 TileSet 的独立化并动态配置物理层
	/// </summary>
	public static void SetPhysicsLayerAndMask(this TileMapLayer layer, CollisionLayerEnum colLayerEnum, CollisionLayerEnum colMask = CollisionLayerEnum.None, int physicsLayerIndex = 0)
	{
		if (layer == null || layer.TileSet == null) return;

		// 1. 资源独立化 (Make Unique)
		// 使用 Duplicate(true) 进行深拷贝，确保物理层数组也是独立的
		layer.TileSet = (TileSet)layer.TileSet.Duplicate(true);
		
		TileSet ts = layer.TileSet;

		// 2. 动态维护物理层数量
		// 如果想要设置的索引超过了当前的数量，就通过循环补齐
		// 比如想设置索引 2，但现在只有 0 个，就会补齐到 3 个物理层
		while (ts.GetPhysicsLayersCount() <= physicsLayerIndex)
		{
			ts.AddPhysicsLayer(); 
			Logger.Info($"[TileMapLayerUtil] 为图层 {layer.Name} 的独立 TileSet 动态添加了物理层索引: {ts.GetPhysicsLayersCount() - 1}");
		}

		// 3. 设置 Layer 和 Mask
		// colLayer: 我是谁 (Layer)
		// colMask: 我撞谁 (Mask)
		ts.SetPhysicsLayerCollisionLayer(physicsLayerIndex, (uint)colLayerEnum);
		ts.SetPhysicsLayerCollisionMask(physicsLayerIndex, (uint)colMask);

		Logger.Info($"[TileMapLayerUtil] 图层 {layer.Name} 物理配置成功: Layer位({colLayerEnum}), Mask位({colMask}) at Index {physicsLayerIndex}");
	}

	/// <summary>
	/// 背景层快捷配置：关闭 YSort，设置低 ZIndex（通常用于地面、地毯）
	/// </summary>
	public static void SetAsBackLayer(this TileMapLayer backFloors)
	{
		if (backFloors == null) return;
		// 背景层通常不需要 YSort，因为它永远在玩家脚下
		backFloors.SetOrdering(ySort: false, zIndex: -10);
		Logger.Info($"[TileMapLayerUtil] {backFloors.Name} 已设为背景层 (Z:-10)");
	}

	/// <summary>
	/// 前景遮挡层快捷配置：开启 YSort，设置高 ZIndex（通常用于屋顶、悬挂物）
	/// </summary>
	public static void SetAsFrontLayer(this TileMapLayer frontTable)
	{
		if (frontTable == null) return;
		frontTable.SetOrdering(ySort: true, zIndex: 0);
	}

	/// <summary>
	/// 建筑/障碍物层快捷配置：独立化 TileSet 并配置物理碰撞
	/// </summary>
	public static void SetAsBuildingLayer(this TileMapLayer layer, CollisionLayerEnum colLayerEnum, CollisionLayerEnum colMask = CollisionLayerEnum.None, int physicsLayerIndex = 0)
	{
		if (layer == null) return;

		// 1. 设置排序：建筑层通常需要 YSort 来实现人物绕前绕后的效果
		layer.SetOrdering(ySort: true, zIndex: 0);
		// 2. 独立化物理配置并设置 Layer/Mask
		layer.SetPhysicsLayerAndMask(colLayerEnum, colMask, physicsLayerIndex);
		
		layer.AutoGenerateSquareCollision(physicsLayerIndex);
	}
	
	public static void AutoGenerateSquareCollision(this TileMapLayer layer, int physicsLayerIndex = 0)
	{
		if (layer?.TileSet == null) return;
	
		TileSet ts = layer.TileSet;
		// 确保有物理层槽位
		if (ts.GetPhysicsLayersCount() <= physicsLayerIndex)
			ts.AddPhysicsLayer();

		// 假设你的图块大小是 16x16
		Vector2 tileSize = ts.TileSize;
		Vector2[] squarePolygon = new Vector2[] {
			new Vector2(-tileSize.X/2, -tileSize.Y/2),
			new Vector2(tileSize.X/2, -tileSize.Y/2),
			new Vector2(tileSize.X/2, tileSize.Y/2),
			new Vector2(-tileSize.X/2, tileSize.Y/2)
		};

		// 遍历 TileSet 中的所有图源 (Source)
		for (int i = 0; i < ts.GetSourceCount(); i++)
		{
			int sourceId = ts.GetSourceId(i);
			TileSetAtlasSource source = ts.GetSource(sourceId) as TileSetAtlasSource;
			if (source == null) continue;

			// 遍历该图源下的所有坐标块
			for (int j = 0; j < source.GetTilesCount(); j++)
			{
				Vector2I tileCoord = source.GetTileId(j);
				TileData tileData = source.GetTileData(tileCoord, 0);
			
				// 为该图块在指定的物理层创建碰撞多边形
				tileData.AddCollisionPolygon(physicsLayerIndex);
				tileData.SetCollisionPolygonPoints(physicsLayerIndex, 0, squarePolygon);
			}
		}
	}
}
