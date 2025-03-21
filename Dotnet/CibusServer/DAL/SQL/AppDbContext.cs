using CibusServer.DAL.SQL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CibusServer.DAL.SQL
{
    public class AppDbContext : DbContext
    {
        public DbSet<MessageEntity> Messages { get; set; }

        public DbSet<UserEntity> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
