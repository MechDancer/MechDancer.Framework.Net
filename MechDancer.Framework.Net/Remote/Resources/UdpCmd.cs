namespace MechDancer.Framework.Net.Remote.Resources {
	public enum UdpCmd : byte {
		YellAsk      = 0,
		YellAck      = 1,
		AddressAsk   = 2,
		AddressAck   = 3,
		PackageSlice = 4,
		Common       = 127
	}
}