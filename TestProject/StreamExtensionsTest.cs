using System.IO;
using System.Linq;
using MechDancer.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject {
	[TestClass]
	public class StreamExtensionsTest {
		[TestMethod]
		public void TestCopyRange() {
			var temp = new[] {0, 1, 2, 3, 4, 5};
			Assert.IsTrue(temp.CopyRange().SequenceEqual(new[] {0, 1, 2, 3, 4, 5}));
			Assert.IsTrue(temp.CopyRange(2).SequenceEqual(new[] {2, 3, 4, 5}));
			Assert.IsTrue(temp.CopyRange(2, 3).SequenceEqual(new[] {2}));
		}

		[TestMethod]
		public void TestWrite() {
			var temp   = new byte[] {1, 2, 3, 4, 5};
			var stream = new MemoryStream();
			Extensions.Write(stream, temp);
			// 为了与 .Net Core 支持的 Write(Span) 区分
			Assert.IsTrue(stream
						 .ToArray()
						 .SequenceEqual(temp));
		}

		[TestMethod]
		public void TestWriteReversed() {
			var temp1  = new byte[] {1, 2, 3, 4, 5};
			var temp2  = new byte[] {5, 4, 3, 2, 1};
			var stream = new MemoryStream();
			stream.WriteReversed(temp1);
			Assert.IsTrue(stream
						 .ToArray()
						 .SequenceEqual(temp2));
		}

		[TestMethod]
		public void TestWaitNBytes() {
			Assert.IsTrue(new MemoryStream(new byte[] {1, 2, 3, 4, 5})
						 .WaitNBytes(3)
						 .ToArray()
						 .SequenceEqual(new byte[] {1, 2, 3}));
			Assert.IsTrue(new MemoryStream(new byte[] {1, 2, 3, 4, 5})
						 .WaitNBytes(20)
						 .ToArray()
						 .SequenceEqual(new byte[] {1, 2, 3, 4, 5}));
		}

		[TestMethod]
		public void TestWaitReversed() {
			Assert.IsTrue(new MemoryStream(new byte[] {1, 2, 3, 4, 5})
						 .WaitReversed(3)
						 .ToArray()
						 .SequenceEqual(new byte[] {3, 2, 1}));
			Assert.IsTrue(new MemoryStream(new byte[] {1, 2, 3, 4, 5})
						 .WaitReversed(20)
						 .ToArray()
						 .SequenceEqual(new byte[] {5, 4, 3, 2, 1}));
		}

		[TestMethod]
		public void TestReadRest() {
			var stream = new MemoryStream(new byte[] {1, 2, 3, 4, 5});
			stream.ReadByte();
			Assert.IsTrue(stream
						 .ReadRest()
						 .ToArray()
						 .SequenceEqual(new byte[] {2, 3, 4, 5}));
		}

		[TestMethod]
		public void TestAvailable() {
			var stream = new MemoryStream(new byte[] {1, 2, 3, 4, 5});
			stream.ReadByte();
			Assert.AreEqual(4, stream.Available());
		}

		[TestMethod]
		public void TestGetBytes() {
			Assert.IsTrue("12345".GetBytes().SequenceEqual(new byte[] {49, 50, 51, 52, 53}));
		}

		[TestMethod]
		public void TestString() {
			Assert.IsTrue(new byte[] {49, 50, 51, 52, 53}.GetString() == "12345");
		}

		[TestMethod]
		public void TestWriteEnd() {
			var stream = new MemoryStream();
			stream.WriteEnd("12345");
			Assert.IsTrue(stream.ToArray().SequenceEqual(new byte[] {49, 50, 51, 52, 53, 0}));
		}

		[TestMethod]
		public void TestReadEnd() {
			var stream = new MemoryStream(new byte[] {49, 50, 51, 52, 53, 0, 1, 2, 3});
			Assert.IsTrue(stream.ReadEnd() == "12345");
		}
	}
}