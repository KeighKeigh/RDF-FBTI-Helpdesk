using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport.SLAReport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.BackjobReport.BackJobReportHandler;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.FinanceReports.FinanceReport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.SADSLAReport.SADSLAReport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport.AllTicketReports;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport.TicketReports;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport.OnHoldTicketReport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport.OpenTicketReports;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TransferReport.TransferTicketReports;

namespace MakeItSimple.WebApi.Controllers.Report
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all-tickets")]
        public async Task<IActionResult> AllTicketReports([FromQuery] AllTicketReportsQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpGet("closing")]
        public async Task<IActionResult> TicketReports([FromQuery] TicketReportsQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }


        [HttpGet("open")]
        public async Task<IActionResult> OpenTicketReports([FromQuery] OpenTicketReportsQuery query)
        {
            try
            {
                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }


        [HttpGet("transfer")]
        public async Task<IActionResult> TransferTicketReports([FromQuery] TransferTicketReportsQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpGet("on-hold")]
        public async Task<IActionResult> OnHoldTicketReport([FromQuery] OnHoldTicketReportQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpGet("sla-report")]
        public async Task<IActionResult> SLATicketReports([FromQuery] SLAReportQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }



        [HttpGet("finance-report")]
        public async Task<IActionResult> FinanceReports([FromQuery] FinanceReportQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }


        [HttpGet("sad-sla-report")]
        public async Task<IActionResult> SADSLAReports([FromQuery] SADSLAReportQuery query)
        {
            try
            {

                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpGet("rework")]
        public async Task<IActionResult> ReworkTicketReports([FromQuery] BackJobReportQuery query)
        {
            try
            {
                var reports = await _mediator.Send(query);

                Response.AddPaginationHeader(

                reports.CurrentPage,
                reports.PageSize,
                reports.TotalCount,
                reports.TotalPages,
                reports.HasPreviousPage,
                reports.HasNextPage

                );

                var result = new
                {
                    reports,
                    reports.CurrentPage,
                    reports.PageSize,
                    reports.TotalCount,
                    reports.TotalPages,
                    reports.HasPreviousPage,
                    reports.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

    }
}
