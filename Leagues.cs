using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonProject
{
    class Leagues
    {
        public string league_name { get; set; }   
        public int league_id { get; set; }
        public string team_name { get; set; }
        public int overall_league_position { get; set; }
        public int overall_league_PTS { get; set; }
        public int overall_league_GF { get; set; }       
        public int overall_league_GA { get; set; }
    }
}
