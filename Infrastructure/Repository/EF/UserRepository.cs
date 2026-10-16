using Application.DTOs.AuthDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using FluentValidation;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.EF
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {

        public UserRepository(StoreLabDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<bool> IsEmailExsists(string email)
        {
            return await _dbSet.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> IsPhoneNumberExsists(string phoneNumber)
        {
            return await _dbSet.AnyAsync(x => x.PhoneNumber.Equals(phoneNumber));
        }
    }
}
