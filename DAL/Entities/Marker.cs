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
        public String X { get; set; }
        public String Y { get; set; }
        public MarkerType MarkerType { get; set; }
        public String Title { get; set; }
        public AudioStatus AudioStatus { get; set; }
    }
}
