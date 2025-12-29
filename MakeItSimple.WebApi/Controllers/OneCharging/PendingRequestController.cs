using Azure;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Controllers.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.CreatePendingRequest.CreatePendingRequestHandler;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.GetPendingRequest.GetPendingRequestHandler;

namespace MakeItSimple.WebApi.Controllers.OneCharging
{
    [Route("api/PendingRequest")]
    [ApiController]
    public class PendingRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PendingRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [ApiKeyAuth]
        [HttpPost]
        public async Task<IActionResult> CreatePendingRequest(CreatePendingRequestCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPendingRequest([FromQuery] GetPendingRequestQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
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
