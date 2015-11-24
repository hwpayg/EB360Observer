using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using EB360Observer.Entity;
namespace EB360Observer.DAL
{
    public class EbItemDbContext : DbContext
    {
        public EbItemDbContext()
            : base("name=EBMsSql")
        {

        }
        public DbSet<EBItem> EBItems { get; set; }
    }
}
