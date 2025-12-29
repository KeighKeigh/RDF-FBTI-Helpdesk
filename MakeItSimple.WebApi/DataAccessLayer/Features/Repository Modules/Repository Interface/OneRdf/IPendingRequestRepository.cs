using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.OneCharging;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.CreatePendingRequest.CreatePendingRequestHandler;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.OneRdf
{
    public interface IPendingRequestRepository
    {
        Task<User> ExistingUserByEmpId(string idPrefix, string idNumber);
        Task<PendingRequest> ExistingPendingUserByEmpId(string idPrefix, string idNumber);

        Task<bool> UpdateExistingUser(CreatePendingRequestCommand command);
        Task<bool> UpdateExistingPendingUser(CreatePendingRequestCommand command);
        Task<PendingRequest> AddNewPendingAccount(CreatePendingRequestCommand command);
        Task<bool> UsernameExist(string username);

        Task<bool> UsernameExistInPendingRequest(string idPrefix, string idNo);
    }
}
