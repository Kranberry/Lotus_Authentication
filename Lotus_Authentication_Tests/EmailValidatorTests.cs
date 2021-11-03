using NUnit.Framework;
using Lotus_Authentication.Data;

namespace Lotus_Authentication_Tests;

[TestFixture]
internal class EmailValidatorTests
{
    [SetUp]
    public void Setup()
    {

    }
    
    [TestCase("firstname-lastname@example.com")]
    [TestCase("email@example.co.jp")]
    [TestCase("email@example.museum")]
    [TestCase("email@example.name")]
    [TestCase("_______@example.com")]
    [TestCase("email@example-one.com")]
    [TestCase("1234567890@example.com")]
    [TestCase("\"email\"@example.com")]
    [TestCase("email@123.123.123.123")]
    [TestCase("firstname+lastname@example.com")]
    [TestCase("email@subdomain.example.com")]
    [TestCase("firstname.lastname@example.com")]
    [TestCase("email@example.com")]
    public void ValidateTrue(string email)
    {
        Assert.IsTrue(EmailValidator.IsValidEmail(email));
    }

    [TestCase("plainaddress")]
    [TestCase("#@%^%#$@#$@#.com")]
    [TestCase("@example.com")]
    [TestCase("Joe Smith <email@example.com>")]
    [TestCase("email.example.com")]
    [TestCase("email@example@example.com")]
    [TestCase(".email@example.com")]
    [TestCase("email.@example.com")]
    [TestCase("email..email@example.com")]
    [TestCase("あいうえお@example.com")]
    [TestCase("email@example.com (Joe Smith)")]
    [TestCase("email@example")]
    [TestCase("email@-example.com")]
    [TestCase("email@example.web")]
    [TestCase("email@111.222.333.44444")]
    [TestCase("email@example..com")]
    [TestCase("Abc..123@example.com")]
    public void ValidateFalse(string email)
    {
        Assert.IsFalse(EmailValidator.IsValidEmail(email));
    }
}