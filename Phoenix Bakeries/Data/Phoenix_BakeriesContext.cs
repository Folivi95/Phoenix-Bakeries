﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Phoenix_Bakeries.Models;

namespace Phoenix_Bakeries.Models
{
    public class Phoenix_BakeriesContext : DbContext
    {
        public Phoenix_BakeriesContext (DbContextOptions<Phoenix_BakeriesContext> options)
            : base(options)
        {
        }

        public DbSet<Phoenix_Bakeries.Models.Transfer> Transfer { get; set; }

        public DbSet<Phoenix_Bakeries.Models.NewTransfer> NewTransfer { get; set; }
    }
}
