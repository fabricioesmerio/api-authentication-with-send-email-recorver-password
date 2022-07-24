using apiLogin.Models;
using Microsoft.EntityFrameworkCore;

namespace apiLogin.Data
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Solicitacao> Solicitacoes { get; set; }
    }
}