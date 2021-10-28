using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NinetiesTVMVC.Models
{
    public class SearchShowsViewModel
    {
        public List<Show> Shows { get; set; }
        public int GenreId { get; set; }
        public string QueryString { get; set; }

        public int TotalPages { get; set; }

        public int Page { get; set; }
    }
}
