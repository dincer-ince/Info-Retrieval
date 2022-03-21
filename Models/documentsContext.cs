using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoRetrieval.Models
{
    public class documentsContext:DbContext
    {
        public documentsContext(DbContextOptions<documentsContext> options):base(options)
        {

        }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<Dictionary> Dictionary{ get; set; }
    }
}
