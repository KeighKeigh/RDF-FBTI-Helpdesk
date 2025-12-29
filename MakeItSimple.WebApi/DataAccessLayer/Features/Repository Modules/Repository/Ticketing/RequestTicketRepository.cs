using DocumentFormat.OpenXml.Office2010.Excel;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Ticketing
{
    public class RequestTicketRepository : IRequestTicketRepository
    {
        private readonly MisDbContext context;
        private readonly ContentType contentType;

        public RequestTicketRepository(MisDbContext context, ContentType contentType)
        {
            this.context = context;
            this.contentType = contentType;
        }

        public async Task<RequestConcern> RequestConcernExist(int? id)
        {
            return await context.RequestConcerns
                .Include(x => x.TicketConcerns)
                .Include(x => x.User)
                .ThenInclude(x => x.OneChargingMIS)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //public async Task<TicketConcern> TicketConcernExist(int? id)
        //{
        //    return await context.TicketConcerns
        //        .Include(x => x.User)
        //        .ThenInclude(x => x.Department)
        //        .FirstOrDefaultAsync(x => x.Id == id);
        //}
        public async Task<TicketConcern> TicketConcernExistByRequestConcernId(int? id)
        {
            return await context
                  .TicketConcerns.FirstOrDefaultAsync(x => x.RequestConcernId == id);

        }

        public async Task<TicketConcern> TicketConcernExist(int? id)
        {
           return await context
                .TicketConcerns 
                .Include(x => x.User)
                .Include(x => x.RequestorByUser)
                .FirstOrDefaultAsync(x => x.Id == id);   
        }

        public async Task<TicketConcern> TicketConcernExistAndRequestConcern(int? id, int? reqId)
        {
            return await context
                 .TicketConcerns
                 .Include(x => x.User)
                 .Include(x => x.RequestorByUser).Where(x => x.RequestConcernId == reqId)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        //public async Task<TicketConcern> TicketConcernExistByRequestConcern(int? id)
        //{
        //    return await context
        //         .TicketConcerns
        //         .Include(x => x.User)
        //         .Include(x => x.RequestorByUser)
        //         .FirstOrDefaultAsync(x => x.RequestConcernId == id);
        //}

        public async Task UpdateTicketConcerns(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {
            var update = await context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == ticketConcern.Id, cancellationToken);

            if (update.TargetDate != ticketConcern.TargetDate && ticketConcern.TargetDate is not null)
            {
                update.TargetDate = ticketConcern.TargetDate;
                
            }

            if (update.UserId != ticketConcern.UserId && ticketConcern.UserId is not null)
            {
                update.UserId = ticketConcern.UserId;

            }

            if (update.IsApprove != ticketConcern.IsApprove && ticketConcern.IsApprove is not null)
            {
                update.IsApprove = ticketConcern.IsApprove;

            }

            if (update.ApprovedBy != ticketConcern.ApprovedBy && ticketConcern.ApprovedBy is not null)
            {
                update.ApprovedBy = ticketConcern.ApprovedBy;

            }

            if (update.ApprovedAt != ticketConcern.ApprovedAt && ticketConcern.ApprovedAt is not null)
            {
                update.ApprovedAt = ticketConcern.ApprovedAt;

            }
            if (update.ConcernStatus != ticketConcern.ConcernStatus && ticketConcern.ConcernStatus is not null)
            {
                update.ConcernStatus = ticketConcern.ConcernStatus;

            }
            if (update.IsAssigned != ticketConcern.IsAssigned && ticketConcern.IsAssigned is not null)
            {
                update.IsAssigned = ticketConcern.IsAssigned;

            }
            if (update.AssignTo != ticketConcern.AssignTo && ticketConcern.AssignTo is not null)
            {
                update.AssignTo = ticketConcern.AssignTo;

            }
            if (update.ConcernStatus != ticketConcern.ConcernStatus && ticketConcern.ConcernStatus is not null)
            {
                update.ConcernStatus = ticketConcern.ConcernStatus;
            }
            if (update.IsDateApproved != ticketConcern.IsDateApproved && ticketConcern.IsDateApproved is not null)
            {
                update.IsDateApproved = ticketConcern.IsDateApproved;
            }
            if (update.DateApprovedAt != ticketConcern.DateApprovedAt && ticketConcern.DateApprovedAt is not null)
            {
                update.DateApprovedAt = ticketConcern.DateApprovedAt;
            }
            if (update.ApprovedDateBy != ticketConcern.ApprovedDateBy && ticketConcern.ApprovedDateBy is not null)
            {
                update.ApprovedDateBy = ticketConcern.ApprovedDateBy;
            }

            if (update.Reason != ticketConcern.Reason && ticketConcern.Reason is not null)
            {
                update.Reason = ticketConcern.Reason;
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdateRequestConcern(RequestConcern requestConcern, CancellationToken cancellationToken)
        {
            bool isChange = false;

            var update = await context.RequestConcerns
                .FirstOrDefaultAsync(x => x.Id == requestConcern.Id,cancellationToken);

            if (update.Concern != requestConcern.Concern && !string.IsNullOrEmpty(requestConcern.Concern))
            {
                update.Concern = requestConcern.Concern;
                isChange = true;
            }

            if (update.ChannelId != requestConcern.ChannelId && requestConcern.ChannelId is not null)
            {
                update.ChannelId = requestConcern.ChannelId;
                isChange = true;
            }

            if (update.ContactNumber != requestConcern.ContactNumber && requestConcern.ContactNumber is not null)
            {
                update.ContactNumber = requestConcern.ContactNumber;
                isChange = true;
            }

            if (update.RequestType != requestConcern.RequestType && requestConcern.RequestType is not null)
            {
                update.RequestType = requestConcern.RequestType;
                isChange = true;
            }

            if (update.DateNeeded != requestConcern.DateNeeded && requestConcern.DateNeeded is not null)
            {
                update.DateNeeded = requestConcern.DateNeeded;
                isChange = true;
            }

            if (update.BackJobId != requestConcern.BackJobId && requestConcern.BackJobId is not null)
            {
                update.BackJobId = requestConcern.BackJobId;
                isChange = true;
            }

            if(update.ConcernStatus != requestConcern.ConcernStatus && requestConcern.ConcernStatus is not null)
            {
                update.ConcernStatus = requestConcern.ConcernStatus;
            }

            if (update.TargetDate != requestConcern.TargetDate && requestConcern.TargetDate is not null)
            {
                update.TargetDate = requestConcern.TargetDate;
            }
            if (update.AssignTo != requestConcern.AssignTo && requestConcern.AssignTo is not null)
            {
                update.AssignTo = requestConcern.AssignTo;
            }
            if (update.SubUnitId != requestConcern.SubUnitId && requestConcern.SubUnitId is not null)
            {
                update.SubUnitId = requestConcern.SubUnitId;
            }
            if (update.UnitId != requestConcern.UnitId && requestConcern.UnitId is not null)
            {
                update.UnitId = requestConcern.UnitId;
            }
            if (update.UserId != requestConcern.UserId && requestConcern.UserId is not null)
            {
                update.UserId = requestConcern.UserId;
            }
            if (update.DepartmentId != requestConcern.DepartmentId && requestConcern.DepartmentId is not null)
            {
                update.DepartmentId = requestConcern.DepartmentId;
            }
            if (update.DatePicked != requestConcern.DatePicked)
            {
                update.DatePicked = requestConcern.DatePicked;
            }

            if (isChange)
            {
                update.ModifiedBy = requestConcern.ModifiedBy;
                update.UpdatedAt = DateTime.Now;
            }

            await context.SaveChangesAsync();

        }


        public async Task UpdateRequest(RequestConcern requestConcern, CancellationToken cancellationToken)
        {
            bool isChange = false;

            var update = await context.RequestConcerns
                .FirstOrDefaultAsync(x => x.Id == requestConcern.Id, cancellationToken);


            if (update.ChannelId != requestConcern.ChannelId && requestConcern.ChannelId is not null)
            {
                update.ChannelId = requestConcern.ChannelId;
                isChange = true;
            }
            if (update.TransferChannelId != requestConcern.TransferChannelId && requestConcern.TransferChannelId is not null)
            {
                update.TransferChannelId = requestConcern.TransferChannelId;
                isChange = true;
            }

            if (update.UserId != requestConcern.UserId && requestConcern.UserId is not null)
            {
                update.UserId = requestConcern.UserId;
                isChange = true;
            }

            if (update.AddedBy != requestConcern.AddedBy && requestConcern.AddedBy is not null)
            {
                update.AddedBy = requestConcern.AddedBy;
                isChange = true;
            }
            if (update.AssignTo != requestConcern.AssignTo && requestConcern.AssignTo is not null)
            {
                update.AssignTo = requestConcern.AssignTo;
            }

            if (isChange)
            {
                update.ModifiedBy = requestConcern.ModifiedBy;
                update.UpdatedAt = DateTime.Now;
            }

        }

        public async Task UpdateTicket(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {
            var update = await context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == ticketConcern.Id, cancellationToken);


            if (update.UserId != ticketConcern.UserId && ticketConcern.UserId is not null)
            {
                update.UserId = ticketConcern.UserId;

            }
            if (update.AssignTo != ticketConcern.AssignTo && ticketConcern.AssignTo is not null)
            {
                update.AssignTo = ticketConcern.AssignTo;

            }
            if (update.AddedBy != ticketConcern.AddedBy && ticketConcern.AddedBy is not null)
            {
                update.AddedBy = ticketConcern.AddedBy;
            }
            if (update.RequestorBy != ticketConcern.RequestorBy && ticketConcern.RequestorBy is not null)
            {
                update.RequestorBy = ticketConcern.RequestorBy;
            }
            if (update.ApprovedDateBy != ticketConcern.ApprovedDateBy && ticketConcern.ApprovedDateBy is not null)
            {
                update.ApprovedDateBy = ticketConcern.ApprovedDateBy;
            }
        }
        //kk

        public async Task UpdateTicketConcernss(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {
            var update = await context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == ticketConcern.Id, cancellationToken);

            if (update.TargetDate != ticketConcern.TargetDate)
            {
                update.TargetDate = ticketConcern.TargetDate;

            }

            if (update.UserId != ticketConcern.UserId && ticketConcern.UserId is not null)
            {
                update.UserId = ticketConcern.UserId;

            }

            if (update.IsApprove != ticketConcern.IsApprove && ticketConcern.IsApprove is not null)
            {
                update.IsApprove = ticketConcern.IsApprove;

            }

            if (update.ApprovedBy != ticketConcern.ApprovedBy && ticketConcern.ApprovedBy is not null)
            {
                update.ApprovedBy = ticketConcern.ApprovedBy;

            }

            if (update.ApprovedAt != ticketConcern.ApprovedAt && ticketConcern.ApprovedAt is not null)
            {
                update.ApprovedAt = ticketConcern.ApprovedAt;

            }
            if (update.ConcernStatus != ticketConcern.ConcernStatus && ticketConcern.ConcernStatus is not null)
            {
                update.ConcernStatus = ticketConcern.ConcernStatus;

            }

            if (update.ConcernStatus != ticketConcern.ConcernStatus && ticketConcern.ConcernStatus is not null)
            {
                update.ConcernStatus = ticketConcern.ConcernStatus;
            }

        }
        public async Task UpdateRequestConcerns(RequestConcern requestConcern, CancellationToken cancellationToken)
        {
            bool isChange = false;

            var update = await context.RequestConcerns
                .FirstOrDefaultAsync(x => x.Id == requestConcern.Id, cancellationToken);

            if (update.Concern != requestConcern.Concern && !string.IsNullOrEmpty(requestConcern.Concern))
            {
                update.Concern = requestConcern.Concern;
                isChange = true;
            }

            if (update.ChannelId != requestConcern.ChannelId && requestConcern.ChannelId is not null)
            {
                update.ChannelId = requestConcern.ChannelId;
                isChange = true;
            }

            if (update.ContactNumber != requestConcern.ContactNumber && requestConcern.ContactNumber is not null)
            {
                update.ContactNumber = requestConcern.ContactNumber;
                isChange = true;
            }

            if (update.RequestType != requestConcern.RequestType && requestConcern.RequestType is not null)
            {
                update.RequestType = requestConcern.RequestType;
                isChange = true;
            }

            if (update.DateNeeded != requestConcern.DateNeeded && requestConcern.DateNeeded is not null)
            {
                update.DateNeeded = requestConcern.DateNeeded;
                isChange = true;
            }

            if (update.BackJobId != requestConcern.BackJobId && requestConcern.BackJobId is not null)
            {
                update.BackJobId = requestConcern.BackJobId;
                isChange = true;
            }

            if (update.ConcernStatus != requestConcern.ConcernStatus && requestConcern.ConcernStatus is not null)
            {
                update.ConcernStatus = requestConcern.ConcernStatus;
            }

            if (update.TargetDate != requestConcern.TargetDate)
            {
                update.TargetDate = requestConcern.TargetDate;
            }

            if (isChange)
            {
                update.ModifiedBy = requestConcern.ModifiedBy;
                update.UpdatedAt = DateTime.Now;
            }

        }

        //kk


        public async Task UpdateTicketAttachment(TicketAttachment ticketAttachment, CancellationToken cancellationToken)
        {
            await context.TicketAttachments
                .Where(x => x.Id == ticketAttachment.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.FileName, u => ticketAttachment.FileName)
                .SetProperty(u => u.FileSize, u => ticketAttachment.FileSize)
                .SetProperty(u => u.Attachment, u => ticketAttachment.Attachment)
                .SetProperty(u => u.ModifiedBy, u => ticketAttachment.ModifiedBy)
                .SetProperty(u => u.UpdatedAt, u => DateTime.Now), cancellationToken);
        }


        public async Task CreateRequestConcern(RequestConcern requestConcern, CancellationToken cancellationToken)
        {
           await context.RequestConcerns.AddAsync(requestConcern,cancellationToken);
        }

        public async Task CreateTicketConcern(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {
            await context.TicketConcerns.AddAsync(ticketConcern,cancellationToken);
        }

        public async Task<List<Approver>> ApproverBySubUnitList(int? id)
        {
            return await context.Approvers
                 .Include(x => x.User)
                 .Where(x => x.SubUnitId == id)
                 .ToListAsync();
        }

        public async Task ApproveDateTicket(ApproverDate approver, CancellationToken cancellationToken)
        {
            await context.ApproverDates.AddAsync(approver);
        }

        public async Task CreateApproval(ApproverTicketing approverTicketing, CancellationToken cancellationToken)
        {
            await context.ApproverTicketings.AddAsync(approverTicketing, cancellationToken);
        }
        public async Task CreateTicketHistory(TicketHistory ticketHistory, CancellationToken cancellationToken)
        {
            await context.TicketHistories.AddAsync(ticketHistory,cancellationToken);
        }

        public async Task CreateTicketNotification(TicketTransactionNotification ticketNotification, CancellationToken cancellationToken)
        {
            await context.TicketTransactionNotifications.AddAsync(ticketNotification, cancellationToken);

        }

        public async Task CreateTicketApproval(ApproverTicketing approverTicketing, CancellationToken cancellationToken)
        {
            await context.ApproverTicketings.AddAsync(approverTicketing, cancellationToken);

        }

        public async Task CreateTicketCategory(TicketCategory ticketCategory, CancellationToken cancellationToken)
        {
           await context.TicketCategories.AddAsync(ticketCategory, cancellationToken);
        }

        public async Task CreateTicketSubCategory(TicketSubCategory ticketSubCategory, CancellationToken cancellationToken)
        {
            await context.TicketSubCategories.AddAsync(ticketSubCategory, cancellationToken);
        }

        public async Task CreateTicketAttachment(TicketAttachment ticketAttachment, CancellationToken cancellationToken)
        {
            await context.TicketAttachments.AddAsync(ticketAttachment, cancellationToken);
        }

        public async Task<int> PossibleRequestId()
        {

            var hasRecords = await context.RequestConcerns.AnyAsync();
            if (!hasRecords)
                return 1;

            var possibleId = await context.RequestConcerns.MaxAsync(x => x.Id);
            return possibleId + 1;
        }

        public async Task<int> PossibleTicketId()
        {
            var hasRecords = await context.TicketConcerns.AnyAsync();
            if (!hasRecords)
                return 1;

            var possibleId = await context.TicketConcerns.MaxAsync(x => x.Id);
            return possibleId + 1;
        }

        public async Task<TicketCategory> TicketCategoryExist(int? id, int? requestConcernId)
        {
           return await context.TicketCategories.FirstOrDefaultAsync(x => x.CategoryId == id && x.RequestConcernId == requestConcernId);
        }

        public async Task<TicketSubCategory> TicketSubCategoryExist(int? id, int? requestConcernId )
        {
            return await context.TicketSubCategories.FirstOrDefaultAsync(x => x.SubCategoryId == id && x.RequestConcernId == requestConcernId);
        }

        public async Task<TicketCategory> TicketCategoryExist(int? id)
        {
            return await context.TicketCategories.FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<TicketSubCategory> TicketSubCategoryExist(int? id)
        {
            return await context.TicketSubCategories.FirstOrDefaultAsync(x => x.SubCategoryId == id);
        }
        public async Task<TicketCategory> TicketCategoryExists(int? id, int? requestConcern)
        {
            return await context.TicketCategories.FirstOrDefaultAsync(x => x.CategoryId == id && x.RequestConcernId == requestConcern);
        }

        public async Task<TicketSubCategory> TicketSubCategoryExists(int? id, int? requestConcern)
        {
            return await context.TicketSubCategories.FirstOrDefaultAsync(x => x.SubCategoryId == id && x.RequestConcernId == requestConcern);
        }

        public async Task<bool> UpdateAttachmentForCherryPicking(ApproverDate approverDate, CancellationToken cancellationToken)
        {
            var ticket = await context.TicketAttachments.Where(x => x.TicketConcernId == approverDate.TicketConcernId).ToListAsync();

            if (!ticket.Any())
            {
                return false;
            }
            foreach (var attachment in ticket)
            {
                attachment.ApproverDateId = approverDate.Id;
            }
            return true;

        }

        public async Task RemoveTicketCategory(int id, List<int?> categoryId,CancellationToken cancellationToken)
        {
            await context.TicketCategories
                .Where(x => x.RequestConcernId == id && !categoryId.Contains(x.CategoryId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task RemoveTicketSubCategory(int id, List<int?> subCategoryId, CancellationToken cancellationToken)
        {
            await context.TicketSubCategories
                .Where(x => x.RequestConcernId == id && !subCategoryId.Contains(x.SubCategoryId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<TicketAttachment> TicketAttachmentExist(int? id)
        {
          return await context.TicketAttachments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateTicketConcern(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {
            bool hasChanged = false;

            var update = await context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == ticketConcern.Id);

            if (update.UserId != ticketConcern.UserId && ticketConcern.UserId is not null)
            {
                update.UserId = ticketConcern.UserId;
                hasChanged = true;
            }

            if (update.TargetDate != ticketConcern.TargetDate && ticketConcern.UserId is not null)
            {
                update.TargetDate = ticketConcern.TargetDate;
                hasChanged = true;
            }

            if(hasChanged)
            {
                update.ModifiedBy = ticketConcern.ModifiedBy;
                update.UpdatedAt = DateTime.Now;

            }

        }

        public async Task ApproveOpenTicket(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {
            await context.TicketConcerns
                 .Where(x => x.Id == ticketConcern.Id)
                 .ExecuteUpdateAsync(approve => approve
                 .SetProperty(a => a.IsApprove, a => true)
                 .SetProperty(a => a.ApprovedBy, a => ticketConcern.ApprovedBy)
                 .SetProperty(a => a.ConcernStatus, a => ticketConcern.ConcernStatus)
                 .SetProperty(a => a.ApprovedAt , a => ticketConcern.ApprovedAt)
                 .SetProperty(a => a.IsAssigned, a => ticketConcern.IsAssigned));

        }

        public async Task CancelledTicketConcern(int? id)
        {
            await context.TicketConcerns
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(cancel => cancel
                .SetProperty(c => c.IsActive, c => false));
        }

        public async Task CancelledRequestConcern(int? id)
        {
            await context.RequestConcerns
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(cancel => cancel
                .SetProperty(c => c.IsActive, c => false));
        }

        public async Task CancelledTicketAttachment(int? id)
        {
              await context.TicketAttachments
                .Where(x => x.TicketConcernId == id)
                .ExecuteUpdateAsync(cancel => cancel
                .SetProperty(u => u.IsActive, u => false));
        }

        public async Task<List<TicketHistory>> TicketHistoryByForApprovalList(int? id)
        {
            return await context.TicketHistories
                .Where(x => x.TicketConcernId == id && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                .ToListAsync();
        }

        public async Task UpdateTicketHistory(TicketHistory ticketHistory, CancellationToken cancellationToken)
        {
            var update = await context.TicketHistories
                .FirstOrDefaultAsync(x => x.Id == ticketHistory.Id);

            bool isChange = true;

            if(update.TransactedBy != ticketHistory.TransactedBy && ticketHistory.TransactedBy != null)
            {
                update.TransactedBy = ticketHistory.TransactedBy;
                isChange = true;
            }

            if (update.Request != ticketHistory.Request && ticketHistory.Request != null)
            {
                update.Request = ticketHistory.Request;
                isChange = true;
            }

            if (update.Status != ticketHistory.Status && ticketHistory.Status != null)
            {
                update.Status = ticketHistory.Status;
                isChange = true;
            }

            if(isChange)
            {
                update.TransactionDate = DateTime.Now;
                update.IsApprove = true;

            }

        }

        public async Task<TicketHistory> TicketHistoryMinByForApproval(int? id)
        {
            var historyList = await context.TicketHistories
                  .Where(x => x.TicketConcernId == id && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                  .FirstOrDefaultAsync();

            return historyList;

        }

        public async Task RemoveTicketHistory(int? id)
        {
            await context.TicketHistories
                .Where(x => x.TicketConcernId == id)
                .Where(x => x.IsApprove == null && x.Request.Contains(TicketingConString.Approval)
                     || x.Request.Contains(TicketingConString.NotConfirm))
                .ExecuteDeleteAsync();
        }

        public async Task<TicketConcern> TicketConcernByRequest(int? id)
        {
          return await context.TicketConcerns
                .FirstOrDefaultAsync(x => x.RequestConcernId == id);
        }
    }
}
