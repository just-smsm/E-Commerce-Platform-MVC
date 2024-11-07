using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Models;
using Myshop.Web.UnitOfWorks;
using Myshop.Web.ViewModels;
using Newtonsoft.Json;

namespace Myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            var mappedCategories = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return View(mappedCategories);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var mappedCategory = _mapper.Map<CategoryViewModel>(category);
            return View(mappedCategory);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            var categories = await _unitOfWork.Category.GetCategoreisIdandNames();
            TempData["categories"] = JsonConvert.SerializeObject(categories);
            return View();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            var category = _mapper.Map<Category>(categoryVM);
            await _unitOfWork.Category.CreateAsync(category);
            TempData["toastrMessage"] = "Data has been created successfully";
            TempData["toastrType"] = "created"; // Set the type for Toastr
            return RedirectToAction(nameof(GetAllCategory));
        }


        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = _mapper.Map<CategoryViewModel>(category);
            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(CategoryViewModel categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            var category = _mapper.Map<Category>(categoryVM);
            var result = await _unitOfWork.Category.UpdatedWithCreatedDateAsync(category);

            if (result == null)
            {
                ModelState.AddModelError("", "Update failed. Please try again.");
                return View(categoryVM);
            }

            TempData["toastrMessage"] = "Data has been updated successfully";
            TempData["toastrType"] = "updated"; // Set the type for Toastr
            return RedirectToAction(nameof(GetAllCategory));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "Invalid category ID.");
                return BadRequest(ModelState);
            }

            var success = await _unitOfWork.Category.DeleteAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Deletion failed. Please try again.");
                return BadRequest(ModelState);
            }

            TempData["toastrMessage"] = "Data has been deleted successfully";
            TempData["toastrType"] = "deleted"; // Set the type for Toastr
            return RedirectToAction(nameof(GetAllCategory));
        }
    }
}
