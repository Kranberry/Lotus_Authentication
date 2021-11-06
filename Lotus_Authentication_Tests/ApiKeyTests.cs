using NUnit.Framework;
using Lotus_Authentication.Data;
using System.IO;
using System.Text;

namespace Lotus_Authentication_Tests;

[TestFixture]
internal class ApiKeyTests
{

    [OneTimeSetUp]
    public void Setup()
    {
        
    }

    [TestCase("7096E-FcD1bD5D-6FBBe34f-ab7eECed-35EEBEcf")]
    [TestCase("6302B-8BdbFCE9-2fCadfEF-3c5eAbA5-6aAbAc99")]
    [TestCase("5810C-f0D8a5BA-6fBBe3ff-e5f5Ef67-233a6Cdc")]
    [TestCase("4260B-aC04Cc9E-fa8ccfc4-BcEFBa51-fABbeCDf")]
    [TestCase("5451a-c92feaA6-8de38AbF-Daced62C-77fC5D83")]
    [TestCase("2238d-A1Ff75b1-D68EDcD8-dbF5F1a2-91eCcbf6")]
    [TestCase("4314b-ABF7C0f9-1f1DE0B2-5CBca8Ed-b5B4CCDe")]
    [TestCase("8047B-5ADaD6FE-c96e6a99-CA4cBdD0-bfaecaF9")]
    [TestCase("4158D-D5D8F9eA-bAA6e5CB-cbD21A33-eCFaceB3")]
    [TestCase("4423D-F2361BDC-11CBF13F-FD8FeCBd-753aEDaA")]
    [TestCase("7989D-1C3c02eE-0B616dd5-4eC38FB7-eBf3cD1f")]
    [TestCase("0854C-61b657fB-7EFED4cE-8baEE3a3-dE5FC5b0")]
    [TestCase("1547e-AEDf6A98-7F6A2Dfb-BEcAcC5E-3EbBCfD6")]
    [TestCase("0762d-e3d1C88f-3b3FAbAB-0aEa6fcD-a6AEC39B")]
    [TestCase("4189F-DB5dB45E-FCFFf35B-A3dEFa0D-efC09fa1")]
    [TestCase("5254d-3A18EBCA-Bca19f87-a291AF3b-9AbF977E")]
    [TestCase("3516A-Cab6Af5E-D735FDD9-3c6e627F-BDafe3dC")]
    [TestCase("7139f-CBb00BFF-dAf2eb3D-91ede17F-FA928DCC")]
    [TestCase("0105A-4cC1ca85-b6cAbbC1-Cbce0be3-AB1F26Bb")]
    [TestCase("2154b-c61fCeb8-38d18eBc-D7fD3fec-Fbc1aDA7")]
    [TestCase("9358C-5B38aC8b-c8dfe23b-fCfEffB7-Faa6Dbbe")]
    [TestCase("0561a-aec6aFBc-d0BfbF2C-eE7a700D-EDcEEaE6")]
    [TestCase("1215a-A6B196D0-3A0ABcc1-EABBdA07-8E6aA583")]
    [TestCase("9729e-7a96c07e-Edd24fBa-dCb2CABD-7CCDbc9d")]
    [TestCase("7732e-04ebB5dd-65e8C7B7-de87Bffd-Ed5cE962")]
    [TestCase("4920a-Cbf01cad-3afF0e26-71ACACfa-AcF2702B")]
    [TestCase("9958F-b0eA73bB-7bc8DefB-AD8AcC34-c0Eb7a77")]
    [TestCase("9017E-5CeDf78f-30CC9b3a-BABDbC5C-2Cf94dd9")]
    [TestCase("3983e-adaCFafb-3deBAbdA-F7CBfdbd-DeadbcC8")]
    [TestCase("3633A-60BB4ff1-B6fe6bCe-1eCBfC6A-BC3ce06a")]
    [TestCase("2356E-ad6F9dBb-67BFeDb3-9faeEcf3-B8eA624a")]
    [TestCase("3261e-fa8eecCF-D5d0ceBe-6cC652C3-eacfe3F4")]
    [TestCase("1655e-0dBF8aAC-cEe80Ff3-3cBef3c4-3Dc5aCCA")]
    [TestCase("4550b-c7BFCe9b-c7FCCbba-C5402224-60edCCAe")]
    [TestCase("5421b-aC74E0Cc-DBA03c64-Ad3Ffd9d-e95eBbBa")]
    [TestCase("0713e-5d5cCe77-bB7fCBB4-B38eDfcE-67CA7F0e")]
    [TestCase("0007a-C69DcaD1-844A6476-DABFfE8B-7CbAEABC")]
    [TestCase("4059E-df64B71b-FCAcaDf6-B03FdDFf-edfE5fe0")]
    [TestCase("7000b-ddDBcBBB-abdeBE8c-B10fAB1B-adaCdAEe")]
    [TestCase("4287a-b5FADcdB-BcEcfA2b-DDf8fE0d-bbaE22BC")]
    [TestCase("2436B-DabaE22c-Dee6BDc5-FaaceAC8-B97cDfD5")]
    [TestCase("4671d-132aECdB-7af2dADF-ebC5dbe3-Abad8E9C")]
    [TestCase("2460F-EfAEFDe2-DdfC5aAe-d28f4Afc-Ced9Fe10")]
    [TestCase("2556a-Ad36e9cD-f0dAeC4C-09A5c1fA-c87d1Ced")]
    [TestCase("9951d-bcaDCE7e-EdCb0A69-A1fb63d3-ab9A1e8c")]
    public void IsValidApiKeyTest(string apiKey)
    {
        Assert.IsTrue(ApiKey.IsValidApiKey(apiKey));
    }

