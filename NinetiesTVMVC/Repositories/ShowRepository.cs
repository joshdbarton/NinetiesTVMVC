using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NinetiesTVMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NinetiesTVMVC.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly IConfiguration _config;

        public ShowRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public SearchShowsViewModel Get(string queryString, int genreId, int page = 1, int pageSize = 5)
        {
            var shows = new List<Show>();
            int totalCount = 0;

            //determine how many results to skip
            var offset = (page - 1) * pageSize;
            
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @$"SELECT pageOfShows.*, g.GenreName, g.Id GenreId FROM 
                                (SELECT s.*, TotalCount = COUNT(*) OVER()
                                FROM Show s
                                {(genreId > 0 ? @"
                                                 JOIN ShowGenre sg ON sg.ShowId = s.Id
                                                 JOIN Genre g ON g.Id = sg.GenreId"
                                              : "")}
                                WHERE s.Name LIKE '%' + @queryString + '%'
                                {(genreId > 0 ? "AND g.Id = @genreId" : "")}
                                ORDER BY s.Name
                                OFFSET @offset ROWS
                                FETCH NEXT @pageSize ROWS ONLY) pageOfShows
                                LEFT JOIN ShowGenre sg ON sg.ShowId = pageOfShows.Id
                                LEFT JOIN Genre g ON g.Id = sg.GenreId";
            cmd.Parameters.AddWithValue("@queryString", String.IsNullOrWhiteSpace(queryString) ? "" : queryString);
            cmd.Parameters.AddWithValue("@offset", offset);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);

            if (genreId > 0)
            {
                cmd.Parameters.AddWithValue("@genreId", genreId);
            }
            
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                }

                var existingShow = shows.FirstOrDefault(s => s.Id == reader.GetInt32(reader.GetOrdinal("Id")));
                if (existingShow == null)
                {
                    existingShow = new Show
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        StartYear = reader.GetInt16(reader.GetOrdinal("StartYear")),
                        EndYear = reader.GetInt16(reader.GetOrdinal("EndYear")),
                        EpisodeCount = reader.GetInt16(reader.GetOrdinal("EpisodeCount")),
                        ImdbRating = reader.GetDouble(reader.GetOrdinal("ImdbRating")),
                        Genres = new List<Genre>()
                    };

                    shows.Add(existingShow);

                    
                }

                if (!reader.IsDBNull(reader.GetOrdinal("GenreId")))
                {
                    existingShow.Genres.Add(new Genre
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("GenreId")),
                        GenreName = reader.GetString(reader.GetOrdinal("GenreName"))
                    });
                }
            }

            // if the total doesn't evenly divide into the page size, or there are no records
            // add a page to hold the remainder or a single page with no records if the query 
            // returns nothing.
            var totalPages = totalCount / pageSize;
            if (totalCount % pageSize != 0 || totalCount == 0)
            {
                totalPages += 1;
            }

            // if the currently selected page is now greater than the total number of pages
            // (because records have been deleted since the page was last loaded) 
            // reset the page to be the greatest possible page and rerun the query
            // (this is an example of recursion)
            if (page > totalPages)
            {
                return Get(queryString, genreId, totalPages, pageSize);
            }

            return new SearchShowsViewModel
            {
                Shows = shows,
                TotalPages = totalPages,
                GenreId = genreId,
                QueryString = queryString,
                Page = page,
            };
        }

        public Show Get(int id)
        {
            Show show = null;
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select s.*, g.Id GenreId, g.GenreName
                                FROM Show s 
                                LEFT JOIN ShowGenre sg ON sg.ShowId = s.Id
                                LEFT JOIN Genre g ON g.Id = sg.GenreId
                                WHERE s.Id = @id";

            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (show == null)
                {
                    show = new Show
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        StartYear = reader.GetInt16(reader.GetOrdinal("StartYear")),
                        EndYear = reader.GetInt16(reader.GetOrdinal("EndYear")),
                        EpisodeCount = reader.GetInt16(reader.GetOrdinal("EpisodeCount")),
                        ImdbRating = reader.GetDouble(reader.GetOrdinal("ImdbRating")),
                        Genres = new List<Genre>()
                    };

                }

                if (!reader.IsDBNull(reader.GetOrdinal("GenreId")))
                {
                    show.Genres.Add(new Genre
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("GenreId")),
                        GenreName = reader.GetString(reader.GetOrdinal("GenreName"))
                    });
                }
            }

            return show;
        }

        public List<Genre> GetGenres()
        {
            var genres = new List<Genre>();

            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Genre";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                genres.Add(new Genre
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    GenreName = reader.GetString(reader.GetOrdinal("GenreName"))
                });
            }

            return genres;
        }
    }
}
