using Data;
using Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly ReadLaterDataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid _userId;

        public BookmarkService(ReadLaterDataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public async Task<Bookmark> Create(Bookmark bookmark)
        {
            bookmark.UserId = _userId;
            var newBookmark = await _context.Bookmark.AddAsync(bookmark);

            // ToDo: Check if the bookmark has been successfully created (EntityState), if not then return an object indicating the failure

            await _context.SaveChangesAsync();
            return newBookmark.Entity;
        }

        public async Task<Bookmark> Delete(int id)
        {
            var bookmarkForDeletion = await _context.Bookmark.FirstOrDefaultAsync(m => m.ID == id && m.UserId == _userId);

            // ToDo: Check if the bookmark for deletion exists, if not then return an object indicating that the bookmark does not exist

            var deletedBookmark = _context.Bookmark.Remove(bookmarkForDeletion);

            // ToDo: Check if the bookmark has been successfully deleted (EntityState), if not then return an object indicating that the bookmark has not been deleted

            await _context.SaveChangesAsync();
            return deletedBookmark.Entity;
        }

        public async Task<List<Bookmark>> Get()
        {
            return await _context.Bookmark.Include(b => b.Category).Where(b => b.UserId == _userId).ToListAsync();
        }

        public async Task<Bookmark> Get(int id)
        {
            var bookmark = await _context.Bookmark.Include(b => b.Category)
                                                  .FirstOrDefaultAsync(m => m.ID == id && m.UserId == _userId);

            // ToDo: Check if the bookmark is default(Bookmark), if that is true, return an object indicating that the bookmark does not exist for the given ID

            return bookmark;
        }

        public async Task<Bookmark> Update(Bookmark bookmark)
        {
            var updatedBookmark = _context.Update(bookmark);

            // ToDo: Check if the bookmark has been successfully updated, if not, then return an object indicating that the update was unsuccessful

            await _context.SaveChangesAsync();
            return updatedBookmark.Entity;
        }
    }
}
