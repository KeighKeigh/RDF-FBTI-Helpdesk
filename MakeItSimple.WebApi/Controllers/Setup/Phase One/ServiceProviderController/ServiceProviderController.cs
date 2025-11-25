using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.AddNewChannel;
using System.Security.Claims;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup.AddNewServiceProvider;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.GetChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.GetChannelValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.GetMember;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.UpdateChannelStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup.GetServiceProviderValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup.UpdateServiceProviderStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup.GetServiceProvider;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup.GetServiceProviders;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_One.ServiceProviderController
{
    [Route("api/service-provider")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ValidatorHandler _validatorHandler;

        public ServiceProviderController(IMediator mediator, ValidatorHandler validatorHandler)
        {
            _mediator = mediator;
            _validatorHandler = validatorHandler;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetServiceProvider([FromQuery] GetServiceProviderQuery query)
        {
            try
            {
                var serviceProvider = await _mediator.Send(query);

                Response.AddPaginationHeader(

                serviceProvider.CurrentPage,
                serviceProvider.PageSize,
                serviceProvider.TotalCount,
                serviceProvider.TotalPages,
                serviceProvider.HasPreviousPage,
                serviceProvider.HasNextPage

                );

                var result = new
                {
                    serviceProvider,
                    serviceProvider.CurrentPage,
                    serviceProvider.PageSize,
                    serviceProvider.TotalCount,
                    serviceProvider.TotalPages,
                    serviceProvider.HasPreviousPage,
                    serviceProvider.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("pages")]
        public async Task<IActionResult> GetServiceProviders([FromQuery] GetServiceProvidersQuery query)
        {
            try
            {
                var serviceProvider = await _mediator.Send(query);

                Response.AddPaginationHeader(

                serviceProvider.CurrentPage,
                serviceProvider.PageSize,
                serviceProvider.TotalCount,
                serviceProvider.TotalPages,
                serviceProvider.HasPreviousPage,
                serviceProvider.HasNextPage

                );

                var result = new
                {
                    serviceProvider,
                    serviceProvider.CurrentPage,
                    serviceProvider.PageSize,
                    serviceProvider.TotalCount,
                    serviceProvider.TotalPages,
                    serviceProvider.HasPreviousPage,
                    serviceProvider.HasNextPage
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
        public async Task<IActionResult> UpdateServiceProviderStatus([FromRoute] int id)
        {

            try
            {
                var command = new UpdateServiceProviderStatusCommand
                {
                    Id = id
                };
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



        //[HttpGet("member-list")]
        //public async Task<IActionResult> GetMember([FromQuery] GetMemberQuery query)
        //{
        //    try
        //    {
        //        var result = await _mediator.Send(query);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Conflict(ex.Message);
        //    }
        //}


        [HttpPost("validation")]
        public async Task<IActionResult> GetServiceProviderValidation(GetServiceProviderValidationCommand command)
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


        [HttpPost("addNewServiceProvider")]
        public async Task<ActionResult> AddNewServiceProvider([FromBody] AddNewServiceProviderCommands command)
        {
            try
            {
                //if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                //{
                //    command.addedBy = userId;
                //    command.modifiedBy = userId;
                //}
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
