using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _unitOfWork.Category.GetAll().ToList();
            return View(categoriesList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category categoryData)
        {
            if (categoryData.Name == categoryData.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Category Name and Display Order can't be same");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(categoryData);
                _unitOfWork.Save();
                TempData["Success"] = "Data Successfully Added!";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? CategoryId)
        {
            if (CategoryId == null || CategoryId == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == CategoryId);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category categoryData)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(categoryData);
                _unitOfWork.Save();
                TempData["Success"] = "Data Successfully Updated!";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Delete(int? CategoryId)
        {
            if (CategoryId == null || CategoryId == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == CategoryId);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? CategoryId)
        {
            Category? categoryData = _unitOfWork.Category.Get(u => u.Id == CategoryId);

            if (categoryData == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(categoryData);
            _unitOfWork.Save();
            TempData["Success"] = "Data Successfully Deleted!";
            return RedirectToAction("Index");
        }
    }
}
