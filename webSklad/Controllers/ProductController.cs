using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        private readonly ISRRepository _srRepository;
        private readonly IFopRepository _fopRepository;
        private readonly IUserRepository _userRepository;
        private readonly IShopPostInfoRepository _shopPostInfoRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProductRepository _productRepository;

        private const string ShopInfoType = "shopinfo";
        private const string PostInfoType = "postinfo";

        public ProductController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, WebSkladContext context,
           ISRRepository srRepositroy, IFopRepository fopRepository, IUserRepository userRepository, IShopPostInfoRepository shopPostInfoRepository, IInvoiceRepository invoiceRepository, IProductRepository productRepository)
        {

            _srRepository = srRepositroy;
            _fopRepository = fopRepository;
            _userRepository = userRepository;
            _shopPostInfoRepository = shopPostInfoRepository;
            _invoiceRepository = invoiceRepository;
            _productRepository = productRepository;
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

        [HttpGet]
        public async Task<IActionResult> InvoiceInfo(string? sku, int? invoiceId)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentUserIdAsync();

                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser);
                }

                if (invoice == null)
                {
                    return NotFound();
                }
                invoice.Products = userProducts;

                return View(invoice);
            }

            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
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

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == createdByUser.Id)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser.Id);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser.Id);
                }

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.Products = userProducts;
                return View(invoice);
            }

            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string productCode, string productName, string barcode, string barcodeOwn, decimal productIncomingPrice, decimal productPrice, decimal productPriceOne, decimal productPriceTwo, decimal productPriceThree)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentyUser();


                if (currentUser == null)
                {
                    return NotFound();
                }

                var product = new Product
                {
                    Sku = productCode,
                    ProductName = productName,
                    IncomingPrice = productIncomingPrice,
                    Price = productPrice,
                    UserProductId = currentUser.Id,
                    User = currentUser,
                    BarCode = barcode,
                    BarCodeOwn = GenerateUniqueBarcodeOwn(barcodeOwn)
                };

                await _productRepository.CreateProduct(product);

                return Ok();
            }
            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
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

                var product = new Product
                {
                    Sku = productCode,
                    ProductName = productName,
                    IncomingPrice = productIncomingPrice,
                    Price = productPrice,
                    UserProductId = createdByUser.Id,
                    User = createdByUser,
                    BarCode = barcode,
                    BarCodeOwn = GenerateUniqueBarcodeOwn(barcodeOwn)
                };

                await _productRepository.CreateProduct(product);

                return Ok();

            }
            return NotFound();

        }

        private string GenerateRandomBarcodeOwn()
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 13)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

      
        private string GenerateUniqueBarcodeOwn(string barcodeOwn)
        {
           
            while (_productRepository.AnyProductWithBarcodeOwn(barcodeOwn))
            {
                barcodeOwn = GenerateRandomBarcodeOwn();
            }

            return barcodeOwn;
        }

        [HttpPost]
        public async Task<IActionResult> FilterProducts(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_ProductList", filteredProducts);
        }

        [HttpPost]
        public async Task<IActionResult> FilterProductsInInventory(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_InventoryList", filteredProducts);
        }

        [HttpPost]
        public async Task<IActionResult> FilterProductsInCkeck(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_CkeckList", filteredProducts);
        }

        [HttpPost]
        public async Task<IActionResult> FilterProductsInWritingProduct(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_WrittingOffList", filteredProducts);
        }

        [HttpPost]
        public async Task<IActionResult> FilterCreatingAnOrder(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_CreateAnOrderList", filteredProducts);
        }
        [HttpPost]
        public async Task<IActionResult> FilterScore(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_ScoreList", filteredProducts);
        }
        [HttpPost]
        public async Task<IActionResult> FilterSalesInvoice(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_SalesInvoiceList", filteredProducts);
        }
        [HttpPost]
        public async Task<IActionResult> FilterReturnToSupplier(string productCode, string productName, string barcode, string barcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentyUser();
            if (currentUser == null)
            {
                return NotFound();
            }
            var filteredProducts = _productRepository.FilterProducts(currentUser.Id, productCode, productName, barcodeOwn, barcode);
            return PartialView("_ReturnToSupplierList", filteredProducts);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
           

            var currentUser = await _userRepository.GetCurrentUserIdAsync();
            if (currentUser == null)
            {
                return NotFound();
            }



            var product = await _productRepository.GetProductById(productId);


            if (product == null || product.UserProductId != currentUser)
            {
                return NotFound();
            }

            await _productRepository.DeleteProduct(product);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int productId, string productCode, string productName, decimal productIncomingPrice, decimal productPrice, decimal productPriceOne, decimal productPriceTwo, decimal productPriceThree, string productBarcode, string productBarcodeOwn)
        {
            var currentUser = await _userRepository.GetCurrentUserIdAsync();

            if (currentUser == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetProductById(productId);

            if (product == null || product.UserProductId != currentUser)
            {
                return NotFound();
            }

            product.Sku = productCode;
            product.ProductName = productName;
            product.IncomingPrice = productIncomingPrice;
            product.Price = productPrice;
            product.BarCode = productBarcode;
            product.BarCodeOwn = productBarcodeOwn;


            await _productRepository.SaveAsync();

            return Ok();
        }

       




        [HttpGet]
        public async Task<IActionResult> CreateSalesInvoice()
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
        public async Task<IActionResult> CreateSalesInvoice(InvoiceViewModel viewModel)
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
                return RedirectToAction("SalesInvoiceInfo");


            }

            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
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
                return RedirectToAction("SalesInvoiceInfo");
            }


            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CreateScore()
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
        public async Task<IActionResult> CreateScore(InvoiceViewModel viewModel)
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



                return RedirectToAction("ScoreInfo");

            }

            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
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
                return RedirectToAction("ScoreInfo");

            }


            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CreateReturnToSupplier()
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

        // POST: /Product/CreateInvoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReturnToSupplier(InvoiceViewModel viewModel)
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
                return RedirectToAction("ReturnToSupplierInfo");
            }

            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
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
                return RedirectToAction("ReturnToSupplierInfo");

            }


            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CreateAnOrder()
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
        public async Task<IActionResult> CreateAnOrder(InvoiceViewModel viewModel)
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
                return RedirectToAction("CreateAnOrderInfo");

            }

            else if (User.IsInRole("Accountant") || User.IsInRole("Executor"))
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
                return RedirectToAction("CreateAnOrderInfo");
            }


            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> SalesInvoiceInfo(string? sku, int? invoiceId)
        {
            if (User.IsInRole("ShopOwner"))
            {

                var currentUser = await _userRepository.GetCurrentUserIdAsync();

                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser);
                }

                if (invoice == null)
                {
                    return NotFound();
                }
                invoice.Products = userProducts;

                return View(invoice);
            }

            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
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

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == createdByUser.Id)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser.Id);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser.Id);
                }

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.Products = userProducts;
                return View(invoice);
            }



            return NotFound();

        }

        [HttpGet]
        public async Task<IActionResult> ScoreInfo(string? sku, int? invoiceId)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentUserIdAsync();

                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser);
                }

                if (invoice == null)
                {
                    return NotFound();
                }
                invoice.Products = userProducts;

                return View(invoice);
            }

            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
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

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == createdByUser.Id)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser.Id);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser.Id);
                }

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.Products = userProducts;
                return View(invoice);
            }



            return NotFound();

        }


        [HttpGet]
        public async Task<IActionResult> ReturnToSupplierInfo(string? sku, int? invoiceId)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentUserIdAsync();

                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser);
                }

                if (invoice == null)
                {
                    return NotFound();
                }
                invoice.Products = userProducts;

                return View(invoice);
            }

            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
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

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == createdByUser.Id)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser.Id);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser.Id);
                }

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.Products = userProducts;
                return View(invoice);
            }



            return NotFound();

        }

        [HttpGet]
        public async Task<IActionResult> CreateAnOrderinfo(string? sku, int? invoiceId)
        {
            if (User.IsInRole("ShopOwner"))
            {
                var currentUser = await _userRepository.GetCurrentUserIdAsync();

                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == currentUser)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser);
                }

                if (invoice == null)
                {
                    return NotFound();
                }
                invoice.Products = userProducts;

                return View(invoice);
            }

            else if (User.IsInRole("Accountant") || (User.IsInRole("Executor")))
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

                var userProducts = _productRepository.GetProducts()
                    .Where(p => p.UserProductId == createdByUser.Id)
                    .ToList();

                Invoice invoice;

                if (invoiceId.HasValue)
                {
                    invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId.Value, currentUser.Id);
                }
                else
                {
                    invoice = await _invoiceRepository.GetInvoiceOrderByIdAsync(currentUser.Id);
                }

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.Products = userProducts;
                return View(invoice);
            }



            return NotFound();

        }


      



    }
}

