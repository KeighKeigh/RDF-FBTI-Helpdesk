using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount
{
    public class OneChangePassword
    {
        public class OneChangePasswordResult
        {
            public Guid Id { get; set; }
            public bool? Is_PasswordChanged { get; set; }
        }

        public class OneChangePasswordCommand : IRequest<Result>
        {
            public string EmpId { get; set; }

            [Required]
            public string old_password { get; set; }
            [Required]
            public string password { get; set; }

        }

        public class Handler : IRequestHandler<OneChangePasswordCommand, Result>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }
            public async Task<Result> Handle(OneChangePasswordCommand command, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == command.EmpId, cancellationToken);

                if (user == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }


                if (!BCrypt.Net.BCrypt.Verify(command.old_password, user.Password))
                {
                    return Result.Failure(UserError.UserOldPasswordInCorrect());

                }

                if (command.password == user.Username)
                {
                    return Result.Failure(UserError.InvalidDefaultPassword());
                }

                if (command.password == command.old_password)
                {
                    return Result.Failure(UserError.UserPasswordShouldChange());
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(command.password);
                user.IsPasswordChange = true;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new OneChangePasswordResult
                {
                    Id = user.Id,
                    Is_PasswordChanged = user.IsPasswordChange

                };

                return Result.Success(result);

            }
        }
    }
}
