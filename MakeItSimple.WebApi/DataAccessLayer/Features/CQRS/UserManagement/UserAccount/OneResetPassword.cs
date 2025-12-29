using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount
{
    
        public class OneResetPassword
        {
            public class OneResetPasswordResult
            {
                public Guid Id { get; set; }
                public bool? IsPasswordChange { get; set; }
            }

            public class OneResetPasswordCommand : IRequest<Result>
            {

                public string EmpId { get; set; } = string.Empty;

            }

            public class Handler : IRequestHandler<OneResetPasswordCommand, Result>
            {
                private readonly MisDbContext _context;

                public Handler(MisDbContext context)
                {
                    _context = context;
                }

                public async Task<Result> Handle(OneResetPasswordCommand command, CancellationToken cancellationToken)
                {

                    var User = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == command.EmpId, cancellationToken);

                    if (User == null)
                    {
                        return Result.Failure(UserError.UserNotExist());

                    }

                    User.Password = BCrypt.Net.BCrypt.HashPassword(User.Username);
                    User.IsPasswordChange = null;

                    await _context.SaveChangesAsync(cancellationToken);

                    var results = new OneResetPasswordResult
                    {
                        Id = User.Id,
                        IsPasswordChange = User.IsPasswordChange
                    };

                    return Result.Success(results);

                }




            }
        }
    
}
