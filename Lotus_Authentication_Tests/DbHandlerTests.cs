using NUnit.Framework;
using Lotus_Authentication.Data;
using System.Threading.Tasks;
using System;

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
}