using NUnit.Framework;

namespace Lotus_Authentication_Tests
{
	[TestFixture]
	public class Tests
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void SHA1Test()
		{
			string toBehashed = "testss";
			string expectedHash = "C6B77501AF2051430FDCE1659E8A9582CCBA40CA";

			string recievedHash = Lotus_Authentication.Data.SHA1Hash.Hash(toBehashed);
			Assert.AreEqual(expectedHash, recievedHash);
		}

		[TestCase("C6B77501AF2051430FDCE1659E8A9582CCBA40CA")]
		[TestCase("127646882493FAB0325644A39FA5247F27B73C4C")]
		[TestCase("5325A7AEB59092DC4060EBE36EE83148811CFEDD")]
		[TestCase("A94A8FE5CCB19BA61C4C0873D391E987982FBBD3")]
		[TestCase("A0F1490A20D0211C997B44BC357E1972DEAB8AE3")]
		[TestCase("96A05046147D74CAF69831A8FECB6BC7AB1F95AD")]
		[TestCase("13F566A247AAF7DFD9CF0C8FA1AC5B140045F10A")]
		[TestCase("8A49DEDBB72C569F42C81FA0820FB31F34E3143C")]
		[TestCase("96637AC78B5176FECD97EDCF161BC5F63DAAFAB7")]
		public void SHA1ValidateTrueTest(string hash)
		{
			Assert.IsTrue(Lotus_Authentication.Data.SHA1Hash.IsValidSHA1(hash));
		}

		[TestCase("C6B77501AF2051430FDCE1659E8A9582CCBA40CFAS")]
		[TestCase("127646882493FAB0325644A39FA5247F272B73C4C")]
		[TestCase("5325A7AEB59092DC4060EBE36EE8314881CFEDD")]
		[TestCase("A94A8FE5CCB19BA61C4C0873D391E987982FBB2ED3")]
		[TestCase("A0F1490A20D0211C997B44BC357E1972DEAAE3")]
		[TestCase("96A05046147D74CAF69831A8FECB6BC7AB1#95AD")]
		[TestCase("13F566_247AAF7DFD9CF0C8FA1AC5B140045F10A")]
		[TestCase("8A49DEDBB72C569�42C81FA0820FB31F34E3143C")]
		[TestCase("96637AC78B5176FECD97EDCF161BC5F63DAAF*B7")]
		public void SHA1ValidateFalseTest(string hashThatIsBad)
		{
			Assert.IsFalse(Lotus_Authentication.Data.SHA1Hash.IsValidSHA1(hashThatIsBad));
		}
	}
}