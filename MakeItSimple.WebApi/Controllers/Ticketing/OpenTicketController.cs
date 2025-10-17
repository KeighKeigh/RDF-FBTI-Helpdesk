using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
//using Microsoft.AspNetCore.SignalR;
//using MakeItSimple.WebApi.Common.SignalR;
//using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OnHoldTicket.GetOnHoldApprover.GetOnHoldApprover;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetClosedTicketSubUnit.GetClosedTicketSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit.GetOpenTicketSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ApprovalDateTicket.ApprovalDateTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.NewFolder.DateApproval;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.RejectDateTicket.RejectDateTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosing.ApprovalClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing.GetClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.RejectClosing.RejectClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewTicketHistory.GetTicketHistory;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/open-ticket")]
    [ApiController]
    public class OpenTicketController : ControllerBase
    {

        private readonly IMediator _mediator;
        //private readonly TimerControl _timerControl;

        public OpenTicketController(IMediator mediator)
        {
            _mediator = mediator;
            //_timerControl = timerControl;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetOpenTicket([FromQuery] GetOpenTicketQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    { 
                        query.UserId = userId;
                    }

                }

                var openTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                openTicket.CurrentPage,
                openTicket.PageSize,
                openTicket.TotalCount,
                openTicket.TotalPages,
                openTicket.HasPreviousPage,
                openTicket.HasNextPage

                );

                var result = new
                {
                    openTicket,
                    openTicket.CurrentPage,
                    openTicket.PageSize,
                    openTicket.TotalCount,
                    openTicket.TotalPages,
                    openTicket.HasPreviousPage,
                    openTicket.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("page_subunit")]
        public async Task<IActionResult> GetClosingTicket([FromQuery] GetOpenTicketSubUnitQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                    }
                }

                var closingTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                closingTicket.CurrentPage,
                closingTicket.PageSize,
                closingTicket.TotalCount,
                closingTicket.TotalPages,
                closingTicket.HasPreviousPage,
                closingTicket.HasNextPage

                );

                var result = new
                {
                    closingTicket,
                    closingTicket.CurrentPage,
                    closingTicket.PageSize,
                    closingTicket.TotalCount,
                    closingTicket.TotalPages,
                    closingTicket.HasPreviousPage,
                    closingTicket.HasNextPage
                };

                var successResult = Result.Success(result);


                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

       
        //[HttpGet("page_close_subunit")]
        //public async Task<IActionResult> GetClosingTicket([FromQuery] GetClosedTicketSubUnitQuery query)
        //{
        //    try
        //    {
        //        if (User.Identity is ClaimsIdentity identity)
        //        {
        //            var userRole = identity.FindFirst(ClaimTypes.Role);
        //            if (userRole != null)
        //            {
        //                query.Role = userRole.Value;
        //            }

        //            if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
        //            {
        //                query.UserId = userId;
        //            }
        //        }

        //        var closingTicket = await _mediator.Send(query);

        //        Response.AddPaginationHeader(

        //        closingTicket.CurrentPage,
        //        closingTicket.PageSize,
        //        closingTicket.TotalCount,
        //        closingTicket.TotalPages,
        //        closingTicket.HasPreviousPage,
        //        closingTicket.HasNextPage

        //        );

        //        var result = new
        //        {
        //            closingTicket,
        //            closingTicket.CurrentPage,
        //            closingTicket.PageSize,
        //            closingTicket.TotalCount,
        //            closingTicket.TotalPages,
        //            closingTicket.HasPreviousPage,
        //            closingTicket.HasNextPage
        //        };

        //        var successResult = Result.Success(result);


        //        return Ok(successResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }

        //}

        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetTicketHistory([FromRoute] int id)
        {
            try
            {
                var query = new GetTicketHistoryQuery
                {
                    TicketConcernId = id
                };

                var results = await _mediator.Send(query);
                if (results.IsFailure)
                {
                    return BadRequest(query);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetDateNotApproved([FromQuery] DateApprovalQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                    }
                }

                var closingTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                closingTicket.CurrentPage,
                closingTicket.PageSize,
                closingTicket.TotalCount,
                closingTicket.TotalPages,
                closingTicket.HasPreviousPage,
                closingTicket.HasNextPage

                );

                var result = new
                {
                    closingTicket,
                    closingTicket.CurrentPage,
                    closingTicket.PageSize,
                    closingTicket.TotalCount,
                    closingTicket.TotalPages,
                    closingTicket.HasPreviousPage,
                    closingTicket.HasNextPage
                };

                var successResult = Result.Success(result);


                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("approval")]
        public async Task<IActionResult> ApprovalClosingTicket([FromBody] ApprovalDateTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.ApprovedDateBy = userId;
                        command.Users = userId;
                        command.Transacted_By = userId;
                    }
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectTargetDateTicket([FromBody] RejectDateTicketCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.RejectDate_By = userId;
                        command.Transacted_By = userId;
                    }
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
