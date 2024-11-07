using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using Myshop.Web.UnitOfWorks;
using Myshop.Web.ViewModels;
using SQLitePCL;

namespace Myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProduct _product;
        private readonly ICategory _category;
        public ProductController(IUnitOfWork unitOfWork, ICategory category,IProduct product,IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _product = product;
            _category = category;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var Products = await _unitOfWork.Product.GetProductsWithCategoryAsync();
            var mappedProducts = _mapper.Map<IEnumerable<ProductViewModel>>(Products);
            return View(Products);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var mappedProduct = _mapper.Map<ProductViewModel>(product);
            return View(mappedProduct); // Ensure this view expects a single ProductViewModel
        }
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            await LoadCategories();
            var model = new ProductViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel productVM, IFormFile file)
        {
            
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please upload a file.");
            }
            else
            {
                // Handle file upload
                string fileName = Guid.NewGuid().ToString();
                string fileExtension = Path.GetExtension(file.FileName);
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");

                // Ensure the upload directory exists
                Directory.CreateDirectory(uploadPath);

                string filePath = Path.Combine(uploadPath, fileName + fileExtension);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                //images/products/l1.jpg
               //images/product/l2.jpg
                productVM.PictureUrl = Path.Combine("images", "Products", fileName + fileExtension).Replace('\\', '/');
            }

            // Clear ModelState errors before re-validating
            ModelState.Clear();

            // Validate PictureUrl
            if (string.IsNullOrEmpty(productVM.PictureUrl))
            {
                ModelState.AddModelError(nameof(productVM.PictureUrl), "The PictureUrl field is required.");
            }

            // Manually validate productVM again
            TryValidateModel(productVM);

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(productVM);
            }

            var product = _mapper.Map<Product>(productVM);
            await _unitOfWork.Product.CreateAsync(product);

            TempData["toastrMessage"] = "Data has been created successfully";
            TempData["toastrType"] = "created"; // Set the type for Toastr

            return RedirectToAction(nameof(GetAllProducts));
        }

        private async Task LoadCategories()
        {
            var categories = await _unitOfWork.Category.GetCategoreisIdandNames();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _unitOfWork.Category.GetCategoreisIdandNames();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name");

            var productVM = _mapper.Map<ProductViewModel>(product);
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(ProductViewModel productVM, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please upload a file.");
            }
            else
            {
                // Handle file upload
                string fileName = Guid.NewGuid().ToString();
                string fileExtension = Path.GetExtension(file.FileName);
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");

                // Ensure the upload directory exists
                Directory.CreateDirectory(uploadPath);

                string filePath = Path.Combine(uploadPath, fileName + fileExtension);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                productVM.PictureUrl = Path.Combine("images", "Products", fileName + fileExtension).Replace('\\', '/');
            }

            // Clear ModelState errors before re-validating
            ModelState.Clear();

            // Validate PictureUrl
            if (string.IsNullOrEmpty(productVM.PictureUrl))
            {
                ModelState.AddModelError(nameof(productVM.PictureUrl), "The PictureUrl field is required.");
            }

            // Manually validate productVM again
            TryValidateModel(productVM);

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(productVM);
            }

            var product = _mapper.Map<Product>(productVM);
            var result = await _unitOfWork.Product.UpdateAsync(product);

            if (result == null)
            {
                ModelState.AddModelError("", "Update failed. Please try again.");
                var categories = await _unitOfWork.Category.GetCategoreisIdandNames();
                ViewData["Categories"] = new SelectList(categories, "Id", "Name");
                return View(productVM);
            }

            TempData["toastrMessage"] = "Data has been updated successfully";
            TempData["toastrType"] = "updated"; // Set the type for Toastr
            return RedirectToAction(nameof(GetAllProducts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "Invalid product ID.");
                return BadRequest(ModelState);
            }

            // Retrieve the product to get the PictureUrl
            var product = await _unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
            {
                ModelState.AddModelError("", "Product not found.");
                return NotFound();
            }

            // Extract the PictureUrl and determine the file path
            var pictureUrl = product.PictureUrl;
            if (!string.IsNullOrEmpty(pictureUrl))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, pictureUrl.Replace('/', '\\'));

                // Check if the file exists and delete it
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Delete the product from the database
            var success = await _unitOfWork.Product.DeleteAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Deletion failed. Please try again.");
                return BadRequest(ModelState);
            }

            TempData["toastrMessage"] = "Data has been deleted successfully";
            TempData["toastrType"] = "deleted"; // Set the type for Toastr
            return RedirectToAction(nameof(GetAllProducts));
        }

    }
}
