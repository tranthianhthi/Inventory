using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.EntityFramework;

namespace CottonOnAPI.Models
{
    public class RPContext : DbContext
    {
        public RPContext(DbContextOptions<RPContext> options)
            : base(options)
        {
        }

        public DbSet<Slip> SlipItems { get; set; }
    }
}
