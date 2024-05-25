using Microsoft.EntityFrameworkCore;
using NodeTree.Domain;

namespace NodeTree.Data
{
    public class NodeDbContext : DbContext
    {
        public NodeDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Nodes> Nodes { get; set; }


    }
}
