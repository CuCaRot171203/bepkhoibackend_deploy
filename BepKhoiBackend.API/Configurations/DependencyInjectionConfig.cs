using BepKhoiBackend.BusinessObject.Abstract.MenuBusinessAbstract;
using BepKhoiBackend.BusinessObject.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.BusinessObject.Abstract.UnitAbstract;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using BepKhoiBackend.BusinessObject.Services.MenuService;
using BepKhoiBackend.BusinessObject.Services.ProductCategoryService;
using BepKhoiBackend.BusinessObject.Services.UnitService;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.DataAccess.Abstract.UnitAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.LoginRepository;
using BepKhoiBackend.DataAccess.Repository.LoginRepository.Interface;
using BepKhoiBackend.DataAccess.Repository.ProductCategoryRepository;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository.Interface;
using BepKhoiBackend.DataAccess.Repository.RoomAreaRepository;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using BepKhoiBackend.DataAccess.Repository.RoomRepository;
using BepKhoiBackend.DataAccess.Repository.UnitRepository;

using Microsoft.EntityFrameworkCore;
using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using BepKhoiBackend.BusinessObject.Services.RoomAreaService;
using BepKhoiBackend.BusinessObject.Services.RoomService;
using BepKhoiBackend.BusinessObject.Services.InvoiceService;
using BepKhoiBackend.DataAccess.Repositories;
using BepKhoiBackend.DataAccess.Repository.InvoiceRepository;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using BepKhoiBackend.BusinessObject.Services.UserService.CashierService;
using BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository;
using BepKhoiBackend.BusinessObject.Services.UserService.ShipperService;
using BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository;
using BepKhoiBackend.BusinessObject.Services.CustomerService;
using BepKhoiBackend.DataAccess.Repository.CustomerRepository;
using BepKhoiBackend.DataAccess.Repository.UserRepository.ManagerRepository;
using BepKhoiBackend.BusinessObject.Services.UserService.ManagerService;
using BepKhoiBackend.DataAccess.Repository.TakeAwayOrderRepository;
using BepKhoiBackend.BusinessObject.Services.TakeAwayOrderService;
using BepKhoiBackend.DataAccess.Repository.ShippingOrderRepository;
using BepKhoiBackend.BusinessObject.Services.ShippingOrderService;
using BepKhoiBackend.BusinessObject.Services.OrderDetailService;
using BepKhoiBackend.BusinessObject.Services.OrderService;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Repository.OrderDetailRepository;
using BepKhoiBackend.DataAccess.Repository.OrderRepository;
using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.Abstract.OrderDetailAbstract;

namespace BepKhoiBackend.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<bepkhoiContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Register any Services and Repositories
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserRepository, UserRepository>();

            // RoomArea
            services.AddScoped<IRoomAreaRepository, RoomAreaRepository>();
            services.AddScoped<IRoomAreaService, RoomAreaService>();

            //room 
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomService, RoomService>();


            //Dependency Injection for Services and Repositories

            // Menu Repositories and Services
            services.AddScoped<IMenuRepository, MenuRepository>(); // Repository
            services.AddScoped<IMenuService, MenuService>();       // Service

            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IUnitService, UnitService>();

            // Register services for Manager
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<IManagerService, ManagerService>();


            // Register services for ShippingOrder
            services.AddScoped<IShippingOrderRepository, ShippingOrderRepository>();
            services.AddScoped<IShippingOrderService, ShippingOrderService>();


            // Register services for TakeAwayOrder
            services.AddScoped<ITakeAwayOrderRepository, TakeAwayOrderRepository>();
            services.AddScoped<ITakeAwayOrderService, TakeAwayOrderService>();

            // Register services for Invoices
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceService, InvoiceService>();

            // Register for cashier
            services.AddScoped<ICashierRepository, CashierRepository>();
            services.AddScoped<ICashierService, CashierService>();

            // Register to shipper
            services.AddScoped<IShipperRepository, ShipperRepository>();
            services.AddScoped<IShipperService, ShipperService>();

            // Register for customer
            services.AddScoped<ICustomerRepository, CustomerRepository>(); // Repository cho Customer
            services.AddScoped<ICustomerService, CustomerService>();

            // Register for Order
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            //Register for order detail
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();

            // Register CloudinaryService and QRCodeService
            services.AddSingleton<CloudinaryService>();
            services.AddScoped<QRCodeService>();
            //pdf print
            services.AddScoped<PrintInvoicePdfService>();
            services.AddScoped<PrintOrderPdfService>();
            //VnPay
            services.AddScoped<VnPayService>();
        }
    }
}
