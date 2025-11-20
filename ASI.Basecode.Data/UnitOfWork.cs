using ASI.Basecode.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace ASI.Basecode.Data
{

    /// <summary>
    /// Unit of Work Implementation
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets the database context
        /// </summary>
        public DbContext Database { get; private set; }

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        public UnitOfWork(NexDeskDbContext serviceContext)
        {
            Database = serviceContext;
        }

        /// <summary>
        /// Saves the changes to database
        /// </summary>
        public void SaveChanges()
        {
            Database.SaveChanges();
        }

        // Note: The DbContext instance is provided by the DI container and
        // will be disposed by the container at the end of the scope. Do not
        // dispose it here to avoid double-dispose issues and 'disposed' logs.
    }
}
