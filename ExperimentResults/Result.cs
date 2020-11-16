using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RandomizerAlgorithms
{
    //Used to store the results of each experiment within a SQLite database
    [Table("Results")]
    class Result
    {
        [Key]
        public int PK { get; set; }
        public string Algorithm { get; set; }
        public string World { get; set; }
        public bool Completable { get; set; }
        public double ExecutionTime { get; set; }
        public double Bias { get; set; }
        public bool BiasDirection { get; set; }
        public double Interestingness { get; set; }

    }
}
