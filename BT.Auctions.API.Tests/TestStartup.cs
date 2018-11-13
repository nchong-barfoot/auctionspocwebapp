using AutoMapper;
using BT.Auctions.API.Models.MappingProfiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BT.Auctions.API.Tests
{
    [TestClass]
    public class TestStartup
    {
        /// <summary>
        /// Assemblies the initialize.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            //Initialise the Automapper here to run once before test execution
            //Static implmementation of the Mapper class is used and shared to help with performance
            //Need to call a reset as NCrunch will continue to run multithreaded and not define where
            //it will start during continous testing
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AuctionSessionProfile>();
                cfg.AddProfile<VenueProfile>();
                cfg.AddProfile<LotProfile>();
                cfg.AddProfile<LotDetailProfile>();
                cfg.AddProfile<DisplayConfigurationProfile>();
                cfg.AddProfile<DisplayProfile>();
                cfg.AddProfile<DisplayGroupProfile>();
                cfg.AddProfile<ImageProfile>();
                cfg.AddProfile<BidProfile>();
                cfg.AddProfile<MediaProfile>();
            });
        }
    }
}
