using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateModels
{
    public class Estate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int OwnerId { get; set; }
        public double Price { get; set; }
        public EstateType Type { get; set; }
        public DateTime CreateDate { get; set; }

        public List<Picture> Pictures { get; set; }

        public Estate()
        {
            Pictures = new List<Picture>();
        }

    }
}