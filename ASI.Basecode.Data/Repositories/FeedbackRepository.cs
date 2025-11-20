using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class FeedbackRepository: BaseRepository, IFeedbackRepository
    {
        public FeedbackRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            // Use raw SQL since Feedback table has no primary key
            return await GetDbSet<Feedback>()
                .FromSqlRaw("SELECT * FROM [dbo].[Feedbacks] ORDER BY [Date] DESC")
                .ToListAsync();
        }

        public async Task<Feedback> GetFeedbackByIdAsync(string id)
        {
            // Use raw SQL since Feedback table has no primary key
            return await GetDbSet<Feedback>()
                .FromSqlRaw("SELECT * FROM [dbo].[Feedbacks] WHERE [Id] = {0}", id)
                .FirstOrDefaultAsync();
        }

        public Task AddFeedbackAsync(Feedback feedback)
        {
            // Use raw SQL since Feedback table has no primary key
            var sql = @"INSERT INTO [dbo].[Feedbacks] ([Id], [Name], [Email], [Title], [Message], [Experience], [Date], [TicketId])
                        VALUES (@Id, @Name, @Email, @Title, @Message, @Experience, @Date, @TicketId)";

            Context.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Id", (object)feedback.Id ?? DBNull.Value),
                new SqlParameter("@Name", (object)feedback.Name ?? DBNull.Value),
                new SqlParameter("@Email", (object)feedback.Email ?? DBNull.Value),
                new SqlParameter("@Title", (object)feedback.Title ?? DBNull.Value),
                new SqlParameter("@Message", (object)feedback.Message ?? DBNull.Value),
                new SqlParameter("@Experience", (object)feedback.Experience ?? DBNull.Value),
                new SqlParameter("@Date", (object)feedback.Date ?? DBNull.Value),
                new SqlParameter("@TicketId", (object)feedback.TicketId ?? DBNull.Value));

            return Task.CompletedTask;
        }
    }
}
