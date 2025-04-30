using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.Base
{
    public abstract class RepositoryBase
    {
        private readonly ILogger _logger;

        protected RepositoryBase(ILogger logger)
        {
            _logger = logger;
        }

        //Function for list or return type
        protected async Task<T> ExecuteDbActionAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error.");
                throw new Exception("Database error while processing data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during database operation.");
                throw new Exception("Unexpected error while processing data."); 
            }
        }

        // Function for void/ not return type
        protected async Task ExecuteDbActionAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error.");
                throw new Exception("Database error while processing data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during database operation.");
                throw new Exception("Unexpected error while processing data.");
            }
        }
    }
}