    [TestCase("7096E-FcD1bD5D-6FBBe34f-ab7eECed-35EEBEcf-")]
    [TestCase("6302B-8BdbFCE9-2fCadfEF-3c5eAbA5-6aAbAc599")]
    [TestCase("5810C-f0D8a5BA-6fBBe3ff-e5f5Ef67-233a6C/dc")]
    [TestCase("4260B-aC04Cc9E-fa8ccfc4-BcEFBa51-fABbeDf")]
    [TestCase("5451a-c92feaA6-8de38AbF-Daced62C-77fD83")]
    [TestCase("2238d-A1Ff75b1-D68EDcD8-dbF5F1a-91eCcbf6")]
    [TestCase("4314b-ABfF7C0f9-1f1DE0B2-5CBcaEd-b5B4CCDe")]
    [TestCase("8047B-5AaDaD6FE-c96e6a99-CA4cBd0-bfaecaF9")]
    [TestCase("4158DD5D8F9eA-bAA6e5CB-cbD21A33-eCFjaceB3")]
    [TestCase("A423D-F2361BDC-11CBF13F-FD8FeCBd-75taEDaA")]
    [TestCase("7989D-1C3c02eE-0B616dd5-4eC38FB7-eBaa3cD1f")]
    [TestCase("0854C-61b657fB-7EFED4cE-8baEE3a3-dE532C5b0")]
    [TestCase("1547e-AEDf6A98-7F6A2Dfb-BEcAcC5E-3EbBnfD6")]
    [TestCase("0762d-e3d1C88f-3b3FAbAg-0aEa6fcD-a6AEC39B")]
    [TestCase("4189F-DB5dB45E-FCFFf35--A3dEFa0D-efC09fa1")]
    [TestCase("5254d-3A18CA-Bca19f87-a291AF3b-9AbF977E")]
    [TestCase("3516A-Cab6Af5E-D735FDD@-3c6e627F-BDafe3dC")]
    [TestCase("7139f-CBb00BFF-dAf§eb3D-91ede17F-FA928DCC")]
    [TestCase("0105A-4cC1ca85-b6c%bbC1-Cbce0be3-AB1F26Bb")]
    [TestCase("2154b-c61fCeb8-38d#8eBc-D7fD3fec-Fbc1aDA7")]
    [TestCase("9358C-5B38aC8b-c8dfe&3b-fCfEffB7-Faa6Dbbe")]
    [TestCase("0561a-aec6aFBc-d0Bfb/2C-eE7a700D-EDcEEaE6")]
    [TestCase("1215a-A6B196D0-3A0AByc1-EABBdA07-8E6aA583")]
    [TestCase("97293-7a96c07e-Edd24fBa-dCb2CABD-7CCDbc9d")]
    [TestCase("77322-04ebB5dd-65e8CiB7-de87Bffd-Ed5cE962")]
    [TestCase("4920a-Cbf1cad-3afF0e26-71ACACfa-AcF2702B")]
    [TestCase("9958F-b0eåä3bB-7bc8DefB-AD8AcC34-c0Eb7a77")]
    [TestCase("9017E-5CeDö78f-30CC9b3a-BABDbC5C-2Cf94dd9")]
    [TestCase("3983e-adaCoafb-3deBAbdA-F7CBfdbd-DeadbcC8")]
    [TestCase("3633A-60BB4ff1_B6fe6bCe-1eCBfC6A-BC3ce06a")]
    [TestCase("2356E-ad6F9dBb-r7BFeDb3-9faeEcf3-B8eA624a")]
    [TestCase("3261e-fa8eecCF-D50ceBe-cC652C3-eacfe3F4")]
    [TestCase("1655e-0dBF8aAC-cEe80F--3cBef3c4-3Dc5aCCA")]
    [TestCase("4550b-c7BFCe9b-c7FCCibbas-C5402224-60edCCAe")]
    [TestCase("5421b-aC74E0Cc-DBA03c64-Ad53Ffd9d-e95eBbBa")]
    [TestCase("0713e-5d5cCe77-bB7fCBB4-B38yrteDfcE-67CA7F0e")]
    [TestCase("0007a-C69DcaD1-844A6476-DABFsfE8B-7CbAEABC")]
    [TestCase("4059E-df64B71b-FCAcaDf6-B03FdDFf")]
    [TestCase("7000b-ddDBcBBB-adaCdAEe")]
    [TestCase("-BcEcfA2b-DDf8fE0d-bbaE22BC")]
    [TestCase("2436h-DabaE22c-Dee6BDc5-FaaceAC8-B97cDfD5")]
    [TestCase("467fd-132aECdB-7af2dADF-ebC5dbe3-Abad8E9C")]
    [TestCase("2460F-gfAEFDe2-DdfC5aAe-d28f4Afc-Ced9Fe10")]
    [TestCase("2556a-Gd36e9cD-f0dAeC4C-09A5c1fA-c87d1Ced")]
    [TestCase("99512-bcaDCE7e-EdCb0A69-A1fb63d3-ab9A1e8c")]
    public void IsInvalidApiKeyTest(string apiKey)
    {
        Assert.IsFalse(ApiKey.IsValidApiKey(apiKey));
    }

    [Test]
    public void IsValidGeneratedApiKey()
    {
        Assert.IsTrue(ApiKey.IsValidApiKey(ApiKey.GenerateApiKey()));
    }
}