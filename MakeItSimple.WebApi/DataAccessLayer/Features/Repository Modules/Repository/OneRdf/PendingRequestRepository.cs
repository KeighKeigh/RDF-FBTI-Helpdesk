using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.OneRdf;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.OneCharging;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.CreatePendingRequest.CreatePendingRequestHandler;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.OneRdf
{
    public class PendingRequestRepository : IPendingRequestRepository
    {
        private readonly MisDbContext _context;
        public PendingRequestRepository(MisDbContext context)
        {
            _context = context;
        }

        public async Task<User> ExistingUserByEmpId(string idPrefix, string idNumber)
        {
            var empId = $"{idPrefix}-{idNumber}";
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == empId);

            return existingUser;
        }

        public async Task<PendingRequest> ExistingPendingUserByEmpId(string idPrefix, string idNumber)
        {
            var existingUser = await _context.PendingRequests.FirstOrDefaultAsync(x => x.IdPrefix == idPrefix && x.IdNo == idNumber);

            return existingUser;
        }

        public async Task<bool> UpdateExistingUser(CreatePendingRequestCommand command)
        {
            var empId = $"{command.Id_Prefix}-{command.Id_No}";
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
            var existingUserName = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == empId);

            if (existingUserName != null)
            {
                existingUserName.Username = command.Username;
                existingUserName.Password = hashPassword;

                return true;
            }



            return false;
        }

        public async Task<bool> UpdateExistingPendingUser(CreatePendingRequestCommand command)
        {
            var existingUser = await _context.PendingRequests.FirstOrDefaultAsync(x => x.IdPrefix == command.Id_Prefix && x.IdNo == command.Id_No);

            if (existingUser == null)
            {
                return false;
            }

            existingUser.Username = command.Username;
            existingUser.Password = command.Password;
            existingUser.FirstName = command.First_Name;
            existingUser.MiddleName = command.Middle_Name;
            existingUser.LastName = command.Last_Name;
            existingUser.Suffix = command.Suffix;

            return true;
        }

        public async Task<PendingRequest> AddNewPendingAccount(CreatePendingRequestCommand command)
        {
            var addNewPendingAccount = new PendingRequest
            {
                IdPrefix = command.Id_Prefix,
                IdNo = command.Id_No,
                Username = command.Username,
                Password = command.Password,
                FirstName = command.First_Name,
                LastName = command.Last_Name,
                MiddleName = command.Middle_Name,
                Suffix = command.Suffix,
            };

            await _context.PendingRequests.AddAsync(addNewPendingAccount);
            return addNewPendingAccount;
        }


        public async Task<bool> UsernameExist(string username)
        {
            var usernameExist = await _context.Users.AnyAsync(x => x.Username == username);

            return usernameExist;
        }

        public async Task<bool> UsernameExistInPendingRequest(string idPrefix, string idNo)
        {
            var usernameExist = await _context.PendingRequests.AnyAsync(x => x.IdPrefix == idPrefix && x.IdNo == idNo);

            return usernameExist;
        }
    }
}
