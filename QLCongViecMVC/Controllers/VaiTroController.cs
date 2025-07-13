using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCongViecMVC.Data;
using QLCongViecMVC.Models;
using QLCongViecMVC.Filters;
namespace QLCongViecMVC.Controllers
{
    [CheckLogin]
    public class VaiTroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VaiTroController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.VaiTros.ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VaiTro model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Guid.NewGuid().ToString("N")[..12];
                _context.VaiTros.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var entity = await _context.VaiTros.FindAsync(id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, VaiTro model)
        {
            if (id != model.ID) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.VaiTros.FindAsync(id);
            return entity == null ? NotFound() : View(entity);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entity = await _context.VaiTros.FindAsync(id);
            if (entity != null)
            {
                _context.VaiTros.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
