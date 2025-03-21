using CibusServer.DAL.SQL.Entities;
using CibusServer.Models;
using CibusServer.Services;
using Microsoft.EntityFrameworkCore;

namespace CibusServer.DAL.SQL
{
    public abstract class RepositoryBase
    {
        protected IConfiguration _configuration;
        public RepositoryBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected virtual AppDbContext? CreateDbContext()
        {
            try
            {
                var connectionString = _configuration["DB_CONNECTION_STRING"];
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 17));
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseMySql(connectionString, serverVersion,
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 100,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                        )

                    )
                    .Options;
                return new AppDbContext(options);
            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when Create Db Context, Error messsage : {ex.Message}");
                return null;
            }
        }

        public AppDbContext GetDbCtx() => CreateDbContext();
        public async Task CreateOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await using var ctx = GetDbCtx();
            var table = ctx.Set<TEntity>();
            if (await table.ContainsAsync(entity))
            {
                table.Update(entity);
            }
            else
            {
                await table.AddAsync(entity);
            }

            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await using var ctx = GetDbCtx();
            var table = ctx.Set<TEntity>();
            table.Update(entity);
            await ctx.SaveChangesAsync();

        }

        public async Task DeleteAsync<TEntity>(TEntity id) where TEntity : class
        {
            await using var ctx = GetDbCtx();
            var table = ctx.Set<TEntity>();
            table.Remove(id);
            await ctx.SaveChangesAsync();
        }

        public virtual async Task<List<MessageEntity>> GetAllMessages()
        {
            await using var ctx = GetDbCtx();
            return await ctx.Messages.ToListAsync();
        }

        public virtual async Task<List<MessageEntity>> GetAllMessagesByUserId(int userId)
        {
            await using var ctx = GetDbCtx();
            return await ctx.Messages
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }


        public virtual async Task<UserEntity> Login(string username, string password)
        {
            await using var ctx = GetDbCtx();

            var res = await ctx.Users
                .SingleOrDefaultAsync(u => u.Password == password && u.UserName == username);
            return res;
        }

        public virtual async Task<UserEntity> GetUserByUserName(string username)
        {
            await using var ctx = GetDbCtx();
            return ctx.Users.FirstOrDefault(u => u.UserName == username);
        }

        public virtual async Task<UserEntity> GetUserById(int id)
        {
            await using var ctx = GetDbCtx();
            return ctx.Users.FirstOrDefault(u => u.Id == id);
        }

        public virtual async Task<MessageEntity> GetMessageById(int id)
        {
            await using var ctx = GetDbCtx();
            return ctx.Messages.FirstOrDefault(u => u.Id == id);
        }

        public virtual async Task<UserEntity> AddUser(UserEntity entity)
        {
            await using var ctx = GetDbCtx();

            ctx.Users.Add(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<MessageEntity> AddMessage(MessageEntity entity)
        {
            await using var ctx = GetDbCtx();

            ctx.Messages.Add(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<MessageEntity> UpdateMessage(MessageEntity messageentity)
        {
            await using var ctx = GetDbCtx();

            ctx.Messages.Update(messageentity);
            await ctx.SaveChangesAsync();
            return messageentity;
        }

        public virtual async Task<MessageEntity> RemoveMessage(MessageEntity entity)
        {
            await using var ctx = GetDbCtx();

            ctx.Messages.Remove(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

    }
}
