namespace MechDancer.Framework.Net.Remote.Resources {
	public enum UdpCmd : byte {
		YellAsk      = 0,  // 存在性请求
		YellAck      = 1,  // 存在性回复
		AddressAsk   = 2,  // 地址请求
		AddressAck   = 3,  // 地址回复
		PackageSlice = 4,  // 包分片
		Common       = 127 // 通用广播
	}

	public enum TcpCmd : byte {
		Common = 127 // 通用广播
	}
}