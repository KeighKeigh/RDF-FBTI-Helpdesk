using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount
{
    public class AddNewUser
    {

        public class AddNewUserResult
        {
            public Guid Id { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public int UserRoleId { get; set; }
            public int? DepartmentId { get; set; }
            public int? SubUnitId { get; set; }
            public int? UnitId { get; set; }
            public int? CompanyId { get; set; }
            public string LocationCode { get; set; }

            public int? BusinessUnitId { get; set; }
            public string OneChargingCode { get; set; }
            public string OneChargingName { get; set; }

            //public int ? TeamId { get; set; }


            public Guid? Added_By { get; set; }

        }

        public class AddNewUserCommand : IRequest<Result>
        {

            public string EmpId { get; set; }
            public string Fullname { set; get; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int UserRoleId { get; set; }
            public int? DepartmentId { get; set; }
            public int? SubUnitId { get; set; }

            //public int ? TeamId { get; set; }

            public int? UnitId { get; set; }

            public int? CompanyId { get; set; }
            public string LocationCode { get; set; }

            public int? BusinessUnitId { get; set; }

            public bool? Is_Store { get; set; }

            public Guid? Added_By { get; set; }
            public string OneChargingCode { get; set; }
            public string OneChargingName { get; set; }


        }


        public class Handler : IRequestHandler<AddNewUserCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewUserCommand command, CancellationToken cancellationToken)
            {

                var UserAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == command.EmpId && x.Fullname == command.Fullname, cancellationToken);

                if (UserAlreadyExist != null)
                {
                    return Result.Failure(UserError.UserAlreadyExist(command.EmpId, command.Fullname));
                }

                var UsernameAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Username == command.Username, cancellationToken);
                if (UsernameAlreadyExist != null)
                {
                    return Result.Failure(UserError.UsernameAlreadyExist(command.Username));
                }

                var UserRoleNotExist = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == command.UserRoleId, cancellationToken);

                if (UserRoleNotExist == null)
                {
                    return Result.Failure(UserError.UserRoleNotExist());
                }


                var CompanyNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.company_id == command.CompanyId, cancellationToken);

                if (CompanyNotExist == null)
                {
                    return Result.Failure(UserError.CompanyNotExist());
                }

                var BusinessUnitNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.business_unit_id == command.BusinessUnitId, cancellationToken);

                if (BusinessUnitNotExist == null)
                {
                    return Result.Failure(UserError.BusinessUnitNotExist());
                }

                var DepartmentNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.department_id == command.DepartmentId, cancellationToken);

                if (DepartmentNotExist == null)
                {
                    return Result.Failure(UserError.DepartmentNotExist());
                }


                var UnitNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.department_unit_id == command.UnitId, cancellationToken);
                if (UnitNotExist == null)
                {
                    return Result.Failure(UserError.UnitNotExist());
                }

                var SubUnitNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.sub_unit_id == command.SubUnitId, cancellationToken);

                if (SubUnitNotExist == null)
                {
                    return Result.Failure(UserError.SubUnitNotExist());
                }

                var LocationNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.location_code == command.LocationCode, cancellationToken);

                if (LocationNotExist == null)
                {
                    return Result.Failure(UserError.LocationNotExist());
                }

                var OneChrgingNotExist = await _context.OneChargings.FirstOrDefaultAsync(x => x.code == command.OneChargingCode, cancellationToken);

                if (OneChrgingNotExist == null)
                {
                    return Result.Failure(UserError.OneCharginNotExist());
                }


                var users = new User
                {

                    EmpId = command.EmpId,
                    Fullname = command.Fullname,
                    Username = command.Username,
                    //Password = BCrypt.Net.BCrypt.HashPassword(command.Username),
                    Password = command.Password.IsNullOrEmpty() ? BCrypt.Net.BCrypt.HashPassword(command.Username) : command.Password,
                    UserRoleId = command.UserRoleId,
                    SubUnitId = command.SubUnitId,
                    ProfilePic = "https://res-console.cloudinary.com/dctfcg76v/thumbnails/v1/image/upload/v1719373008/TWFrZUlUU2ltcGxlL1JERi1GZWF0dXJlZF9zdWJ6dXU=/drilldown",
                    FileName = "RDF-Featured_subzuu",
                    FileSize = 84.47m,
                    DepartmentId = command.DepartmentId,
                    CompanyId = command.CompanyId,
                    LocationId = LocationNotExist.Id,
                    BusinessUnitId = command.BusinessUnitId.Value,
                    UnitId = command.UnitId,
                    AddedBy = command.Added_By,
                    IsStore = command.Is_Store,
                    OneChargingCode = command.OneChargingCode,
                    OneChargingName = command.OneChargingName,

                };

                await _context.Users.AddAsync(users, cancellationToken);

                var pendinguser = await _context.PendingRequests.FirstOrDefaultAsync(x => (x.IdPrefix + "-" + x.IdNo) == command.EmpId, cancellationToken);

                if (pendinguser != null)
                {
                    _context.PendingRequests.Remove(pendinguser);
                }

                await _context.SaveChangesAsync(cancellationToken);

                var result = new AddNewUserResult
                {
                    Id = users.Id,
                    EmpId = users.EmpId,
                    Fullname = users.Fullname,
                    Username = users.Username,
                    UserRoleId = users.UserRoleId,
                    DepartmentId = users.DepartmentId,
                    SubUnitId = users.SubUnitId,
                    //TeamId = users.TeamId,  
                    CompanyId = users.CompanyId,
                    LocationCode = command.LocationCode,
                    BusinessUnitId = users.BusinessUnitId,
                    UnitId = users.UnitId,
                    Added_By = users.AddedBy,
                    OneChargingCode = users.OneChargingCode,
                    OneChargingName = users.OneChargingName,
                    
                };

                return Result.Success(result);

            }
        }




    }
}
