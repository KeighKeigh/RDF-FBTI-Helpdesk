using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Ticketing
{
    public class ClosingTicketRepository : IClosingRepository
    {
        private readonly MisDbContext context;

        public ClosingTicketRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<ApproverTicketing> ApproverThatNotNullByClosingTicket(int? id)
        {
            return await context.ApproverTicketings
                .Where(x => x.ClosingTicketId == id && x.IsApprove != null)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Approver>> ApproverBySubUnitList(int? id)
        {
            return await context.Approvers
                 .Include(x => x.User)
                 .Where(x => x.SubUnitId == id)
                 .ToListAsync();
        }

        public async Task<List<ApproverTicketing>> ApproverThatNullByClosingTicketList(int? id)
        {
            return await context.ApproverTicketings
                .Where(x => x.ClosingTicketId == id && x.IsApprove == null)
                .ToListAsync();
        }

        public async Task<ClosingTicket> ClosingTicketExist(int? id)
        {
            return await context.ClosingTickets
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.RequestorByUser)
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.RequestConcern)
                .ThenInclude(x => x.User)
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateApproval(ApproverTicketing approverTicketing, CancellationToken cancellationToken)
        {
            await context.ApproverTicketings.AddAsync(approverTicketing, cancellationToken);
        }

        public async Task CreateClosingTicket(ClosingTicket closingTicket, CancellationToken cancellationToken)
        {
            await context.ClosingTickets.AddAsync(closingTicket);
        }

        public async Task CreateTicketTechnician(TicketTechnician ticketTechnician, CancellationToken cancellationToken)
        {
            await context.TicketTechnicians.AddAsync(ticketTechnician, cancellationToken);
        }

        public async Task ForClosingTicket(int? id)
        {
            await context.TicketConcerns
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsClosedApprove, u => false));
        }

        public async Task RemoveTicketTechnician(int id, List<int> ticketTechnicianId, CancellationToken cancellationToken)
        {
            await context.TicketSubCategories
                     .Where(x => x.RequestConcernId == id && !ticketTechnicianId.Contains(x.Id))
                     .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<TicketTechnician> TicketTechnicianExist(int? id)
        {
            return await context.TicketTechnicians.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateClosingTicket(ClosingTicket closingTicket, CancellationToken cancellationToken)
        {
            bool IsChanged = false;

            var update = await context.ClosingTickets
                .FirstOrDefaultAsync(x => x.Id == closingTicket.Id, cancellationToken);

            if (update.Resolution != closingTicket.Resolution)
            {
                update.Resolution = closingTicket.Resolution;
                IsChanged = true;
            }

            if (update.Notes != closingTicket.Notes)
            {
                update.Notes = closingTicket.Notes;
                IsChanged = true;
            }

            if (update.CategoryConcernId != closingTicket.CategoryConcernId)
            {
                update.CategoryConcernId = closingTicket.CategoryConcernId;
                IsChanged = true;
            }

            if (update.CategoryConcernName != closingTicket.CategoryConcernName)
            {
                update.CategoryConcernName = closingTicket.CategoryConcernName;
                IsChanged = true;
            }
            //update.IsClosing = true;
            if (IsChanged)
            {
                update.ModifiedBy = closingTicket.ModifiedBy;
                update.UpdatedAt = DateTime.Now;
            }
        }

        public async Task<ApproverTicketing> ApproverByClosingTicketList(int? id)
        {
            return await context.ApproverTicketings
                 .Where(x => x.ClosingTicketId == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<ApproverTicketing> ApproverByMinLevel(int? id)
        {
            var approverList = await context.ApproverTicketings
                  .Where(x => x.ClosingTicketId == id && x.IsApprove == null)
                  .FirstOrDefaultAsync();

            return approverList;
        }

        public async Task<ApproverTicketing> ApproverPlusOne(int? id)
        {
            return await context.ApproverTicketings
                .Include(x => x.User)
                .Where(x => x.ClosingTicketId == id)
                .FirstOrDefaultAsync();
        }

        public async Task ApprovedApproval(int? id)
        {
            await context.ApproverTicketings
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsApprove, u => true));
        }

        public async Task ApprovedClosingTicket(ClosingTicket closingTicket, CancellationToken cancellationToken)
        {

            await context.ClosingTickets
                .Where(x => x.Id == closingTicket.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.TicketApprover, u => null)
                .SetProperty(u => u.ClosingAt, u => DateTime.Now)
                .SetProperty(u => u.IsClosing, u => closingTicket.IsClosing)
                .SetProperty(u => u.ClosedBy, u => closingTicket.ClosedBy));
        }

        public async Task ApprovedTicketConcernByClosing(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {

            await context.TicketConcerns
                .Where(x => x.Id == ticketConcern.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsClosedApprove, u => ticketConcern.IsClosedApprove)
                .SetProperty(u => u.Closed_At, u => DateTime.Now)
                .SetProperty(u => u.ClosedApproveBy, u => ticketConcern.ClosedApproveBy)
                .SetProperty(u => u.IsDone, u => true)
                .SetProperty(u => u.ConcernStatus, u => ticketConcern.ConcernStatus));
        }

        public async Task ApprovedRequestConcernByClosing(RequestConcern requestConcern, CancellationToken cancellation)
        {
            await context.RequestConcerns
                .Where(x => x.Id == requestConcern.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsDone, u => true)
                .SetProperty(u => u.Resolution, u => requestConcern.Resolution)
                .SetProperty(u => u.ConcernStatus, u => requestConcern.ConcernStatus)
                .SetProperty(u => u.CategoryConcernName, u => requestConcern.CategoryConcernName));

        }

        //public async Task AddForClosingDate(TicketConcern ticketConcern, CancellationToken cancellationToken)
        //{

        //    await context.TicketConcerns
        //        .Where(x => x.Id == ticketConcern.Id)
        //        .ExecuteUpdateAsync(update => update
        //        .SetProperty(u => u.ForClosingDate, u => ticketConcern.ForClosingDate));
        //}

        public async Task NextApproverUser(int? id, Guid? userId)
        {
            await context.ClosingTickets
                 .Where(x => x.Id == id)
                 .ExecuteUpdateAsync(update => update
                 .SetProperty(u => u.TicketApprover, u => userId));
        }

        public async Task RemoveClosingApprover(int? id)
        {
            await context.ApproverTicketings
                 .Where(x => x.ClosingTicketId == id)
                 .ExecuteDeleteAsync();
        }
        public async Task RemoveClosingTicket(int? id)
        {
            await context.ClosingTickets
                 .Where(x => x.TicketConcernId == id)
                 .ExecuteDeleteAsync();
        }

        public async Task CancelClosingTicket(int? id)
        {
            var cancelClosing = await context.ClosingTickets
                  .FirstOrDefaultAsync(x => x.Id == id);


            await context.ClosingTickets
                .Where(x => x.Id == id)
                .Include(x => x.TicketConcern)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsActive, u => false)
                .SetProperty(u => u.ForClosingAt, u => null));

            await context.TicketConcerns
              .Where(x => x.Id == cancelClosing.TicketConcernId)
              .ExecuteUpdateAsync(update => update
              .SetProperty(u => u.IsClosedApprove, u => null));


        }

        public async Task ConfirmClosingTicket(int? id)
        {
            await context.RequestConcerns
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(confirm => confirm
                .SetProperty(c => c.Is_Confirm, c => true)
                .SetProperty(c => c.Confirm_At, c => DateTime.Now)
                .SetProperty(c => c.ConcernStatus, c => TicketingConString.Done));
        }

        public async Task ConfirmTicketHistory(int? id)
        {
            var updateTicketConcern = await context.TicketConcerns
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            await context.TicketHistories
                .Where(x => x.TicketConcernId == id &&
                x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.TransactedBy, u => updateTicketConcern.RequestorBy)
                .SetProperty(u => u.TransactionDate, u => DateTime.Now)
                .SetProperty(u => u.Request, u => TicketingConString.Confirm)
                .SetProperty(u => u.Status, u => TicketingConString.CloseConfirm));
        }
        //kk

        public async Task RequestorConfirmation(int? id, Guid? requestor)
        {
            var requestApprover = await context.TicketConcerns
                .Where(x => x.RequestConcernId == id && x.AddedBy == requestor).FirstOrDefaultAsync();


        }

        public async Task RejectClosingTicket(ClosingTicket closingTicket)
        {
            var rejectClosing = await context.ClosingTickets
                .FirstOrDefaultAsync(x => x.Id == closingTicket.Id);


            await context.ClosingTickets
                .Where(x => x.Id == closingTicket.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.RejectClosedAt, u => DateTime.Now)
                .SetProperty(u => u.IsRejectClosed, u => true)
                .SetProperty(u => u.RejectClosedBy, u => closingTicket.RejectClosedBy)
                .SetProperty(u => u.RejectRemarks, u => closingTicket.RejectRemarks)
                .SetProperty(u => u.Remarks, u => closingTicket.RejectRemarks)
                .SetProperty(u => u.ForClosingAt, u => null));


            await context.TicketConcerns
                .Where(x => x.Id == rejectClosing.TicketConcernId)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsClosedApprove, u => null)
                .SetProperty(u => u.Remarks, u => closingTicket.RejectRemarks));

        }
        //public async Task CancelClosingTicket(ClosingTicket closingTicket)
        //{
        //    var rejectClosing = await context.ClosingTickets
        //        .FirstOrDefaultAsync(x => x.Id == closingTicket.Id);


        //    await context.ClosingTickets
        //        .Where(x => x.Id == closingTicket.Id)
        //        .ExecuteUpdateAsync(update => update
        //        .SetProperty(u => u.RejectClosedAt, u => DateTime.Now)
        //        .SetProperty(u => u.IsRejectClosed, u => true)
        //        .SetProperty(u => u.RejectClosedBy, u => closingTicket.RejectClosedBy)
        //        .SetProperty(u => u.RejectRemarks, u => closingTicket.RejectRemarks)
        //        .SetProperty(u => u.Remarks, u => closingTicket.RejectRemarks)
        //        .SetProperty(u => u.ForClosingAt, u => null));


        //    await context.TicketConcerns
        //        .Where(x => x.Id == rejectClosing.TicketConcernId)
        //        .ExecuteUpdateAsync(update => update
        //        .SetProperty(u => u.IsClosedApprove, u => null)
        //        .SetProperty(u => u.Remarks, u => closingTicket.RejectRemarks));

        //}


        public async Task ReturnClosingTicket(int? id, string status, string remarks)
        {

            var ticketConcernExist = await context.TicketConcerns
                .FirstOrDefaultAsync(x => x.RequestConcernId == id);


            await context.RequestConcerns
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.ConcernStatus, u => status)
                .SetProperty(u => u.IsDone, u => false));

            await context.TicketConcerns
                .Where(x => x.Id == ticketConcernExist.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsClosedApprove, u => null)
                .SetProperty(u => u.Closed_At, u => null)
                .SetProperty(u => u.ClosedApproveBy, u => null)
                .SetProperty(u => u.ConcernStatus, u => status)
                .SetProperty(u => u.IsDone, u => null)
                .SetProperty(u => u.Remarks, u => remarks));


            await context.ClosingTickets
                .Where(x => x.TicketConcernId == ticketConcernExist.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsRejectClosed, u => true)
                .SetProperty(u => u.IsActive, u => false));



        }
    }
}
