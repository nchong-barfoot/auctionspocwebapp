using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Tests.Services
{
    [TestClass]
    public class VenuesServiceTests
    {
        private VenueService _venuesService;
        private Mock<ILogger<Venue>> _logger;
        private Mock<IVenueRepository> _venueRepository;
        private Mock<IAuctionSessionRepository> _auctionSessionRepository;
        private List<Venue> _venueList;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Venue>>();
            _venueRepository = new Mock<IVenueRepository>();
            _auctionSessionRepository = new Mock<IAuctionSessionRepository>();
            _venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue",
                    Suburb = "Auckland City",
                    Street = "Shortland Street",
                    Region = "Auckland",
                    OnSite = true
                },
                new Venue
                {
                    VenueId = 1,
                    Name = "Test Venue 2",
                    Suburb = "Hamilton",
                    Street = "Hamilton Street",
                    Region = "Waikato",
                    OnSite = false
                }
            };
            _venueRepository.Setup(v => v.GetVenueById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_venueList.Single(v => v.VenueId == id)));
            
            _venuesService = new VenueService(_logger.Object, _venueRepository.Object, _auctionSessionRepository.Object, Mapper.Instance);
        }

        [TestMethod]
        public void Get_Venue_By_Id_With_Valid_Venue_Requested()
        {
            var testVenue = _venueRepository.Object.GetVenueById(1);
            Assert.AreEqual("Test Venue 2", testVenue.Result.Name);
        }

        [TestMethod]
        public async Task Get_Venue_By_Id_Invalid_Not_Found_Venue_Requested()
        {
            _venueRepository.Setup(v => v.GetVenueById(It.IsAny<int>()))
                .Returns<int>(id =>
                    Task.FromResult(_venueList.Single(v => v.VenueId == id)));
            //Action
            var venue = await _venuesService.GetVenueById(2);

            //Result
            Assert.IsTrue(venue.IsCancelled);
        }


        [TestMethod]
        public async Task Get_All_Venues_In_System()
        {
            //Setup
            var venueTask = Task.FromResult(_venueList as IEnumerable<Venue>);

            //Action
            _venueRepository.Setup(v => v.GetVenues()).Returns(venueTask);
            var venueListResult = await _venuesService.GetVenues();

            //Result
            Assert.AreEqual(venueListResult.Count(), 2);
        }

        [TestMethod]
        public async Task Get_All_Venues_Empty_Venue_List_In_System()
        {
            //Setup
            var venueList = new List<Venue>();
            var venueTask = Task.FromResult((IEnumerable<Venue>)venueList);
            _venueRepository.Setup(v => v.GetVenues()).Returns(venueTask);

            //Action
            var venueListResult = await _venuesService.GetVenues();

            //Result
            Assert.AreEqual(venueListResult.Count(), 0);
        }

        [TestMethod]
        public async Task Update_Venue_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var venueDto = new VenueDto {Name = "Test Venue"};

            //Action
            var venueResult = await _venuesService.UpdateVenue(1, venueDto);

            //Result
            Assert.AreEqual(venueResult.Name, "Test Venue");
            Assert.AreEqual(venueResult.Region, "Waikato");
            Assert.IsFalse(venueResult.IsCancelled);
        }

        [TestMethod]
        public async Task Update_Venue_Not_Found_In_System_With_Valid_Fields()
        {
            //Setup
            var venueDto = new VenueDto { Name = "Test Venue" };

            //Action
            var venueResult = await _venuesService.UpdateVenue(2, venueDto);

            //Result
            Assert.IsTrue(venueResult.IsCancelled);
        }
        
        [TestMethod]
        public async Task Add_Valid_Venue_To_System()
        {
            //Setup
            var venueToAddDto = new VenueDto
            {
                Name = "Test Venue 2",
                Suburb = "Auckland City",
                Street = "Shortland Street",
                Region = "Auckland",
            };

            var venueAddList = new ConcurrentBag<Venue>();
            _venueRepository.Setup(a => a.AddVenue(It.IsAny<Venue>()))
                .Returns<Venue>(s =>
                {
                    venueAddList.Add(s);
                    return Task.FromResult(s);
                });

            //Action
            var venueAdded = await _venuesService.AddVenue(venueToAddDto);

            //Result
            Assert.IsFalse(venueAdded.IsCancelled);
            Assert.AreEqual(1, venueAddList.Count);
        }
        
        [TestMethod]
        public async Task Remove_Valid_Venue_From_System()
        {
            //Setup
            var venueRemoveList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 0,
                    Name = "Test Venue",
                    Suburb = "Auckland City",
                    Street = "Shortland Street",
                    Region = "Auckland",
                    OnSite = true
                }
            };

            _venueRepository.Setup(a => a.DeleteVenue(It.IsAny<Venue>()))
                .Returns(Task.CompletedTask)
                .Callback<Venue>(v => venueRemoveList.RemoveAll(li => v.VenueId == li.VenueId));
           
            //Action
            await _venuesService.DeleteVenue(0);

            //Result
            Assert.AreEqual(0, venueRemoveList.Count);
        }

        [TestMethod]
        public async Task Get_PagedVenues_With_Valid_Results()
        {
            //Setup
            int pageNumber = 1;
            int pageSize = 3;

            var venueList = new List<Venue>
            {
                new Venue
                {
                    VenueId = 1,
                    Name = "Test Venue 1"
                },
                new Venue
                {
                    VenueId = 2,
                    Name = "Test Venue 2"
                },
                new Venue
                {
                    VenueId = 3,
                    Name = "Test Venue 3"
                },
                new Venue
                {
                    VenueId = 4,
                    Name = "Test Venue 4"
                },
                new Venue
                {
                    VenueId = 5,
                    Name = "Test Venue 5"
                }
            }.AsQueryable();

            _venueRepository.Setup(a => a.GetPagedVenues())
                .Returns(venueList);

            //Action
            var testVenues = await _venuesService.GetPagedVenues(pageNumber, pageSize);

            //Result
            Assert.AreEqual(3, testVenues.List.Count());
        }

        [TestMethod]
        public async Task Get_PagedVenues_With_Empty_Results()
        {
            //Setup
            int pageNumber = 1;
            int pageSize = 3;

            var venueList = new List<Venue>().AsQueryable();

            _venueRepository.Setup(a => a.GetPagedVenues())
                .Returns(venueList);

            //Action
            var testVenues = await _venuesService.GetPagedVenues(pageNumber, pageSize);

            //Result
            Assert.AreEqual(0, testVenues.List.Count());
        }
    }
}
