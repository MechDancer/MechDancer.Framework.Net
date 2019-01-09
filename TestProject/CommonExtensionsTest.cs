using System.Linq;
using MechDancer.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject {
	[TestClass]
	public class CommonExtensionsTest {
		[TestMethod]
		public void TestLet() {
			var temp = 1.Let(it => it + 1.0);
			Assert.AreEqual(2, temp);
			Assert.IsInstanceOfType(temp, typeof(double));
		}

		[TestMethod]
		public void TestAlso() {
			var temp1 = 0;
			var temp2 = 1.Also(it => temp1 = it);
			Assert.AreEqual(1,     temp2);
			Assert.AreEqual(temp1, temp2);
		}

		[TestMethod]
		public void TestTakeIf() {
			var temp1 = new[] {10}.TakeIf(it => it.Any());
			var temp2 = new[] {10}.TakeIf(it => it.IsReadOnly);
			Assert.IsInstanceOfType(temp1, typeof(int[]));
			Assert.IsNotNull(temp1);
			Assert.IsNull(temp2);
		}

		[TestMethod]
		public void TestAcceptIf() {
			var temp1 = 10.AcceptIf(it => it > 5);
			var temp2 = 10.AcceptIf(it => it < 5);
			Assert.IsInstanceOfType(temp1, typeof(int?));
			Assert.IsNotNull(temp1);
			Assert.IsNull(temp2);
		}

		[TestMethod]
		public void TestTakeUnless() {
			var temp1 = new[] {10}.TakeUnless(it => it.Any());
			var temp2 = new[] {10}.TakeUnless(it => it.IsReadOnly);
			Assert.IsInstanceOfType(temp2, typeof(int[]));
			Assert.IsNull(temp1);
			Assert.IsNotNull(temp2);
		}

		[TestMethod]
		public void TestAcceptUnless() {
			var temp1 = 10.AcceptUnless(it => it > 5);
			var temp2 = 10.AcceptUnless(it => it < 5);
			Assert.IsInstanceOfType(temp2, typeof(int?));
			Assert.IsNull(temp1);
			Assert.IsNotNull(temp2);
		}

		[TestMethod]
		public void TestFlatten() {
			var temp = new[] {new[] {1, 2, 3}, new[] {4, 5, 6}}.Flatten();
			Assert.IsTrue(new[] {1, 2, 3, 4, 5, 6}.SequenceEqual(temp));
		}

		[TestMethod]
		public void TestNone() {
			Assert.IsTrue(new byte[] { }.None());
			Assert.IsFalse(new[] {1}.None());

			Assert.IsTrue(new[] {1}.None(it => it  > 2));
			Assert.IsFalse(new[] {1}.None(it => it > 0));
		}

		[TestMethod]
		public void TestWhereNot() {
			Assert.IsTrue(new[] {1, 2, 3}.WhereNot(it => it % 2 == 0).SequenceEqual(new[] {1, 3}));
		}

		[TestMethod]
		public void TestWhereNotNull() {
			Assert.IsFalse(new object[] {null}.WhereNotNull().Any());
		}

		[TestMethod]
		public void TestSelectNotNull() {
			Assert.AreEqual(2, new[] {new[] {1}, new[] {1, 2}, new[] {1, 2, 3}}
							  .SelectNotNull(it => it.Length > 1 ? new object() : null)
							  .Count());
		}

		[TestMethod]
		public void TestNotContains() {
			Assert.IsTrue(new[] {1, 2, 3, 4, 5}.NotContains(6));
		}

		[TestMethod]
		public void TestRetain() {
			Assert.IsTrue(new[] {1, 2, 3}.Retain(new[] {2, 3, 4}).SequenceEqual(new[] {2, 3}));
		}

		[TestMethod]
		public void TestThen() {
			var temp = false;

			(10 > 5).Then(() => temp = true);
			Assert.IsTrue(temp);

			(10 < 5).Then(() => temp = false);
			Assert.IsTrue(temp);
		}

		[TestMethod]
		public void TestOtherwise() {
			var temp = false;

			(10 > 5).Otherwise(() => temp = true);
			Assert.IsFalse(temp);

			(10 < 5).Otherwise(() => temp = true);
			Assert.IsTrue(temp);
		}
	}
}