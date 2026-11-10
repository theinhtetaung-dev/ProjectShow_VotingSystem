using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingSystem.Domain.Features.Event;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<Result<PageResult<EventResponseModel>>>> GetEvents(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _eventService.GetEventlist();
            return response.ToPageResult(pageNumber, pageSize).ToActionResult();
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<Result<EventResponseModel>>> GetEventById(long id)
        {
            var response = await _eventService.GetEventByID(id);
            return response.ToActionResult();
        }

        [HttpGet("{id:long}/dashboard")]
        public async Task<ActionResult<Result<EventDashboardResponseModel>>> GetEventDashboard(long id)
        {
            var response = await _eventService.GetEventDashboard(id);
            return response.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<Result<EventResponseModel>>> CreateEvent(
            [FromBody] EventCreateRequestModel request)
        {
            var response = await _eventService.CreateEventAsync(request);
            return response.ToActionResult();
        }

        [HttpPatch]
        public async Task<ActionResult<Result<EventResponseModel>>> UpdateEvent(
            [FromBody] EventUpdateRequestModel request)
        {
            var response = await _eventService.UpdateEvent(request);
            return response.ToActionResult();
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult<Result<EventResponseModel>>> DeleteEvent(long id)
        {
            var response = await _eventService.DeleteEvent(id);
            return response.ToActionResult();
        }
    }
}
