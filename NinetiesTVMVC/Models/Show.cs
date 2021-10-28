using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NinetiesTVMVC.Models
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int EpisodeCount { get; set; }
        public double ImdbRating { get; set; }

        public List<Genre> Genres { get; set; }

        [DisplayName("Genres")]
        public string GenreList
        {
            get
            {
                return String.Join(", ", Genres.Select(g => g.GenreName));
            }
        }
    }
}
