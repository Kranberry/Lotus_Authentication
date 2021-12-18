using NUnit.Framework;
using Lotus_Authentication.Data;
using System.Threading.Tasks;
using System;
using Lotus_Authentication.API.Controllers;
using Lotus_Authentication.Models;

namespace Lotus_Authentication_Tests;

[TestFixture]
internal class DbHandlerTests
{
    [SetUp]
    public void Setup()
    {

    }
    
    [Test]
    public async ValueTask SysLogAddTestSuccess()
    {
        int result = await DbHandler.AddNewSystemLog(LogSeverity.Informational
                                                    , null
                                                    , "Automatic unit test"
                                                    , $"TestClass: {nameof(DbHandlerTests)}, TestMethod: {nameof(SysLogAddTestSuccess)}");
        int expected = 1;
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void InsertSucceedMemberTest()
    {
        // TODO: Generate a test case from a username and email address that is sure to not exist. And then delete the user when done.
        User user = new(0, "test", "testsson", "testaren@test.com", "testarn", UserType.Regular, Gender.Male, "SE", null, null, DateTime.Now, null, false);
        Assert.DoesNotThrowAsync(async () => await DbHandler.InsertUser(user, "2527C-ACAd56BA-F08cBbFE-e8cdbFef-EA0c7Ab1"));
    }

    [Test]
    public void InsertFailMemberTest()
    {
        //TODO: Add more test cases
        User user = new(0, "test", "testsson", "testaren@test.com", "testarn", UserType.Regular, Gender.Male, "SE", null, null, DateTime.Now, null, false);
        Assert.ThrowsAsync<BadApiKeyReferenceException>(async () => await DbHandler.InsertUser(user, "2527C-ACAd56BA-F08cBbFE-e8cbFef-EA0c71"));
    }
}