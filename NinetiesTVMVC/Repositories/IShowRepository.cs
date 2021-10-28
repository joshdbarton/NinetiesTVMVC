using NinetiesTVMVC.Models;
using System.Collections.Generic;

namespace NinetiesTVMVC.Repositories
{
    public interface IShowRepository
    {
        Show Get(int id);
        SearchShowsViewModel Get(string queryString, int genreId, int page = 1, int pageSize = 5);
        List<Genre> GetGenres();
    }
}