using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OpenExport
{
    public partial class OpenTicketExport
    {

        public class Handler : IRequestHandler<OpenTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(OpenTicketExportCommand request, CancellationToken cancellationToken)
            {
                var openTicket = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery()
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true && x.OnHold != true && x.IsTransfer != true)
                    //.Where(x => x.TargetDate.Value.Date >= request.Date_From.Value.Date && x.TargetDate.Value.Date <= request.Date_To.Value.Date)
                    .Select(t => new OpenTicketExportResult
                    {
                        UserId = t.UserId,
                        UnitId = t.User.UnitId,
                        TicketConcernId = t.Id,
                        Concern_Description = t.RequestConcern.Concern,
                        Requestor_Name = t.RequestorByUser.Fullname,
                        CompanyName = t.RequestConcern.OneChargingMIS.company_name,
                        Business_Unit_Name = t.RequestConcern.OneChargingMIS.business_unit_name,
                        Department_Name = t.RequestConcern.OneChargingMIS.department_name,
                        Unit_Name = t.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnit_Name = t.RequestConcern.OneChargingMIS.sub_unit_name,
                        Location_Name = t.RequestConcern.OneChargingMIS.location_name,
                        Category_Description = string.Join(", ", t.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory_Description = string.Join(", ", t.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Issue_Handler = t.User.Fullname,
                        Channel_Id = t.RequestConcern.ChannelId,
                        Channel_Name = t.RequestConcern.Channel.ChannelName,
                        Target_Date = t.TargetDate.Value.Date,
                        Created_At = t.CreatedAt.Date,
                        Modified_By = t.ModifiedByUser.Fullname,
                        Updated_At = t.UpdatedAt,
                        Remarks = t.Remarks,
                        Aging_Days = EF.Functions.DateDiffDay(t.TargetDate.Value.Date, DateTime.Now.Date),
                        ServiceProvider_Id = t.RequestConcern.ServiceProviderId,
                        ServiceProvider_Name = t.RequestConcern.ServiceProvider.ServiceProviderName
                        

                    }).ToListAsync(cancellationToken);

                if (request.ServiceProvider is not null)
                {
                    openTicket = openTicket
                            .Where(x => x.ServiceProvider_Id == request.ServiceProvider)
                            .ToList();

                    if (request.Channel is not null)
                    {
                        openTicket = openTicket
                            .Where(x => x.Channel_Id == request.Channel)
                            .ToList();

                        if (request.UserId is not null)
                        {
                            openTicket = openTicket
                                .Where(x => x.UserId == request.UserId)
                                .ToList();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(request.Search))
                {
                    openTicket = openTicket
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Issue_Handler.Contains(request.Search))
                        .ToList();
                }

                var resultOpenTicket = openTicket
                    .OrderBy(x => x.Target_Date.Value.Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(r => new OpenTicketExportResult
                    {
                        UserId = r.UserId,
                        UnitId = r.UnitId,
                        TicketConcernId = r.TicketConcernId,
                        Concern_Description = r.Concern_Description,
                        Requestor_Name = r.Requestor_Name,
                        CompanyName = r.CompanyName,
                        Business_Unit_Name = r.Business_Unit_Name,
                        Department_Name = r.Department_Name,
                        Unit_Name = r.Unit_Name,
                        SubUnit_Name = r.SubUnit_Name,
                        Location_Name = r.Location_Name,
                        Category_Description = r.Category_Description,
                        SubCategory_Description = r.SubCategory_Description,
                        Issue_Handler  = r.Issue_Handler,
                        Channel_Name = r.Channel_Name,
                        Target_Date = r.Target_Date,
                        Created_At = r.Created_At,
                        Modified_By = r.Modified_By,
                        Updated_At = r.Updated_At,
                        Remarks = r.Remarks,
                        Aging_Days = r.Aging_Days,
                        ServiceProvider_Name = r.ServiceProvider_Name,
                        
                    }).ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Open Ticket Report");
                    var headers = new List<string>
                    {
                        "TicketConcernId",
                        "Concern Description",
                        "Requestor Name",
                        "Company Name", 
                        "Business Unit Name",
                        "Department Name",
                        "Unit Name",
                        "Sub Unit Name",
                        "Location Name",
                        "Service Provider",
                        "Channel Name",
                        "Category Description",
                        "Sub Category Description",
                        "Issue Handler",
                        "Target Date",
                        "Created At",
                        "Modified By",
                        "Updated At",
                        "Remarks",
                        "Aging Days",
                        
                        
                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }
                    for (var index = 1; index <= resultOpenTicket.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = resultOpenTicket[index - 1].TicketConcernId;
                        row.Cell(2).Value = resultOpenTicket[index - 1].Concern_Description;
                        row.Cell(3).Value = resultOpenTicket[index - 1].Requestor_Name;
                        row.Cell(4).Value = resultOpenTicket[index - 1].CompanyName;
                        row.Cell(5).Value = resultOpenTicket[index - 1].Business_Unit_Name;
                        row.Cell(6).Value = resultOpenTicket[index - 1].Department_Name;
                        row.Cell(7).Value = resultOpenTicket[index - 1].Unit_Name;
                        row.Cell(8).Value = resultOpenTicket[index - 1].SubUnit_Name;
                        row.Cell(9).Value = resultOpenTicket[index - 1].Location_Name;
                        row.Cell(10).Value = resultOpenTicket[index - 1].ServiceProvider_Name;
                        row.Cell(11).Value = resultOpenTicket[index - 1].Channel_Name;
                        row.Cell(12).Value = resultOpenTicket[index - 1].Category_Description;
                        row.Cell(13).Value = resultOpenTicket[index - 1].SubCategory_Description;
                        row.Cell(14).Value = resultOpenTicket[index - 1].Issue_Handler;
                        row.Cell(15).Value = resultOpenTicket[index - 1].Target_Date;
                        row.Cell(16).Value = resultOpenTicket[index - 1].Created_At;
                        row.Cell(17).Value = resultOpenTicket[index - 1].Modified_By;
                        row.Cell(18).Value = resultOpenTicket[index - 1].Updated_At;
                        row.Cell(19).Value = resultOpenTicket[index - 1].Remarks;
                        row.Cell(20).Value = resultOpenTicket[index - 1].Aging_Days;
                        

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"OpenTicketReport {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;

            }
        }
    }
}
