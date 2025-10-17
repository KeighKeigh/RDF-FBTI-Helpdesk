using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.ClosedTicketConcern.AddClosingTicket
{

    public class Handler : IRequestHandler<AddNewClosingTicketCommand, Result>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MisDbContext _context;

        public Handler(IUnitOfWork unitOfWork, MisDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this._context = context;
        }

        public async Task<Result> Handle(AddNewClosingTicketCommand command, CancellationToken cancellationToken)
        {
            var ticketCategoryList = new List<int?>();
            var ticketSubCategoryList = new List<int?>();
            var ticketTechnicianList = new List<int>();

            //var userDetails = await unitOfWork.User
            //    .UserExist(command.Added_By);

            var ticketConcernExist = await unitOfWork.RequestTicket
                .TicketConcernExist(command.TicketConcernId);

            if (ticketConcernExist is null)
                return Result.Failure(ClosingTicketError.TicketConcernIdNotExist());

            var closingTicketExist = await unitOfWork.ClosingTicket
               .ClosingTicketExist(command.ClosingTicketId);

            if (closingTicketExist is not null)
            {
                var approver = await unitOfWork.ClosingTicket
                    .ApproverThatNotNullByClosingTicket(closingTicketExist.Id);

                if (approver is not null)
                    return Result.Failure(TicketRequestError.TicketAlreadyApproved());

                var updateClosingTicket = new ClosingTicket
                {
                    Id = closingTicketExist.Id,
                    Resolution = command.Resolution,
                    Notes = command.Notes,
                    CategoryConcernId = command.CategoryConcernId,
                    CategoryConcernName = command.CategoryConcern,

                };

                await unitOfWork.ClosingTicket.UpdateClosingTicket(updateClosingTicket,cancellationToken);

            }
            else
            {
                //var approverList = await unitOfWork.ClosingTicket
                //    .ApproverBySubUnitList(ticketConcernExist.User.SubUnitId);

                //if (!approverList.Any())
                //    return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                foreach (var category in command.ClosingTicketCategories)
                {
                    var ticketCategoryExist = await unitOfWork.Category
                      .CategoryExist(category.CategoryId);

                    if (ticketCategoryExist is null)
                        return Result.Failure(TicketRequestError.CategoryNotExist());

                }

                foreach (var subCategory in command.ClosingSubTicketCategories)
                {
                    var ticketSubCategoryExist = await unitOfWork.SubCategory
                        .SubCategoryExist(subCategory.SubCategoryId);

                    if (ticketSubCategoryExist is null)
                        return Result.Failure(TicketRequestError.SubCategoryNotExist());
                }

                //var approverUser = approverList
                //    .First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                var approver = await _context.ApproverUsers.Where(x => x.UserId == ticketConcernExist.AssignTo).FirstOrDefaultAsync();

                var addNewClosingConcern = new ClosingTicket
                {
                    TicketConcernId = ticketConcernExist.Id,
                    Resolution = command.Resolution,
                    IsClosing = false,
                    TicketApprover = approver.ApproverId,
                    AddedBy = command.Added_By,
                    Notes = command.Notes == null ? null : command.Notes,
                    CategoryConcernId = command.CategoryConcernId,
                    CategoryConcernName = command.CategoryConcern,
                    ForClosingAt = DateTime.Now,
                };

                await unitOfWork.ClosingTicket.CreateClosingTicket(addNewClosingConcern,cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                closingTicketExist = addNewClosingConcern;

                //var forClosingDate = new TicketConcern
                //{
                //    Id = ticketConcernExist.Id,
                //    ForClosingDate = DateTime.Now,
                //};

                //await unitOfWork.ClosingTicket.AddForClosingDate(forClosingDate, cancellationToken);
                //foreach (var approver in approverList)
                //{
                var addNewApprover = new ApproverTicketing
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        ClosingTicketId = closingTicketExist.Id,
                        UserId = approver.ApproverId,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,
                        Status = TicketingConString.CloseTicket,
                    };

                    await unitOfWork.ClosingTicket.CreateApproval(addNewApprover, cancellationToken);

                //}

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcernExist.Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ForClosing,
                    Status = TicketingConString.CloseRequest
                };

                await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                //foreach (var approver in approverList)
                //{
                    //var approverLevel = approver.ApproverLevel == 1 ? $"{approver.ApproverLevel}st"
                    //       : approver.ApproverLevel == 2 ? $"{approver.ApproverLevel}nd"
                    //       : approver.ApproverLevel == 3 ? $"{approver.ApproverLevel}rd"
                    //       : $"{approver.ApproverLevel}th";

                    var addApproverHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = approver.ApproverId,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Approval,
                        Status = $"{TicketingConString.CloseForApproval}",
                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(addApproverHistory, cancellationToken);

                //}

                //var businessUnitList = await unitOfWork.BusinessUnit
                //         .BusinessUnitExist(ticketConcernExist.User.BusinessUnitId);

                //var receiverList = await unitOfWork.Receiver
                //    .ReceiverExistByBusinessUnitId(businessUnitList.Id);

                var addForConfirmationHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcernExist.Id,
                    TransactedBy = closingTicketExist.TicketConcern.RequestorBy,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.NotConfirm,
                    Status = $"{TicketingConString.CloseForConfirmation} {ticketConcernExist.RequestorByUser.Fullname}",
                };

                await unitOfWork.RequestTicket.CreateTicketHistory(addForConfirmationHistory, cancellationToken);


                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcernExist.Id} is pending for closing approval",
                    AddedBy = command.Added_By.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = closingTicketExist.TicketApprover.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForClosingTicket,
                    PathId = ticketConcernExist.Id

                };

                await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);
                await unitOfWork.ClosingTicket.ForClosingTicket(ticketConcernExist.Id);

            }

            if (command.AddClosingTicketTechnicians.Any() && command.AddClosingTicketTechnicians.First().Technician_By is not null)
            {

                foreach (var technician in command.AddClosingTicketTechnicians)
                {
                    var ticketTechnicianExist = await unitOfWork.ClosingTicket
                        .TicketTechnicianExist(technician.TicketTechnicianId);

                    if (ticketTechnicianExist is not null)
                    {
                        ticketTechnicianList.Add(ticketTechnicianExist.Id);

                    }
                    else if(ticketTechnicianExist is null && technician.Technician_By is not null)
                    {
                        var addTicketTechnician = new TicketTechnician
                        {
                            ClosingTicketId = closingTicketExist.Id,
                            TechnicianBy = technician.Technician_By,

                        };

                        await unitOfWork.ClosingTicket.CreateTicketTechnician(addTicketTechnician, cancellationToken);

                    }
                }

            }

            //foreach (var category in command.ClosingTicketCategories)
            //{
            //    var ticketCategoryExist = await unitOfWork.RequestTicket
            //        .TicketCategoryExist(category.CategoryId, command.RequestConcernId);

            //    if (ticketCategoryExist is not null)
            //    {
            //        ticketCategoryList.Add(category.CategoryId.Value);

            //    }
            //    else
            //    {
            //        var addTicketCategory = new TicketCategory
            //        {
            //            RequestConcernId = ticketConcernExist.RequestConcernId.Value,
            //            CategoryId = category.CategoryId.Value,

            //        };

            //        await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory,cancellationToken);
            //    }

            //}

            //foreach (var subCategory in command.ClosingSubTicketCategories)
            //{
            //    var ticketSubCategoryExist = await unitOfWork.RequestTicket
            //        .TicketSubCategoryExist(subCategory.SubCategoryId, command.RequestConcernId);

            //    if (ticketSubCategoryExist is not null)
            //    {
            //        ticketSubCategoryList.Add(subCategory.SubCategoryId.Value); 
            //    }
            //    else
            //    {
            //        var addTicketSubCategory = new TicketSubCategory
            //        {
            //            RequestConcernId = ticketConcernExist.RequestConcernId.Value,
            //            SubCategoryId = subCategory.SubCategoryId.Value,

            //        };

            //        await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory,cancellationToken);
            //    }

            //}

            //if (ticketCategoryList.Any())
            //    await unitOfWork.RequestTicket.RemoveTicketCategory(ticketConcernExist.RequestConcernId.Value, ticketCategoryList, cancellationToken);

            //if (ticketSubCategoryList.Any())
            //    await unitOfWork.RequestTicket.RemoveTicketSubCategory(ticketConcernExist.RequestConcernId.Value, ticketSubCategoryList, cancellationToken);


            foreach (var category in command.ClosingTicketCategories)
            {
                var ticketCategoryExist = await unitOfWork.RequestTicket
                    .TicketCategoryExist(category.CategoryId, ticketConcernExist.RequestConcernId.Value);
                if (category.CategoryId != null)
                {
                    if (ticketCategoryExist is null)
                    {
                        ticketCategoryList.Add(category.CategoryId.Value);
                        // Add new category
                        var addTicketCategory = new TicketCategory
                        {
                            RequestConcernId = ticketConcernExist.RequestConcernId.Value,
                            CategoryId = category.CategoryId,
                        };
                        await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);
                    }
                    else
                    {
                        ticketCategoryList.Add(category.CategoryId.Value);
                    }
                }
            }

            // Process subcategories - add new ones
            foreach (var subCategory in command.ClosingSubTicketCategories)
            {
                var ticketSubCategoryExist = await unitOfWork.RequestTicket
                    .TicketSubCategoryExist(subCategory.SubCategoryId, ticketConcernExist.RequestConcernId.Value);
                if (subCategory.SubCategoryId != null)
                {
                    if (ticketSubCategoryExist is null)
                    {
                        ticketSubCategoryList.Add(subCategory.SubCategoryId.Value);
                        // Add new subcategory
                        var addTicketSubCategory = new TicketSubCategory
                        {
                            RequestConcernId = ticketConcernExist.RequestConcernId.Value,
                            SubCategoryId = subCategory.SubCategoryId,
                        };
                        await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                    }
                    else
                    { 
                        ticketSubCategoryList.Add(subCategory.SubCategoryId.Value); 
                    }
                }
            }

            if (ticketCategoryList.Any())
                await unitOfWork.RequestTicket.RemoveTicketCategory(ticketConcernExist.RequestConcernId.Value, ticketCategoryList, cancellationToken);

            if (ticketSubCategoryList.Any())
                await unitOfWork.RequestTicket.RemoveTicketSubCategory(ticketConcernExist.RequestConcernId.Value, ticketSubCategoryList, cancellationToken);

            if (ticketTechnicianList.Any())
                await unitOfWork.ClosingTicket.RemoveTicketTechnician(ticketConcernExist.RequestConcernId.Value, ticketTechnicianList, cancellationToken);

            if (!Directory.Exists(TicketingConString.AttachmentPath))
            {
                Directory.CreateDirectory(TicketingConString.AttachmentPath);
            }

            if (command.AddClosingAttachments.Count(x => x.Attachment != null) > 0)
            {
                foreach (var attachments in command.AddClosingAttachments.Where(a => a.Attachment.Length > 0))
                {

                    if (attachments.Attachment.Length > 10 * 1024 * 1024)
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                    }

                    var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                    var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                    if (extension == null || !allowedFileTypes.Contains(extension))
                    {
                        return Result.Failure(TicketRequestError.InvalidAttachmentType());
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                    var ticketAttachment = await unitOfWork.RequestTicket
                        .TicketAttachmentExist(attachments.TicketAttachmentId);

                    if (ticketAttachment != null)
                    {
                        ticketAttachment.Attachment = filePath;
                        ticketAttachment.FileName = attachments.Attachment.FileName;
                        ticketAttachment.FileSize = attachments.Attachment.Length;
                        ticketAttachment.UpdatedAt = DateTime.Now;

                    }
                    else
                    {
                        var addAttachment = new TicketAttachment
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            ClosingTicketId = closingTicketExist.Id,
                            Attachment = filePath,
                            FileName = attachments.Attachment.FileName,
                            FileSize = attachments.Attachment.Length,
                            AddedBy = command.Added_By,
                        };

                        await unitOfWork.RequestTicket.CreateTicketAttachment(addAttachment,cancellationToken);

                    }

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachments.Attachment.CopyToAsync(stream);
                    }
                }

            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }

    }

}
