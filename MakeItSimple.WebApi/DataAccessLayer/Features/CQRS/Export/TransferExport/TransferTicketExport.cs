using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Export.OpenExport.OpenTicketExport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.TransferExport
{
    public partial class TransferTicketExport
    {

        public class Handler : IRequestHandler<TransferTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(TransferTicketExportCommand request, CancellationToken cancellationToken)
            {
                var _transferQuery = await _context.TransferTicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery()
                    .OrderBy(x => x.Current_Target_Date)
                    .Where(x => x.IsTransfer == true && x.TicketConcern.UserId != null)
                    .Where(x => x.TransferAt.Value.Date >= request.Date_From.Value.Date && x.TransferAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new TransferTicketExportResult
                    {
                        Unit = x.TransferByUser.UnitId,
                        UserId = x.TransferBy,
                        TicketConcernId = x.TicketConcernId,
                        TransferTicketId = x.Id,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Transfered_By = x.TransferByUser.Fullname,
                        Transfered_To = x.TransferToUser.Fullname,
                        New_Target_Date = x.Current_Target_Date.Value.ToString("MM/dd/yyyy"),
                        Previous_Target_Date = x.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Transfer_At = x.TransferAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        Transfer_Remarks = x.TransferRemarks,
                        Remarks = x.TransferRemarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ApprovedBy = x.ApprovedBy,
                        ServiceProviderId = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ServiceProviderName = x.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ChannnelId = x.TicketConcern.RequestConcern.ChannelId,
                        ChannnelName = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        Requested_Date = x.TicketConcern.CreatedAt.ToString("MM/dd/yyyy HH:mm"),



                    }).ToListAsync(cancellationToken);

                if (request.ServiceProvider is not null)
                {
                    _transferQuery = _transferQuery.Where(x => x.ServiceProviderId == request.ServiceProvider)
                            .ToList();

                    if (request.Channel is not null)
                    {
                        _transferQuery = _transferQuery.Where(x => x.ChannnelId == request.Channel)
                            .ToList();

                        if (request.UserId is not null)
                        {
                            _transferQuery = _transferQuery.Where(x => x.UserId == request.UserId)
                                .ToList();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    _transferQuery = _transferQuery
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Transfered_By.Contains(request.Search))
                        .ToList();
                }

                //var finalTransferQuery = _transferQuery
                //    .OrderBy(x => x.New_Target_Date)
                //    .ThenBy(x => x.TicketConcernId)
                //    .Select(r => new TransferTicketExportResult
                //    {
                //        Unit = r.Unit,
                //        UserId = r.UserId,
                //        TicketConcernId = r.TicketConcernId,
                //        TransferTicketId = r.TransferTicketId,
                //        Concern_Details = r.Concern_Details,
                //        Transfered_By = r.Transfered_By,
                //        Transfered_To = r.Transfered_To,
                //        New_Target_Date = r.New_Target_Date,
                //        Previous_Target_Date = r.Previous_Target_Date,
                //        Transfer_At = r.Transfer_At,
                //        Transfer_Remarks = r.Transfer_Remarks,
                //        Modified_By = r.Modified_By,
                //        Updated_At = r.Updated_At,
                //        ApprovedBy = r.ApprovedBy,
                //        ServiceProviderId = r.ServiceProviderId,
                //        ServiceProviderName = r.ServiceProviderName,
                //        ChannnelId = r.ChannnelId,
                //        ChannnelName = r.ChannnelName,
                //        Requested_Date = r.Requested_Date,
                //    });
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Transfer Ticket Report");
                    var headers = new List<string>
                    {
                        "Transferred By",
                        "Ticket No.",
                        "Ticket Description",
                        "Requested Date",
                        "Previous Target Date",
                        "Approved Date",
                        "New Target Date",
                        "Transferred To",
                        "Transferred No.",
                        "Transfer Remarks",
                        "Modified By",
                        "Updated At",
                        "Approved By",
                        "Service Provider",
                        "Channel"

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
                    for (var index = 1; index <= _transferQuery.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = _transferQuery[index - 1].Transfered_By;
                        row.Cell(2).Value = _transferQuery[index - 1].TicketConcernId;
                        row.Cell(3).Value = _transferQuery[index - 1].Concern_Details;
                        row.Cell(4).Value = _transferQuery[index - 1].Requested_Date;//requestdate
                        row.Cell(5).Value = _transferQuery[index - 1].Previous_Target_Date;
                        row.Cell(6).Value = _transferQuery[index - 1].Transfer_At;
                        row.Cell(7).Value = _transferQuery[index - 1].New_Target_Date;//new targetdate
                        row.Cell(8).Value = _transferQuery[index - 1].Transfered_To;
                        row.Cell(9).Value = _transferQuery[index - 1].TransferTicketId;
                        row.Cell(10).Value = _transferQuery[index - 1].Transfer_Remarks;
                        row.Cell(11).Value = _transferQuery[index - 1].Modified_By;
                        row.Cell(12).Value = _transferQuery[index - 1].Updated_At;
                        row.Cell(13).Value = _transferQuery[index - 1].ApprovedBy;
                        row.Cell(14).Value = _transferQuery[index - 1].ServiceProviderName;
                        row.Cell(15).Value = _transferQuery[index - 1].ChannnelName;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"TransferTicketReport {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;

            }

        }
    }
}
