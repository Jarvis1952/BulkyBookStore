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

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> Companys = _unitOfWork.Company.GetAll().ToList();
            return View(Companys);
        }

        public IActionResult Upsert(int? CompanyID)
        {
            if(CompanyID == null || CompanyID == 0)
            {
                return View(new Company());
            }
            else
            {
                Company Company = _unitOfWork.Company.Get(temp=>temp.Id == CompanyID);
                return View(Company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj) 
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? CompanyId)
        {
            if(CompanyId == null || CompanyId == 0)
            {
                return NotFound();
            }
            Company CompanyFromDb = _unitOfWork.Company.Get(temp=>temp.Id == CompanyId);
            if(CompanyFromDb == null)
            {
                return NotFound();
            }
            return View(CompanyFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? CompanyId)
        {
            Company CompanyFromDB = _unitOfWork.Company.Get(temp=>temp.Id == CompanyId);
            if(CompanyFromDB == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(CompanyFromDB);
            _unitOfWork.Save(); 
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companys = _unitOfWork.Company.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = Companys});
        }
    }
}
