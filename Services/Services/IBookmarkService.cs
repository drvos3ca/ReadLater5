using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IBookmarkService
    {
        Task<List<Bookmark>> Get();
        Task<Bookmark> Get(int id);
        Task<Bookmark> Create(Bookmark bookmark);
        Task<Bookmark> Update(Bookmark bookmark);
        Task<Bookmark> Delete(int id);
    }
}
