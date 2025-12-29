using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.OneRdf;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.CreatePendingRequest
{
    public class CreatePendingRequestHandler
    {
        public class CreatePendingRequestCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string Id_Prefix { get; set; }
            public string Id_No { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Middle_Name { get; set; }
            public string Suffix { get; set; }
        }

        public class Handler : IRequestHandler<CreatePendingRequestCommand, Result>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            async public Task<Result> Handle(CreatePendingRequestCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreatePendingRequest(command, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();

            }

            private async Task<Result> Validator(CreatePendingRequestCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(command.Id_Prefix))
                {
                    return Result.Failure(PendingRequestError.EmptyUsername());
                }

                if (string.IsNullOrWhiteSpace(command.Id_No))
                {
                    return Result.Failure(PendingRequestError.EmptyIdNo());
                }

                if (string.IsNullOrWhiteSpace(command.Username))
                {
                    return Result.Failure(PendingRequestError.EmptyUsername());
                }

                if (string.IsNullOrWhiteSpace(command.Password))
                {
                    return Result.Failure(PendingRequestError.EmptyPassword());
                }

                if (string.IsNullOrWhiteSpace(command.First_Name))
                {
                    return Result.Failure(PendingRequestError.EmptyFirstName());
                }

                if (string.IsNullOrWhiteSpace(command.Last_Name))
                {
                    return Result.Failure(PendingRequestError.EmptyLastName());
                }


                //var existingUsername = await _unitOfWork.PendingRequests.UsernameExist(command.Username);

                //if(existingUsername == true)
                //{
                //    return Result.Failure(PendingRequestError.UserNameExists());
                //}

                //var existingUsernamePending = await _unitOfWork.PendingRequests.UsernameExistInPendingRequest(command.Id_Prefix, command.Id_No);

                //if (existingUsernamePending != true)
                //{
                //    return Result.Failure(PendingRequestError.EmpIdExists());
                //}
                return null;
            }


            private async Task<Result> CreatePendingRequest(CreatePendingRequestCommand command, CancellationToken cancellationToken)
            {
                var userExist = await _unitOfWork.PendingRequests.ExistingUserByEmpId(command.Id_Prefix, command.Id_No);

                if (userExist != null)
                {
                    var updateExistingUser = await _unitOfWork.PendingRequests.UpdateExistingUser(command);
                    if (updateExistingUser == true)
                    {
                        return Result.Success();
                    }

                }



                var userPendingExist = await _unitOfWork.PendingRequests.ExistingPendingUserByEmpId(command.Id_Prefix, command.Id_No);
                if (userPendingExist != null)
                {
                    var updateExistingPendingUser = await _unitOfWork.PendingRequests.UpdateExistingPendingUser(command);
                    if (updateExistingPendingUser == false)
                    {
                        return Result.Failure(PendingRequestError.PendingUserError());
                    }
                    return Result.Success();
                }

                var existingUsername = await _unitOfWork.PendingRequests.UsernameExist(command.Username);

                if (existingUsername == true)
                {
                    return Result.Failure(PendingRequestError.UserNameExists());
                }

                var addNewUser = await _unitOfWork.PendingRequests.AddNewPendingAccount(command);

                return Result.Success(addNewUser);


            }
        }
    }
}
