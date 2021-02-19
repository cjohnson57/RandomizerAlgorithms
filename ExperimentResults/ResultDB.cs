using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace RandomizerAlgorithms
{
    //Class acts as the interface to the database where experimental results are stored
    class ResultDB : DbContext
    {
        string path = "Data Source=" + Path.GetFullPath("../../../ExperimentResults/ResultsDB.db");
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(path);

        public virtual DbSet<Result> Results { get; set; }
    }
}
