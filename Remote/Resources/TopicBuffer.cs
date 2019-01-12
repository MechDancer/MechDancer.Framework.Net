using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MechDancer.Common;
using MechDancer.Framework.Dependency.UniqueComponent;
using Map = System.Collections.Concurrent.ConcurrentDictionary<string, System.IO.Stream>;

namespace MechDancer.Framework.Net.Resources {
	/// <summary>
	/// 	话题数据缓存
	/// </summary>
	public class TopicBuffer : UniqueComponent<TopicBuffer> {
		private readonly ConcurrentDictionary<string, Dictionary<string, Stream>>
			_core = new ConcurrentDictionary<string, Dictionary<string, Stream>>();

		/// <summary>
		/// 	获取一个远端的话题列表
		/// </summary>
		/// <param name="name">远端名字</param>
		public IEnumerable<string> this[string name]
			=> _core.TryGetValue(name, out var dictionary)
				   ? dictionary.Keys.ToArray()
				   : new string[] { };

		/// <summary>
		/// 	读取特定话题
		/// </summary>
		/// <param name="name">名字</param>
		/// <param name="topic">话题</param>
		public Stream this[string name, string topic]
			=> _core.TryGetValue(name, out var dictionary)
				   ? dictionary.TryGetValue(topic, out var stream)
						 ? stream
						 : null
				   : null;

		/// <summary>
		/// 	保存一个话题
		/// </summary>
		/// <param name="name">名字</param>
		/// <param name="payload">数据包</param>
		public void Save(string name, byte[] payload) {
			var stream = new MemoryStream(payload);
			_core.AddOrUpdate(name,
							  _ => new Dictionary<string, Stream> {{stream.ReadEnd(), stream}},
							  (_, last) => {
								  last[stream.ReadEnd()] = stream;
								  return last;
							  });
		}
	}
}