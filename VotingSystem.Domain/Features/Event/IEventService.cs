using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Event
{
    public interface IEventService
    {
        Task<Result<EventResponseModel>> CreateEventAsync(EventCreateRequestModel request);
        Task<Result<EventResponseModel>> DeleteEvent(long id);
        Task<Result<EventResponseModel>> GetEventByID(long id);
        Task<Result<List<EventResponseModel>>> GetEventlist();
        Task<Result<EventResponseModel>> UpdateEvent(EventUpdateRequestModel request);
    }
}