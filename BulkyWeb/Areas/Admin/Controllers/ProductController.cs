using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging.Signing;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(products);
        }

        public IActionResult Upsert(int? ProductID)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                Product = new Product()
            };
            if(ProductID == null || ProductID == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(temp=>temp.ProductID == ProductID);
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file) 
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = file.FileName;
                    string productPath = Path.Combine(wwwRootPath, @"Images/Product");

                    if (!string.IsNullOrWhiteSpace(obj.Product.ImgUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImgUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath)) { 
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create);
                        file.CopyTo(fileStream);
                    obj.Product.ImgUrl = @"Images/Product/" + fileName;
                }

                if(obj.Product.ProductID == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? productId)
        {
            if(productId == null || productId == 0)
            {
                return NotFound();
            }
            Product productFromDb = _unitOfWork.Product.Get(temp=>temp.ProductID == productId);
            if(productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? productId)
        {
            Product ProductFromDB = _unitOfWork.Product.Get(temp=>temp.ProductID == productId);
            if(ProductFromDB == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(ProductFromDB);
            _unitOfWork.Save(); 
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = products});
        }
    }
}
