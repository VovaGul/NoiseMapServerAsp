using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Marker
    {
        public int Id { get; set; }
        public Coordinate Coordinate { get; set; }
        public MarkerType MarkerType { get; set; }
        public String Title { get; set; }
    }
}
