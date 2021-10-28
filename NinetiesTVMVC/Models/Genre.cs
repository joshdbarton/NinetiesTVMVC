using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NinetiesTVMVC.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string GenreName { get; set; }

        public List<Show> Shows { get; set; }
    }
}
