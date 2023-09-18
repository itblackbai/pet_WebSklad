using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Extensions;
using webSklad.Interfaces;
using webSklad.Migrations;
using webSklad.Models;
using webSklad.Models.ViewModels;
using System.Data;


namespace webSklad.Controllers
{
    public class CartRegisterController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICartRegisterRepository _cartRegisterRepository;




        private readonly WebSkladContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public CartRegisterController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebSkladContext context,
            IProductRepository productRepository, IUserRepository userRepository, ICartRepository cartRepository, IInvoiceRepository invoiceRepository, ICartRegisterRepository cartRegisterRepository)
        {


             _productRepository = productRepository;
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _invoiceRepository = invoiceRepository;
            _cartRegisterRepository = cartRegisterRepository;



            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        



        [Authorize(Roles = "admin, ShopOwner, Cashier, Executor")]
        [HttpGet]
        public async Task<IActionResult> ShowAllProductCartRegister(string? sku, int? cartregisterid)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }


                var createCartRegister = new CartRegister
                {
                    CartRegisterDateTime = DateTime.Now,
                    UserProductId = currentUser.Id,

                };

                await _cartRegisterRepository.AddCartRegisterCart(createCartRegister);

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser.Id)
                    .ToList();
                CartRegister cartRegister;

                if (cartregisterid.HasValue)
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterById(cartregisterid.Value, currentUser.Id);
                }
                else
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterOrdering(currentUser.Id);

                }
                if (cartRegister == null)
                {
                    return NotFound();
                }
                cartRegister.Products = userProducts;
                return View(cartRegister);
            }
            else if (User.IsInRole("Cashier") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var createCartRegister = new CartRegister
                {
                    CartRegisterDateTime = DateTime.Now,
                    UserProductId = createdByUser.Id,

                };
                await _cartRegisterRepository.AddCartRegisterCart(createCartRegister);

                var userProducts = _productRepository.GetProducts()
                   .Where(p => p.UserProductId == createdByUser.Id)
                   .ToList();

                CartRegister cartRegister;
                if (cartregisterid.HasValue)
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterById(cartregisterid.Value, currentUser.Id);

                }
                else
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterOrdering(currentUser.Id);

                }
                if (cartRegister == null)
                {
                    return NotFound();
                }
                cartRegister.Products = userProducts;
                return View(cartRegister);
            }

            return NotFound();
        }


        [Authorize(Roles = "admin, ShopOwner, Cashier, Executor")]
        [HttpGet]
        public async Task<IActionResult> WriteOffTheProduct(string? sku, int? cartregisterid)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createCartRegister = new CartRegister
                {
                    CartRegisterDateTime = DateTime.Now,
                    UserProductId = currentUser.Id,

                };
                await _cartRegisterRepository.AddCartRegisterCart(createCartRegister);

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser.Id)
                    .ToList();
                CartRegister cartRegister;
                if (cartregisterid.HasValue)
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterById(cartregisterid.Value, currentUser.Id);

                }
                else
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterOrdering(currentUser.Id);


                }
                if (cartRegister == null)
                {
                    return NotFound();
                }
                cartRegister.Products = userProducts;
                return View(cartRegister);
            }

            else if (User.IsInRole("Cashier") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var createCartRegister = new CartRegister
                {
                    CartRegisterDateTime = DateTime.Now,
                    UserProductId = createdByUser.Id,

                };
                await _cartRegisterRepository.AddCartRegisterCart(createCartRegister);
  
                var userProducts = _productRepository.GetProducts()
                   .Where(p => p.UserProductId == createdByUser.Id)
                   .ToList();

                CartRegister cartRegister;
                if (cartregisterid.HasValue)
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterById(cartregisterid.Value, currentUser.Id);

                }
                else
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterOrdering(currentUser.Id);

                }
                if (cartRegister == null)
                {
                    return NotFound();
                }
                cartRegister.Products = userProducts;
                return View(cartRegister);
            }

            return NotFound();
        }

        [Authorize(Roles = "admin, ShopOwner, Cashier, Executor")]
        [HttpGet]
        public async Task<IActionResult> Inventory(string? sku, int? cartregisterid)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createCartRegister = new CartRegister
                {
                    CartRegisterDateTime = DateTime.Now,
                    UserProductId = currentUser.Id,

                };
                await _cartRegisterRepository.AddCartRegisterCart(createCartRegister);

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser.Id)
                    .ToList();

                CartRegister cartRegister;

                if (cartregisterid.HasValue)
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterById(cartregisterid.Value, currentUser.Id);

                }
                else
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterOrdering(currentUser.Id);

                }
                if (cartRegister == null)
                {
                    return NotFound();
                }
                cartRegister.Products = userProducts;

                return View(cartRegister);
            }

            else if (User.IsInRole("Cashier") || (User.IsInRole("Executor")))
            {
                var currentUser = await _userRepository.GetCurrentyUser();
                if (currentUser == null)
                {
                    return NotFound();
                }
                var createdByUserId = currentUser.CreatedByUserId;
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();
                if (createdByUser == null)
                {
                    return NotFound();
                }
                var createCartRegister = new CartRegister
                {
                    CartRegisterDateTime = DateTime.Now,
                    UserProductId = createdByUser.Id,

                };
                await _cartRegisterRepository.AddCartRegisterCart(createCartRegister);

                var userProducts = _productRepository.GetProducts()
                   .Where(p => p.UserProductId == createdByUser.Id)
                   .ToList();

                CartRegister cartRegister;
                if (cartregisterid.HasValue)
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterById(cartregisterid.Value, currentUser.Id);

                }
                else
                {
                    cartRegister = await _cartRegisterRepository.GetCartRegisterOrdering(currentUser.Id);
                }
                if (cartRegister == null)
                {
                    return NotFound();
                }
                cartRegister.Products = userProducts;
                return View(cartRegister);
            }

            return NotFound();

        }











    }
}