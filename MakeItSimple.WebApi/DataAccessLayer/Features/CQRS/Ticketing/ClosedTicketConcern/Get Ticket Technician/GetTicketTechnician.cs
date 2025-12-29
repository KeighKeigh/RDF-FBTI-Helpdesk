using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.Get_Ticket_Technician
{
    public class GetTicketTechnician
    {
        public class GetTicketTechnicianResult
        {
            public Guid? TechnicianId { get; set; }
            public string Technician_Name { get; set; }
        }

        public class GetTicketTechnicianQuery : IRequest<Result>
        {
            //public int TicketConcernId { get; set; }
            public int? ChannelId { get; set; }
            //public Guid? UserId { get; set; }
        }

        public class Handler : IRequestHandler<GetTicketTechnicianQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetTicketTechnicianQuery request, CancellationToken cancellationToken)
            {
                //var ticketTechnician = await _context.TicketTechnicians
                //    .Where(x => x.ClosingTicket.TicketConcernId == request.TicketConcernId)
                //    .Select(x => x.TechnicianBy)
                //    .FirstOrDefaultAsync();

                if (request.ChannelId.HasValue)
                {
                    var channelUsers = await _context.ChannelUsers.Where(x => x.ChannelId == request.ChannelId).Select(x => x.UserId).ToListAsync();

                    var query = await _context.Users
                        .Where(x => x.IsActive &&
                        x.UserRole.UserRoleName.Contains(TicketingConString.Technician)
                        /*&& x.Id != ticketTechnician*/ && channelUsers.Contains(x.Id))
                        .Select(x => new GetTicketTechnicianResult
                        {
                            TechnicianId = x.Id,
                            Technician_Name = x.Fullname,

                        }).ToListAsync();

                    return Result.Success(query);
                }
                return Result.Failure(UserError.ChannelIdNull());
            }
        }
    }
}
