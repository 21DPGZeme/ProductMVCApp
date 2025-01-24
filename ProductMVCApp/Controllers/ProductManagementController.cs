using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMVCApp.Data;
using ProductMVCApp.Entities;
using ProductMVCApp.Models;
using System.Text.Json;

namespace ProductMVCApp.Controllers
{
    public class ProductManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly UserManager<IdentityUser> _userManager;

        public ProductManagementController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<ICollection<Product>, ICollection<ProductModel>>(await _context.Products.Where(p => !p.IsDeleted).ToListAsync()));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = _mapper.Map<ProductModel>(await _context.Products.FirstOrDefaultAsync(m => m.Id == id));
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            CreateViewModel model = new CreateViewModel { ProductCount = _context.Products.Count() };
            return View(model);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Quantity,Price")] ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                DateTime time = DateTime.Now;
                var currentUser = await _userManager.GetUserAsync(User);
                var newProduct = _mapper.Map<Product>(productModel);

                await _context.AddAsync(newProduct);
                await _context.SaveChangesAsync();

                ProjectAudit productAudit = new ProjectAudit() { ProductId = newProduct.Id,
                                                                 Property = ProductProperty.Product,
                                                                 TimeChanged = time,
                                                                 UserId = currentUser.Id,
                                                                 Value = "Created" };
                await _context.AddAsync(productAudit);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(productModel);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = _mapper.Map<ProductModel>(await _context.Products.FindAsync(id));
            if (productModel == null)
            {
                return NotFound();
            }
            return View(productModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Quantity,Price")] ProductModel productModel)
        {
            if (id != productModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime time = DateTime.Now;
                    var currentUser = await _userManager.GetUserAsync(User);
                    var oldEntity = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
                    var newEntity = _mapper.Map<Product>(productModel);

                    // Project audit
                    if (oldEntity.Title != newEntity.Title)
                    {
                        ProjectAudit productTitleAudit = new ProjectAudit() { ProductId = id, Property = ProductProperty.Title, TimeChanged = time, UserId = currentUser.Id, Value = newEntity.Title };
                        await _context.AddAsync(productTitleAudit);
                        await _context.SaveChangesAsync();
                    }
                    if (oldEntity.Quantity != newEntity.Quantity)
                    {
                        ProjectAudit productQuantityAudit = new ProjectAudit() { ProductId = id, Property = ProductProperty.Quantity, TimeChanged = time, UserId = currentUser.Id, Value = newEntity.Quantity.ToString() };
                        await _context.AddAsync(productQuantityAudit);
                        await _context.SaveChangesAsync();
                    }
                    if (oldEntity.Price != newEntity.Price)
                    {
                        ProjectAudit productPriceAudit = new ProjectAudit() { ProductId = id, Property = ProductProperty.Price, TimeChanged = time, UserId = currentUser.Id, Value = newEntity.Price.ToString() };
                        await _context.AddAsync(productPriceAudit);
                        await _context.SaveChangesAsync();
                    }

                    _context.Entry(oldEntity).State = EntityState.Detached;
                    _context.Update(newEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductModelExists(productModel.Id))
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
            return View(productModel);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = _mapper.Map<ProductModel>(await _context.Products.FirstOrDefaultAsync(m => m.Id == id));
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            DateTime time = DateTime.Now;
            var currentUser = await _userManager.GetUserAsync(User);
            var productToDelete = await _context.Products.FindAsync(id);
            productToDelete.IsDeleted = true;

            ProjectAudit productAudit = new ProjectAudit() { ProductId = id, Property = ProductProperty.Product, TimeChanged = time, UserId = currentUser.Id, Value = "Deleted" };
            await _context.AddAsync(productAudit);
            await _context.SaveChangesAsync();

            _context.Products.Update(productToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Audit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Audit(string fromDate, string toDate)
        {
            DateTime fromDateTime = DateTime.Parse(fromDate);
            DateTime toDateTime = DateTime.Parse(toDate);
            var filteredAudit = await _context.ProjectAudits.Where(dt => (DateTime.Compare(fromDateTime, dt.TimeChanged) <= 0 && DateTime.Compare(dt.TimeChanged, toDateTime) <= 0)).ToListAsync();
            foreach (var x in filteredAudit)
            {
                x.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == x.UserId);
                x.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == x.ProductId);
            }
            var json = JsonSerializer.Serialize(filteredAudit);
            return Content(json, "application/json");
        }

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
