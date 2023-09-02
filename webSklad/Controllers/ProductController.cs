using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;
using webSklad.Models.ViewModels;
using webSklad.Repository;
using static System.Net.WebRequestMethods;

namespace webSklad.Controllers
{
    public class ProductController : Controller
    {
        /*
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly WebSkladContext _context;
        */
        
        private readonly ISRRepository _srRepository;
        private readonly IFopRepository _fopRepository;
        private readonly IUserRepository _userRepository;
        private readonly IShopPostInfoRepository _shopPostInfoRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        private const string ShopInfoType = "shopinfo";
        private const string PostInfoType = "postinfo";

        public ProductController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebSkladContext context,
           ISRRepository srRepositroy, IFopRepository fopRepository, IUserRepository userRepository, IShopPostInfoRepository shopPostInfoRepository, IInvoiceRepository invoiceRepository)
        {
            /*
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            */

            _srRepository = srRepositroy;
            _fopRepository = fopRepository;
            _userRepository = userRepository;
            _shopPostInfoRepository = shopPostInfoRepository;
            _invoiceRepository = invoiceRepository;
        }
        [HttpPost]
        public async Task<IActionResult> LoadShopFOPs(int shopInfoId)
        {
            var fops = await _fopRepository.LoadFopsForShopAsync(shopInfoId);
            return Json(new SelectList(fops, "Id", "NameOfFop"));
        }

        [HttpPost]
        public async Task<IActionResult> LoadFOPs(int shopPostInfoId)
        {
            var fops = await _fopRepository.LoadFopsForPostAsync(shopPostInfoId);
            return Json(new SelectList(fops, "Id", "NameOfFop"));
        }

        [HttpPost]
        public async Task<IActionResult> LoadSRsForPost(int srId)
        {
            var srs = await _srRepository.LoadSRsForPostAsync(srId);
            return Json(new SelectList(srs, "Id", "SRName"));
        }


        [HttpGet]
        public async Task<IActionResult> CreateInvoice()
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUserId = await _userRepository.GetCurrentUserIdAsync();

                var postInfos = await _shopPostInfoRepository.GetShopPostInfosByTypeAsync(currentUserId, PostInfoType);
                var shopInfos = await _shopPostInfoRepository.GetShopPostInfosByTypeAsync(currentUserId, ShopInfoType);


                var viewModel = new InvoiceViewModel
                {
                    PostInfos = new SelectList(postInfos, "Id", "Name"),
                    PostFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                    PostSR = new SelectList(new List<SalesRepresentative>(), "Id", "SRName"),

                    ShopInfos = new SelectList(shopInfos, "Id", "Name"),
                    ShopFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                };

                return View(viewModel);
            }
            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
            {
                var createdByUser = await _userRepository.GetCurrentUserCreatedByUserAsync();

                if (createdByUser == null)
                {
                    return NotFound();
                }

                var postInfos = await _shopPostInfoRepository.GetShopPostInfosByTypeAsync(createdByUser.Id, PostInfoType);
                var shopInfos = await _shopPostInfoRepository.GetShopPostInfosByTypeAsync(createdByUser.Id, ShopInfoType);

                
                var viewModel = new InvoiceViewModel
                {
                    PostInfos = new SelectList(postInfos, "Id", "Name"),
                    PostFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                    PostSR = new SelectList(new List<SalesRepresentative>(), "Id", "SRName"),

                    ShopInfos = new SelectList(postInfos, "Id", "Name"),
                    ShopFops = new SelectList(new List<Fop>(), "Id", "NameOfFop"),
                };

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(InvoiceViewModel viewModel)
        {

            if (User.IsInRole("ShopOwner"))
            {

                var currentUser = await _userRepository.GetCurrentyUser();


                if (currentUser == null)
                {
                    return NotFound();
                }

                var invoice = new Invoice
                {
                    Name = viewModel.Name,
                    DateTime = viewModel.DateTime,
                    DateTimeNow = DateTime.Now,
                    DatePostponement = viewModel.DatePostponement,
                    PostInfoId = viewModel.PostInfoId,
                    PostFopId = viewModel.PostFopId,
                    PostSRId = viewModel.PostSRId,
                    ShopInfoId = viewModel.ShopInfoId,
                    ShopFopId = viewModel.ShopFopId,
                    UserInvoiceId = currentUser.Id
                    
                    

                };

                await _invoiceRepository.UpdateInvoice(invoice);
                return RedirectToAction("InvoiceInfo");
            }

            else if (User.IsInRole("Accountant") && (User.IsInRole("Executor")))
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
                var invoice = new Invoice
                {
                    Name = viewModel.Name,
                    DateTime = viewModel.DateTime,
                    DateTimeNow = DateTime.Now,
                    DatePostponement = viewModel.DatePostponement,
                    PostInfoId = viewModel.PostInfoId,
                    PostFopId = viewModel.PostFopId,
                    PostSRId = viewModel.PostSRId,
                    ShopInfoId = viewModel.ShopInfoId,
                    ShopFopId = viewModel.ShopFopId,
                    UserInvoiceId = createdByUser.Id

                };
               
                await _invoiceRepository.UpdateInvoice(invoice);
                return RedirectToAction("InvoiceInfo");
            }
            return NotFound();
        }
    }
}