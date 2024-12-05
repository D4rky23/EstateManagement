using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateModels
{
    public class Picture
    {
        public string FilePath { get; set; }
        public int Id { get; set; }
        public int EstateId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public long Size { get; set; }

        public Estate Estate { get; set; }
    }
}
