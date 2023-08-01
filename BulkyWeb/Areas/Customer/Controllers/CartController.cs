using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM shoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product")
            };
            foreach(var product in shoppingCartVM.ShoppingCartList)
            {
                product.Price = GetPriceBasedOnQuantity(product);
                shoppingCartVM.orderTotal += product.Price * product.Count;
            }
            return View(shoppingCartVM);    
        }

        public IActionResult Summary()
        {
            return View("Summary");
        }

        public IActionResult plus(int CartId)
        {
            var CartFromDb = _unitOfWork.ShoppingCart.Get(item=>item.Id == CartId);
            CartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(CartFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int CartId)
        {
            var CartFromDb = _unitOfWork.ShoppingCart.Get(item => item.Id == CartId);
            if(CartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(CartFromDb);
            }
            else
            {
                CartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(CartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int CartId)
        {
            var CartFromDb = _unitOfWork.ShoppingCart.Get(item => item.Id == CartId);
            _unitOfWork.ShoppingCart.Remove(CartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if(shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
