using BT.Auctions.Poc.Models;
using BT.Auctions.PoC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BT.Auctions.PoC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        // GET api/videos/5
        [HttpGet("{id}")]
        public ActionResult<Video> Get(int id)
        {
            return _videoService.GetVideoById(id);
        }
    }
}
