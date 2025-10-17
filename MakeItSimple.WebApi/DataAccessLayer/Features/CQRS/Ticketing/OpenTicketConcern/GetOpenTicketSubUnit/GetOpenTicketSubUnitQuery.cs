using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit
{
    public partial class GetOpenTicketSubUnit
    {
        public class GetOpenTicketSubUnitQuery : UserParams, IRequest<PagedList<GetOpenTicketSubUnitResult>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string TicketStatus { get; set; }
            public string Role { get; set; }
            public string Search { get; set; }
            public bool? AscendingDate { get; set; }
        }
    }
}
