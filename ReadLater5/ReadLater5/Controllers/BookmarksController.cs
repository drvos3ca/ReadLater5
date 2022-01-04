using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Threading.Tasks;

namespace ReadLater5.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ICategoryService _categoryService;

        public BookmarksController(IBookmarkService bookmarkService, ICategoryService categoryService)
        {
            _bookmarkService = bookmarkService;
            _categoryService = categoryService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var bookmarks = await _bookmarkService.Get();
            return View(bookmarks);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var bookmark = await _bookmarkService.Get(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        [Authorize]
        public IActionResult Create()
        {

            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "ID", "ID");
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,URL,ShortDescription,CategoryId")] Bookmark bookmark)
        {
            if (ModelState.IsValid)
            {
                bookmark.CreateDate = DateTime.UtcNow;

                await _bookmarkService.Create(bookmark);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "ID", "ID", bookmark.CategoryId);
            return View(bookmark);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var bookmark = await _bookmarkService.Get(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "ID", "ID", bookmark.CategoryId);
            return View(bookmark);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("ID,URL,ShortDescription,CategoryId")] Bookmark bookmark)
        {
            if (id != bookmark.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updatedBookmark = await _bookmarkService.Update(bookmark);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BookmarkExists(bookmark.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "ID", "ID", bookmark.CategoryId);
            return View(bookmark);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var bookmark = await _bookmarkService.Get(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deletedBookmark = await _bookmarkService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BookmarkExists(int id)
        {
            return await _bookmarkService.Get(id) != null;
        }
    }
}
