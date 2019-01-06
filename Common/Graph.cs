using System;
using System.Collections.Generic;
using System.Linq;

namespace MechDancer.Common {
	/// <summary>
	/// 	稀疏有向图/出路邻接表
	/// </summary>
	/// <typeparam name="TNode">节点类型</typeparam>
	/// <typeparam name="TPath">包含目的节点的路径类型</typeparam>
	/// <typeparam name="TCore">表结构</typeparam>
	public class Graph<TNode, TPath, TCore>
		where TCore : IReadOnlyDictionary<TNode, IEnumerable<TPath>> {
		private readonly Func<TPath, TNode> _selector;
		public readonly  TCore              Core;

		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="core">存储结构</param>
		/// <param name="selector">从路径找到目标节点</param>
		public Graph(TCore core, Func<TPath, TNode> selector) {
			Core      = core;
			_selector = selector;
		}

		/// <summary>
		/// 	构造包含特定节点的连通子图
		/// </summary>
		/// <param name="root">根节点</param>
		/// <returns>子图</returns>
		public Graph<TNode, TPath, Dictionary<TNode, IEnumerable<TPath>>> SubWith(TNode root)
			=> Sort(root).ToDictionary(it => it, it => Core.GetOrEmpty(it))
						 .Let(it => new Graph<TNode, TPath, Dictionary<TNode, IEnumerable<TPath>>>(it, _selector));

		/// <summary>
		/// 	从特定节点出发进行（反）拓扑排序
		/// </summary>
		/// <param name="root">根节点</param>
		/// <returns>顺序连通的节点列表</returns>
		public IEnumerable<TNode> Sort(TNode root) {
			var sub  = new List<TNode> {root};
			var rest = Core.Keys.ToList().Also(it => it.Remove(root));
			var ptr  = 0;
			while (rest.Any() && sub.Count > ptr)
				if (!Core.TryGetValue(sub[ptr++], out var paths))
					break;
				else {
					var retain = paths.Select(_selector)
									  .Retain(rest)
									  .ToList();
					rest.RemoveAll(retain.Contains);
					sub.AddRange(retain);
				}

			return sub;
		}
	}

	public static partial class Extensions {
		/// <summary>
		/// 	从列表字典获取列表值或空列表
		/// </summary>
		/// <param name="receiver">目标</param>
		/// <param name="key">键</param>
		/// <typeparam name="TKey">键类型</typeparam>
		/// <typeparam name="TElement">列表元素类型</typeparam>
		/// <returns>列表值或空列表</returns>
		public static IEnumerable<TElement> GetOrEmpty<TKey, TElement>(
			this IReadOnlyDictionary<TKey, IEnumerable<TElement>> receiver,
			TKey                                                  key
		) => receiver.TryGetValue(key, out var list) ? list : new List<TElement>();
	}
}