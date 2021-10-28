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

        [DisplayName("Start Year")]
        public int StartYear { get; set; }
        
        [DisplayName("End Year")]
        public int EndYear { get; set; }

        [DisplayName("Episode Count")]
        public int EpisodeCount { get; set; }

        [DisplayName("IMDb Rating")]
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
