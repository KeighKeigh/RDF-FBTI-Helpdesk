using CloudinaryDotNet;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
//using MakeItSimple.WebApi.Hubs;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult.GetForClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern.AddRequestConcernCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest
{
    public partial class AddRequestConcern
    {
        public class returnRequest
        {
            public string TicketConcernId { get; set; }
            public string RequestorName { get; set; }
            public string RequestorDepartment { get; set; }
            public string ChannelName { get; set; }
            public string Concern { get; set; }
        }
        public class Handler : IRequestHandler<AddRequestConcernCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            //private readonly IHubCaller hubCaller;
            private readonly MisDbContext context;

            public Handler(IUnitOfWork unitOfWork, MisDbContext context)
            {
                this.unitOfWork = unitOfWork;
                this.context = context;

            }

            public async Task<Result> Handle(AddRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var oneTicket = 0;
                var requestorData = new returnRequest();
                if (command.AssignTo == null)
                {
                    var ticketConcernId = new int(); //????kkkk
                    var requestConcernId = new int();


                    //var userDetails = await unitOfWork.User
                    //    .UserExist(command.Added_By);

                    var userIdExist = await unitOfWork.User
                        .UserExist(command.UserId);


                    if (userIdExist is null)
                        return Result.Failure(UserError.UserNotExist());

                    //var channelExist = await unitOfWork.Channel
                    //  .ChannelExist(command.ChannelId);

                    //if (channelExist is null)
                    //    return Result.Failure(TicketRequestError.ChannelNotExist());



                    var userDepartment = await unitOfWork.User.UserDepartment(userIdExist.DepartmentId);

                    var existingBackJob = await context.RequestConcerns.Where(x => x.Id == command.BackJobId).Select(x => x.BackJobId).FirstOrDefaultAsync();
                    //KK
                    foreach (var concern in command.ListOfConcerns)
                    {

                        foreach (var category in command.AddRequestTicketCategories)
                        {
                            var ticketCategoryExist = await unitOfWork.Category
                              .CategoryExist(category.CategoryId);

                            //if (ticketCategoryExist is null)
                            //    return Result.Failure(TicketRequestError.CategoryNotExist());
                        }

                        foreach (var subCategory in command.AddRequestTicketSubCategories)
                        {
                            var ticketSubCategoryExist = await unitOfWork.SubCategory
                                .SubCategoryExist(subCategory.SubCategoryId);

                            //if (ticketSubCategoryExist is null)
                            //    return Result.Failure(TicketRequestError.SubCategoryNotExist());
                        }
                        var ticketCategoryList = new List<int?>();
                        var ticketSubCategoryList = new List<int?>();

                        
                        
                        var addRequestConcern = new RequestConcern
                        {
                            UserId = command.UserId,
                            Concern = concern.Concern,
                            AddedBy = command.Added_By,
                            ConcernStatus = TicketingConString.ForApprovalTicket,
                            CompanyId = userIdExist.CompanyId,
                            BusinessUnitId = userIdExist.BusinessUnitId,
                            DepartmentId = userIdExist.DepartmentId,
                            ReqSubUnitId = userIdExist.SubUnitId.ToString() == "" ? null : userIdExist.SubUnitId,
                            ReqUnitId = userIdExist.UnitId.ToString() == "" ? null : userIdExist.UnitId,
                            LocationId = userIdExist.LocationId,
                            DateNeeded = command.DateNeeded,
                            ChannelId = command.ChannelId == 0 ? null : command.ChannelId,
                            Notes = command.Notes,
                            IsDone = false,
                            ContactNumber = command.Contact_Number,
                            RequestType = command.Request_Type,
                            BackJobId =  existingBackJob != null ? existingBackJob : command.BackJobId,
                            Severity = command.Severity,
                            TargetDate = command.TargetDate.ToString() == "" ? null : command.TargetDate,
                            AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,
                            ServiceProviderId = command.ServiceProviderId,
                            OneChargingCode = userIdExist.OneChargingCode,
                            OneChargingName = userIdExist.OneChargingName,
                            //TicketCategories = 

                        };

                        await unitOfWork.RequestTicket.CreateRequestConcern(addRequestConcern, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        requestConcernId = addRequestConcern.Id;


                        //kk

                        var addTicketConcern = new TicketConcern
                        {
                            RequestConcernId = requestConcernId,
                            RequestorBy = command.UserId,
                            IsApprove = false,
                            AddedBy = command.Added_By,
                            ConcernStatus = TicketingConString.ForApprovalTicket,
                            IsAssigned = false,
                            AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,



                        };

                        await unitOfWork.RequestTicket.CreateTicketConcern(addTicketConcern, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);


                        if (oneTicket == 0)
                        {
                            var channelName = await context.Channels.Where(x => x.Id == addRequestConcern.ChannelId).Select(x => x.ChannelName).FirstOrDefaultAsync();
                            requestorData = new returnRequest
                            {
                                TicketConcernId = addTicketConcern.Id.ToString(),
                                RequestorName = addTicketConcern.RequestorByUser.Fullname,
                                RequestorDepartment = userDepartment.department_name,
                                ChannelName = channelName,
                                Concern = addRequestConcern.Concern
                            };
                            oneTicket = 1;
                        }

                        ticketConcernId = addTicketConcern.Id;

                        var addTicketHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernId,
                            TransactedBy = command.Added_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.Request,
                            Status = $"{TicketingConString.RequestCreated} {userIdExist.Fullname}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                        //kk


                        var addNewTicketTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"New request concern number {requestConcernId} has received",
                            AddedBy = command.Added_By.Value,
                            Created_At = DateTime.Now,
                            Modules = PathConString.ReceiverConcerns,
                            PathId = requestConcernId

                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);



                        foreach (var category in command.AddRequestTicketCategories)
                        {
                            var ticketCategoryExist = await unitOfWork.RequestTicket
                                .TicketCategoryExist(category.CategoryId, command.RequestConcernId);
                            if (category.CategoryId != null)
                            {
                                if (ticketCategoryExist is null)
                                {
                                    var addTicketCategory = new TicketCategory
                                    {
                                        RequestConcernId = requestConcernId,
                                        CategoryId = category.CategoryId,
                                    };
                                    await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);
                                }
                            }
                        }

                        foreach (var subCategory in command.AddRequestTicketSubCategories)
                        {
                            var ticketSubCategoryExist = await unitOfWork.RequestTicket
                                .TicketSubCategoryExist(subCategory.SubCategoryId, command.RequestConcernId);
                            if (subCategory.SubCategoryId != null)
                            {
                                if (ticketSubCategoryExist is null)
                                {
                                    var addTicketSubCategory = new TicketSubCategory
                                    {
                                        RequestConcernId = requestConcernId,
                                        SubCategoryId = subCategory.SubCategoryId,
                                    };
                                    await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                                }
                            }
                        }

                        if (ticketCategoryList.Any())
                            await unitOfWork.RequestTicket.RemoveTicketCategory(requestConcernId, ticketCategoryList, cancellationToken);

                        if (ticketSubCategoryList.Any())
                            await unitOfWork.RequestTicket.RemoveTicketSubCategory(requestConcernId, ticketSubCategoryList, cancellationToken);


                        if (!Directory.Exists(TicketingConString.AttachmentPath))
                        {
                            Directory.CreateDirectory(TicketingConString.AttachmentPath);
                        }

                        if (command.RequestAttachmentsFiles.Count(x => x.Attachment != null) > 0)
                        {
                            foreach (var attachments in command.RequestAttachmentsFiles.Where(a => a.Attachment.Length > 0))
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
                                    var updateTicketAttachment = new TicketAttachment
                                    {
                                        Attachment = filePath,
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        UpdatedAt = DateTime.Now,

                                    };

                                    await unitOfWork.RequestTicket.UpdateTicketAttachment(updateTicketAttachment, cancellationToken);
                                }
                                else
                                {
                                    var addAttachment = new TicketAttachment
                                    {
                                        TicketConcernId = ticketConcernId, //kk
                                        
                                        Attachment = filePath,
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        AddedBy = command.Added_By,
                                    };
                                    
                                    await unitOfWork.RequestTicket.CreateTicketAttachment(addAttachment, cancellationToken);

                                }
                                
                                await using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await attachments.Attachment.CopyToAsync(stream);
                                }
                            }

                        }


                    }

                    

                }


                


                else if(command.AssignTo != null)
                {
                    var dateToday = DateTime.Now;
                    var approvedDate = DateTime.Today.AddDays(7);
                    var requestConcernId = new int();


                    //foreach (var category in command.AddRequestTicketCategories)
                    //{
                    //    var ticketCategoryExist = await unitOfWork.Category
                    //      .CategoryExist(category.CategoryId);

                    //    //if (ticketCategoryExist is null)
                    //    //    return Result.Failure(TicketRequestError.CategoryNotExist());
                    //}

                    //foreach (var subCategory in command.AddRequestTicketSubCategories)
                    //{
                    //    var ticketSubCategoryExist = await unitOfWork.SubCategory
                    //        .SubCategoryExist(subCategory.SubCategoryId);

                    //    //if (ticketSubCategoryExist is null)
                    //    //    return Result.Failure(TicketRequestError.SubCategoryNotExist());
                    //}


                    //var userDetails = await unitOfWork.User
                    //    .UserExist(command.Modified_By);

                    var handlerDetails = await unitOfWork.User
                        .UserExist(command.AssignTo);

                    var receiverPermissionList = await unitOfWork.UserRole
                        .UserRoleByPermission(TicketingConString.Requestor);

                    var issueHandlerPermissionList = await unitOfWork.UserRole
                        .UserRoleByPermission(TicketingConString.Requestor);

                    var requestorPermissionList = await unitOfWork.UserRole
                        .UserRoleByPermission(TicketingConString.Requestor);

                    switch (await unitOfWork.User.UserExist(command.UserId))
                    {
                        case null:
                            return Result.Failure<int?>(TicketRequestError.UserNotExist());
                    }

                    switch (await unitOfWork.Channel.ChannelExist(command.ChannelId))
                    {
                        case null:
                            return Result.Failure<int?>(TicketRequestError.ChannelNotExist());
                    }

                    //foreach (var category in command.AddRequestTicketCategories)
                    //{
                    //    var ticketCategoryExist = await unitOfWork.Category
                    //      .CategoryExist(category.CategoryId);


                    //}

                    //foreach (var subCategory in command.AddRequestTicketSubCategories)
                    //{
                    //    var ticketSubCategoryExist = await unitOfWork.SubCategory
                    //        .SubCategoryExist(subCategory.SubCategoryId);

                    //}

                    if (dateToday.Date > command.TargetDate)
                        return Result.Failure<int?>(TicketRequestError.DateTimeInvalid());

                    foreach (var concern in command.ListOfConcerns)
                    {
                        var ticketCategoryList = new List<int?>();
                        var ticketSubCategoryList = new List<int?>();

                        var ticketConcernExist = await unitOfWork.RequestTicket
                               .TicketConcernExist(command.TicketConcernId);

                        //if (ticketConcernExist is not null)
                        //{
                        //    if (ticketConcernExist.IsActive is false)
                        //        return Result.Failure<int?>(TicketRequestError.TicketAlreadyCancel());

                        //    var assignTicket = new TicketConcern
                        //    {
                        //        Id = ticketConcernExist.Id,
                        //        UserId = command.UserId,
                        //        TargetDate = command.TargetDate,
                        //        ModifiedBy = command.Modified_By,


                        //    };

                        //    await unitOfWork.RequestTicket.UpdateTicketConcern(assignTicket, cancellationToken);



                        //    //var addNewTicketTransactionNotification = new TicketTransactionNotification
                        //    //{

                        //    //    Message = $"Ticket number {ticketConcernExist.Id} has been assigned",
                        //    //    AddedBy = command.Added_By.Value,
                        //    //    Created_At = DateTime.Now,
                        //    //    ReceiveBy = command.UserId.Value,
                        //    //    Modules = PathConString.IssueHandlerConcerns,
                        //    //    Modules_Parameter = PathConString.OpenTicket,
                        //    //    PathId = ticketConcernExist.Id,


                        //    //};

                        //    //await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                        //    //var addNewTicketTransactionOngoing = new TicketTransactionNotification
                        //    //{

                        //    //    Message = $"Ticket number {ticketConcernExist.RequestConcernId} is now ongoing",
                        //    //    AddedBy = command.Added_By.Value,
                        //    //    Created_At = DateTime.Now,
                        //    //    ReceiveBy = ticketConcernExist.RequestConcern.UserId.Value,
                        //    //    Modules = PathConString.ConcernTickets,
                        //    //    Modules_Parameter = PathConString.Ongoing,
                        //    //    PathId = ticketConcernExist.RequestConcernId.Value,

                        //    //};

                        //    //await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                        //    requestConcernId = ticketConcernExist.Id;

                        //}


                        var requestorDetails = await unitOfWork.User
                             .UserExist(command.UserId);

                        var addRequestConcern = new RequestConcern
                        {
                            UserId = command.UserId,
                            Concern = concern.Concern,
                            AddedBy = command.Added_By,
                            ConcernStatus = command.TargetDate.Value.Date <= approvedDate.Date ? TicketingConString.OnGoing : TicketingConString.ForApprovalTicket,
                            CompanyId = requestorDetails.CompanyId,
                            BusinessUnitId = requestorDetails.BusinessUnitId,
                            DepartmentId = requestorDetails.DepartmentId,
                            UnitId = requestorDetails.UnitId.ToString() == "" ? null : requestorDetails.UnitId,
                            SubUnitId = requestorDetails.SubUnitId.ToString() == "" ? null : requestorDetails.SubUnitId,
                            LocationId = requestorDetails.LocationId,
                            ChannelId = command.ChannelId,
                            DateNeeded = command.DateNeeded,
                            ContactNumber = command.Contact_Number,
                            RequestType = command.Request_Type,
                            BackJobId = command.BackJobId,
                            Notes = command.Notes,
                            IsDone = false,
                            Severity = command.Severity,
                            ServiceProviderId = command.ServiceProviderId,
                            TargetDate = command.TargetDate.ToString() == "" ? null : command.TargetDate,
                            AssignTo = command.AssignTo,
                            OneChargingCode = requestorDetails.OneChargingCode,
                            OneChargingName = requestorDetails.OneChargingName,

                        };

                        await unitOfWork.RequestTicket.CreateRequestConcern(addRequestConcern, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        requestConcernId = addRequestConcern.Id;

                        var handlerIds = await context.ApproverUsers.Where(x => x.UserId == addRequestConcern.AssignTo).FirstOrDefaultAsync();
                        var addTicketConcern = new TicketConcern
                        {
                            RequestConcernId = requestConcernId,
                            TargetDate = command.TargetDate,
                            UserId = command.UserId,
                            RequestorBy = command.UserId,
                            IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                            AddedBy = command.Added_By,
                            ConcernStatus = command.TargetDate.Value.Date <= approvedDate.Date ? TicketingConString.OnGoing : TicketingConString.ForApprovalTicket,
                            IsAssigned = true,
                            AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,
                            IsDateApproved = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                            DateApprovedAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday : null,
                            ApprovedDateBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.ApproverId : null,
                            ApprovedAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday.Date : null,
                            ApprovedBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.ApproverId : null,

                        };

                        await unitOfWork.RequestTicket.CreateTicketConcern(addTicketConcern, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        //var cache = await cacheService.GetOpenTickets();

                        //var chechcache = cache.Where(x => x.UserId == addTicketConcern.UserId).ToList();

                        ////if (!chechcache.Any())
                        ////{
                        ////    cache.Add(addTicketConcern);
                        ////}

                        ////var filterCache = cache.Select(x => x.UserId == command.UserId.Value);

                        //await hubCaller.SendNotificationAsync(command.UserId.Value, "NewPendingTicket", chechcache);


                        ticketConcernExist = addTicketConcern;

                    //    var approverList = await unitOfWork.RequestTicket
                    //.ApproverBySubUnitList(addTicketConcern.User.SubUnitId);

                    //    if (!approverList.Any())
                    //        return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                    //    if (!approverList.Any())
                    //        return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                    //    var approverUser = approverList
                    //.First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));


                        //var handlerId = await context.Approvers.Where(x => x.SubUnitId == addTicketConcern.User.SubUnitId).FirstOrDefaultAsync();
                        var addNewDateApproveConcern = new ApproverDate
                        {
                            TicketConcernId = addTicketConcern.Id,
                            IsApproved = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                            TicketApprover = handlerIds.ApproverId,
                            AddedBy = command.Added_By,
                            Notes = command.Notes,
                            ApprovedDateBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.ApproverId : null,
                            ApprovedDateAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday : null,



                        };

                        await unitOfWork.RequestTicket.ApproveDateTicket(addNewDateApproveConcern, cancellationToken);

                        await unitOfWork.SaveChangesAsync(cancellationToken);


                        //foreach (var approver in approverList)
                        //{
                        //    var addNewApprover = new ApproverTicketing
                        //    {
                        //        TicketConcernId = ticketConcernExist.Id,
                        //        ApproverDateId = addNewDateApproveConcern.Id,
                        //        UserId = approver.UserId,
                        //        ApproverLevel = approver.ApproverLevel,
                        //        AddedBy = command.Added_By,
                        //        CreatedAt = DateTime.Now,
                        //        Status = TicketingConString.ApprovalDate,
                        //        IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ?  true : null,
                        //    };

                        //    await unitOfWork.RequestTicket.CreateApproval(addNewApprover, cancellationToken);

                        //}

                        
                            var addNewApprover = new ApproverTicketing
                            {
                                TicketConcernId = ticketConcernExist.Id,
                                ApproverDateId = addNewDateApproveConcern.Id,
                                UserId = handlerIds.ApproverId,
                                AddedBy = command.Added_By,
                                CreatedAt = DateTime.Now,
                                Status = TicketingConString.ApprovalDate,
                                IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ? true : null,
                            };

                            await unitOfWork.RequestTicket.CreateApproval(addNewApprover, cancellationToken);

                        

                        //var addRequestTicketHistory = new TicketHistory
                        //{
                        //    TicketConcernId = ticketConcernExist.Id,
                        //    TransactedBy = command.Added_By,
                        //    TransactionDate = DateTime.Now,
                        //    Request = TicketingConString.Ticketing,
                        //    Status = $"{TicketingConString.TicketCreated} {userDetails.Fullname}"
                        //};

                        //await unitOfWork.RequestTicket.CreateTicketHistory(addRequestTicketHistory, cancellationToken);

                        var assignedTicketHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TransactedBy = command.Added_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ConcernAssign,
                            Status = $"{TicketingConString.RequestAssign} {handlerDetails.Fullname}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(assignedTicketHistory, cancellationToken);



                        if (command.TargetDate.Value.Date > approvedDate.Date)
                        {
                            var approveTicketDateHistory = new TicketHistory
                            {
                                TicketConcernId = ticketConcernExist.Id,
                                TransactedBy = handlerIds.UserId,
                                TransactionDate = DateTime.Now,
                                Request = TicketingConString.ForApprovalTicket,
                                Status = $"{TicketingConString.ForApprovalDate}"
                            };

                            await unitOfWork.RequestTicket.CreateTicketHistory(approveTicketDateHistory, cancellationToken);

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {ticketConcernExist.Id} Target Date is now being approved!",
                                AddedBy = command.Added_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = command.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.ForApproval,
                                PathId = ticketConcernExist.Id,

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);
                        }
                        else
                        {
                            //var addNewTicketTransactionNotification = new TicketTransactionNotification
                            //{

                            //    Message = $"Ticket number {ticketConcernExist.Id}, Target Date is approved",
                            //    AddedBy = ticketConcernExist.UserId,
                            //    Created_At = DateTime.Now,
                            //    ReceiveBy = addRequestConcern.UserId.Value,
                            //    Modules = PathConString.ConcernTickets,
                            //    Modules_Parameter = PathConString.Ongoing,
                            //    PathId = ticketConcernExist.Id

                            //};

                            //await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                            //var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            //{

                            //    Message = $"Ticket number {ticketConcernExist.Id}, Target Date is approved",
                            //    AddedBy = ticketConcernExist.UserId,
                            //    Created_At = DateTime.Now,
                            //    ReceiveBy = ticketConcernExist.UserId.Value,
                            //    Modules = PathConString.IssueHandlerConcerns,
                            //    Modules_Parameter = PathConString.Ongoing,
                            //    PathId = ticketConcernExist.Id

                            //};

                            //await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);

                            var addNewTicketTransactionOngoing = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {ticketConcernExist.Id} is now ongoing",
                                AddedBy = ticketConcernExist.UserId,
                                Created_At = DateTime.Now,
                                ReceiveBy = addRequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = ticketConcernExist.Id,


                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                        }

                        if (command.AddRequestTicketCategories != null)
                        {

                            foreach (var category in command.AddRequestTicketCategories)
                            {
                                var ticketCategoryExist = await unitOfWork.RequestTicket
                                    .TicketCategoryExist(category.CategoryId);

                                if (category.CategoryId != null)
                                {
                                    ticketCategoryList.Add(category.CategoryId.Value);

                                    var addTicketCategory = new TicketCategory
                                    {
                                        RequestConcernId = requestConcernId,
                                        CategoryId = category.CategoryId,

                                    };

                                    await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);
                                }

                            }

                            foreach (var subCategory in command.AddRequestTicketSubCategories)
                            {
                                var ticketSubCategoryExist = await unitOfWork.RequestTicket
                                    .TicketSubCategoryExist(subCategory.SubCategoryId);


                                if (subCategory.SubCategoryId != null)
                                {
                                    ticketSubCategoryList.Add(subCategory.SubCategoryId.Value);

                                    var addTicketSubCategory = new TicketSubCategory
                                    {
                                        RequestConcernId = requestConcernId,
                                        SubCategoryId = subCategory.SubCategoryId,

                                    };

                                    await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                                }
                            }
                        }

                        if (ticketCategoryList.Any())
                            await unitOfWork.RequestTicket.RemoveTicketCategory(requestConcernId, ticketCategoryList, cancellationToken);

                        if (ticketSubCategoryList.Any())
                            await unitOfWork.RequestTicket.RemoveTicketSubCategory(requestConcernId, ticketSubCategoryList, cancellationToken);

                        if (!Directory.Exists(TicketingConString.AttachmentPath))
                        {
                            Directory.CreateDirectory(TicketingConString.AttachmentPath);
                        }

                        if (command.RequestAttachmentsFiles.Count(x => x.Attachment != null) > 0)
                        {
                            foreach (var attachments in command.RequestAttachmentsFiles.Where(a => a.Attachment.Length > 0))
                            {

                                if (attachments.Attachment.Length > 10 * 1024 * 1024)
                                {
                                    return Result.Failure<int?>(TicketRequestError.InvalidAttachmentSize());
                                }

                                var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                                var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                                if (extension == null || !allowedFileTypes.Contains(extension))
                                {
                                    return Result.Failure<int?>(TicketRequestError.InvalidAttachmentType());
                                }

                                var fileName = $"{Guid.NewGuid()}{extension}";
                                var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                                var ticketAttachment = await unitOfWork.RequestTicket
                                    .TicketAttachmentExist(attachments.TicketAttachmentId);

                                if (ticketAttachment != null)
                                {
                                    var updateTicketAttachment = new TicketAttachment
                                    {
                                        Attachment = filePath,
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        UpdatedAt = DateTime.Now,

                                    };

                                    await unitOfWork.RequestTicket.UpdateTicketAttachment(updateTicketAttachment, cancellationToken);
                                }
                                else
                                {
                                    var addAttachment = new TicketAttachment
                                    {
                                        TicketConcernId = ticketConcernExist.Id,
                                        ApproverDateId = addNewDateApproveConcern.Id,
                                        Attachment = filePath,
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        AddedBy = command.Added_By,
                                    };

                                    
                                    await unitOfWork.RequestTicket.CreateTicketAttachment(addAttachment, cancellationToken);

                                }

                                await using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await attachments.Attachment.CopyToAsync(stream);
                                }
                            }

                        }
                    }
                    
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(requestorData);
            }

        }
    }
}
