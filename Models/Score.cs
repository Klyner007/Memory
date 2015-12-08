using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory.Models
{
    class Score
    {
        public int playerOneScore { get; set; }
        public int playerTwoScore { get; set; }

        public string PlayerOneScore => String.Format("{0}", playerOneScore);
        public string PlayerTwoScore => String.Format("{0}", playerTwoScore);
    }
}
