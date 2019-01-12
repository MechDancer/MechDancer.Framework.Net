namespace MechDancer.Framework.Net.Resources {
	public enum UdpCmd : byte {
		YellAsk      = 0,  // 存在性请求
		YellAck      = 1,  // 存在性回复
		AddressAsk   = 2,  // 地址请求
		AddressAck   = 3,  // 地址回复
		PackageSlice = 4,  // 包分片
		TopicMessage = 5,  // 话题消息
		Common       = 127 // 通用广播
	}

	public enum TcpCmd : byte {
		Mail   = 0,  // 发送一次
		Dialog = 1,  // 一问一答
		Common = 127 // 通用广播
	}
}