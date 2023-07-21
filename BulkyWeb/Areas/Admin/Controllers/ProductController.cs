using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
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
        public IActionResult Upsert(ProductVM obj, IFormFile? file) 
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //public IActionResult Edit(int? productID)
        //{
        //    if(productID == null || productID == 0)
        //    {
        //        return NotFound();
        //    }

        //    Product? productFromDb = _unitOfWork.Product.Get(temp=>temp.ProductID == productID);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(item => new SelectListItem
        //    {
        //        Text = item.Name,
        //        Value = item.Id.ToString()
        //    });
        //    ViewBag.CategoryList = CategoryList;
        //    return View(productFromDb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");
        //}

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
    }
}
