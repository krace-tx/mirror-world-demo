using System.Collections.Generic;
using Godot;

namespace MirrorWorldDemo.Script.Core;

public static class YSortUtil
{
	/// <summary>
	/// 自动打通两个节点之间的 Y-Sort 链路。
	/// </summary>
	public static void EnableYSortPath(Node2D nodeA, Node2D nodeB, int? zIndex = null)
	{
		if (nodeA == null || nodeB == null) return;

		// 1. 手动查找最近公共祖先 (LCA)
		Node lca = FindLowestCommonAncestor(nodeA, nodeB);
		if (lca == null) return;

		// 2. 收集从 nodeA 到 LCA 以及 nodeB 到 LCA 的所有路径节点
		// 使用 HashSet 自动去重，优化多次重叠路径的操作
		var nodesToEnable = new HashSet<CanvasItem>();

		CollectPathUpwards(nodeA, lca, nodesToEnable);
		CollectPathUpwards(nodeB, lca, nodesToEnable);

		// 3. 处理 LCA 本身
		if (lca is CanvasItem lcaCanvas)
		{
			nodesToEnable.Add(lcaCanvas);
		}

		// 4. 批量开启 Y-Sort
		foreach (var item in nodesToEnable)
		{
			if (!item.YSortEnabled)
			{
				item.YSortEnabled = true;
			}

			if (zIndex.HasValue && item.ZIndex != zIndex.Value)
			{
				item.ZIndex = zIndex.Value;
			}
		}
	}

	/// <summary>
	/// 寻找最近公共祖先算法 (O(D) 复杂度)
	/// </summary>
	public static Node FindLowestCommonAncestor(Node a, Node b)
	{
		var ancestorsA = new HashSet<Node>();
		Node currA = a;

		// 记录 A 的所有祖先
		while (currA != null)
		{
			ancestorsA.Add(currA);
			currA = currA.GetParent();
		}

		// 检查 B 的祖先中第一个出现在 A 祖先集合里的
		Node currB = b;
		while (currB != null)
		{
			if (ancestorsA.Contains(currB))
			{
				return currB;
			}
			currB = currB.GetParent();
		}

		return null;
	}

	/// <summary>
	/// 向上收集路径节点
	/// </summary>
	private static void CollectPathUpwards(Node start, Node limit, HashSet<CanvasItem> collection)
	{
		Node current = start;
		while (current != null && current != limit)
		{
			if (current is CanvasItem canvas)
			{
				collection.Add(canvas);
			}
			current = current.GetParent();
		}
	}
}
