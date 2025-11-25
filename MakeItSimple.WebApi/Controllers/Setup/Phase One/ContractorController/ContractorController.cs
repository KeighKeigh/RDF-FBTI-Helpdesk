using Azure;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ContractorSetup.GetContractor;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ContractorSetup.UpdateContractorStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ContractorSetup.UpsertContractor;


namespace MakeItSimple.WebApi.Controllers.Setup.Phase_One.ContractorController
{
    [Route("api/Contractor")]
    [ApiController]
    public class ContractorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContractorController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> UpsertContractor([FromBody] UpsertContractorCommand command)
        {

            try
            {

                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetContractor([FromQuery] GetContractorQuery query)
        {
            try
            {
                var category = await _mediator.Send(query);

                Response.AddPaginationHeader(

                category.CurrentPage,
                category.PageSize,
                category.TotalCount,
                category.TotalPages,
                category.HasPreviousPage,
                category.HasNextPage

                );

                var result = new
                {
                    category,
                    category.CurrentPage,
                    category.PageSize,
                    category.TotalCount,
                    category.TotalPages,
                    category.HasPreviousPage,
                    category.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateContractorStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateContractorStatusCommand
                {
                    Id = id
                };
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
