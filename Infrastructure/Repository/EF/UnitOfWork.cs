using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Common.Exceptions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly StoreLabDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(StoreLabDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            try
            {
                foreach (var entry in _dbContext.ChangeTracker.Entries())
                {
                    Console.WriteLine(
                        $"BEFORE SAVE => " +
                        $"Entity: {entry.Entity.GetType().Name} | " +
                        $"State: {entry.State}");
                }

                return await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //throw new ConcurrencyException(
                //    "A concurrency conflict occurred.",
                //    ex);

                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine(
                        $"Concurrency Conflict Entity: {entry.Entity.GetType().Name}");

                    Console.WriteLine(
                        $"State: {entry.State}");

                }

                throw new ConcurrencyException(
                    "A concurrency conflict occurred.",
                    ex);
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
