using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class bepkhoiContext : DbContext
    {
        public bepkhoiContext()
        {
        }

        public bepkhoiContext(DbContextOptions<bepkhoiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<DeliveryInformation> DeliveryInformations { get; set; } = null!;
        public virtual DbSet<DiscountCampaign> DiscountCampaigns { get; set; } = null!;
        public virtual DbSet<DiscountCampaignDetail> DiscountCampaignDetails { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderCancellationHistory> OrderCancellationHistories { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<OrderType> OrderTypes { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomArea> RoomAreas { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserInformation> UserInformations { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<VatTax> VatTaxes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.Phone, "UQ__Customer__5C7E359EBDB426EF")
                    .IsUnique();

                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .HasColumnName("Customer_name");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<DeliveryInformation>(entity =>
            {
                entity.ToTable("Delivery_information");

                entity.Property(e => e.DeliveryInformationId).HasColumnName("Delivery_information_id");

                entity.Property(e => e.DeliveryNote)
                    .HasMaxLength(255)
                    .HasColumnName("Delivery_note");

                entity.Property(e => e.ReceiverAddress)
                    .HasMaxLength(255)
                    .HasColumnName("Receiver_address");

                entity.Property(e => e.ReceiverName)
                    .HasMaxLength(100)
                    .HasColumnName("Receiver_name");

                entity.Property(e => e.ReceiverPhone)
                    .HasMaxLength(20)
                    .HasColumnName("Receiver_phone");
            });

            modelBuilder.Entity<DiscountCampaign>(entity =>
            {
                entity.HasKey(e => e.DiscountId)
                    .HasName("PK__Discount__63D7679C5F1F0A62");

                entity.ToTable("Discount_campaign");

                entity.Property(e => e.DiscountId).HasColumnName("Discount_id");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.DiscountByPercentage).HasColumnName("Discount_by_percentage");

                entity.Property(e => e.DiscountTitle)
                    .HasMaxLength(100)
                    .HasColumnName("Discount_title");

                entity.Property(e => e.DiscountValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Discount_value");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("End_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("Is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("Start_date");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DiscountCampaignDetail>(entity =>
            {
                entity.HasKey(e => e.DiscountDetailId)
                    .HasName("PK__Discount__62F0B98E449D8AF2");

                entity.ToTable("Discount_campaign_detail");

                entity.Property(e => e.DiscountDetailId).HasColumnName("Discount_detail_id");

                entity.Property(e => e.DiscountId).HasColumnName("Discount_id");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.DiscountCampaignDetails)
                    .HasForeignKey(d => d.DiscountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount___Disco__7A672E12");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DiscountCampaignDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Discount___Produ__7B5B524B");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice");

                entity.Property(e => e.InvoiceId).HasColumnName("Invoice_id");

                entity.Property(e => e.AmountDue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Amount_due");

                entity.Property(e => e.CashierId).HasColumnName("Cashier_id");

                entity.Property(e => e.CheckInTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Check_in_time");

                entity.Property(e => e.CheckOutTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Check_out_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

                entity.Property(e => e.InvoiceDiscount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Invoice_discount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.InvoiceNote)
                    .HasMaxLength(255)
                    .HasColumnName("Invoice_note");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.OrderTypeId).HasColumnName("Order_type_id");

                entity.Property(e => e.OtherPayment)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Other_payment")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentMethodId).HasColumnName("Payment_method_id");

                entity.Property(e => e.RoomId).HasColumnName("Room_id");

                entity.Property(e => e.ShipperId).HasColumnName("Shipper_id");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalQuantity).HasColumnName("Total_quantity");

                entity.Property(e => e.TotalVat)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Total_vat")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Cashier)
                    .WithMany(p => p.InvoiceCashiers)
                    .HasForeignKey(d => d.CashierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__Cashier__1CBC4616");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Invoice__Custome__1EA48E88");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__Order_i__208CD6FA");

                entity.HasOne(d => d.OrderType)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.OrderTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__Order_t__1BC821DD");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__Payment__1AD3FDA4");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Invoice__Room_id__1F98B2C1");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.InvoiceShippers)
                    .HasForeignKey(d => d.ShipperId)
                    .HasConstraintName("FK__Invoice__Shipper__1DB06A4F");
            });

            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.ToTable("Invoice_detail");

                entity.Property(e => e.InvoiceDetailId).HasColumnName("Invoice_detail_id");

                entity.Property(e => e.InvoiceId).HasColumnName("Invoice_id");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("Product_name");

                entity.Property(e => e.ProductNote)
                    .HasMaxLength(255)
                    .HasColumnName("Product_note");

                entity.Property(e => e.ProductVat)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("Product_vat");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceDetails)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice_d__Invoi__236943A5");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InvoiceDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice_d__Produ__03F0984C");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK__Menu__9833FF92C4E70A99");

                entity.ToTable("Menu");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.CostPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Cost_price");

                entity.Property(e => e.IsAvailable).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductCategoryId).HasColumnName("Product_category_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("Product_name");

                entity.Property(e => e.ProductVat)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("Product_vat");

                entity.Property(e => e.SalePrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Sale_price");

                entity.Property(e => e.SellPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Sell_price");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.UnitId).HasColumnName("Unit_id");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__Product_ca__04E4BC85");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__Unit_id__05D8E0BE");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.AmountDue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Amount_due");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

                entity.Property(e => e.DeliveryInformationId).HasColumnName("Delivery_information_id");

                entity.Property(e => e.OrderNote)
                    .HasMaxLength(255)
                    .HasColumnName("Order_note");

                entity.Property(e => e.OrderStatusId).HasColumnName("Order_status_id");

                entity.Property(e => e.OrderTypeId).HasColumnName("Order_type_id");

                entity.Property(e => e.RoomId).HasColumnName("Room_id");

                entity.Property(e => e.ShipperId).HasColumnName("Shipper_id");

                entity.Property(e => e.TotalQuantity).HasColumnName("Total_quantity");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Order__Customer___7B5B524B");

                entity.HasOne(d => d.DeliveryInformation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DeliveryInformationId)
                    .HasConstraintName("FK__Order__Delivery___7D439ABD");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__Order_sta__00200768");

                entity.HasOne(d => d.OrderType)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__Order_typ__7E37BEF6");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Order__Room_id__7F2BE32F");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipperId)
                    .HasConstraintName("FK__Order__Shipper_i__7C4F7684");
            });

            modelBuilder.Entity<OrderCancellationHistory>(entity =>
            {
                entity.ToTable("Order_cancellation_history");

                entity.Property(e => e.OrderCancellationHistoryId).HasColumnName("Order_cancellation_history_id");

                entity.Property(e => e.CashierId).HasColumnName("Cashier_id");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.HasOne(d => d.Cashier)
                    .WithMany(p => p.OrderCancellationHistories)
                    .HasForeignKey(d => d.CashierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_can__Cashi__0C85DE4D");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderCancellationHistories)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_can__Order__07C12930");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderCancellationHistories)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_can__Produ__0E6E26BF");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("Order_detail");

                entity.Property(e => e.OrderDetailId).HasColumnName("Order_detail_id");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("Product_name");

                entity.Property(e => e.ProductNote)
                    .HasMaxLength(255)
                    .HasColumnName("Product_note");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_det__Order__03F0984C");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order_det__Produ__10566F31");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("Order_status");

                entity.Property(e => e.OrderStatusId).HasColumnName("Order_status_id");

                entity.Property(e => e.OrderStatusTitle)
                    .HasMaxLength(50)
                    .HasColumnName("Order_status_title");
            });

            modelBuilder.Entity<OrderType>(entity =>
            {
                entity.ToTable("Order_type");

                entity.Property(e => e.OrderTypeId).HasColumnName("Order_type_id");

                entity.Property(e => e.OrderTypeTitle)
                    .HasMaxLength(50)
                    .HasColumnName("Order_type_title");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("Payment_method");

                entity.Property(e => e.PaymentMethodId).HasColumnName("Payment_method_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentMethodTitle)
                    .HasMaxLength(50)
                    .HasColumnName("Payment_method_title");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("Product_category");

                entity.Property(e => e.ProductCategoryId).HasColumnName("Product_category_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductCategoryTitle)
                    .HasMaxLength(100)
                    .HasColumnName("Product_category_title");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("Product_image");

                entity.Property(e => e.ProductImageId).HasColumnName("Product_image_id");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.ProductImage1)
                    .HasMaxLength(255)
                    .HasColumnName("Product_image");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product_i__Produ__114A936A");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.RoomId).HasColumnName("Room_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsUse)
                    .HasColumnName("isUse")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrdinalNumber).HasColumnName("Ordinal_number");

                entity.Property(e => e.QrCodeUrl)
                    .HasMaxLength(255)
                    .HasColumnName("Qr_code_url");

                entity.Property(e => e.RoomAreaId).HasColumnName("Room_area_id");

                entity.Property(e => e.RoomName)
                    .HasMaxLength(100)
                    .HasColumnName("Room_name");

                entity.Property(e => e.RoomNote)
                    .HasMaxLength(255)
                    .HasColumnName("Room_note");

                entity.Property(e => e.SeatNumber).HasColumnName("Seat_number");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.RoomArea)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.RoomAreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Room__Room_area___123EB7A3");
            });

            modelBuilder.Entity<RoomArea>(entity =>
            {
                entity.ToTable("Room_area");

                entity.Property(e => e.RoomAreaId).HasColumnName("Room_area_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RoomAreaName)
                    .HasMaxLength(100)
                    .HasColumnName("Room_area_name");

                entity.Property(e => e.RoomAreaNote)
                    .HasMaxLength(255)
                    .HasColumnName("Room_area_note");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.UnitId).HasColumnName("Unit_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UnitTitle)
                    .HasMaxLength(50)
                    .HasColumnName("Unit_title");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__AB6E6164F582B981")
                    .IsUnique();

                entity.HasIndex(e => e.Phone, "UQ__User__B43B145F0D622E7C")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("User_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsVerify)
                    .HasColumnName("is_verify")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserInformationId).HasColumnName("user_information_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__role_id__1332DBDC");

                entity.HasOne(d => d.UserInformation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserInformationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__user_infor__14270015");
            });

            modelBuilder.Entity<UserInformation>(entity =>
            {
                entity.ToTable("User_information");

                entity.Property(e => e.UserInformationId).HasColumnName("User_information_id");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("Date_of_birth");

                entity.Property(e => e.District).HasMaxLength(100);

                entity.Property(e => e.ProvinceCity)
                    .HasMaxLength(100)
                    .HasColumnName("Province_City");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .HasColumnName("User_name");

                entity.Property(e => e.WardCommune)
                    .HasMaxLength(100)
                    .HasColumnName("Ward_Commune");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__User_rol__760965CC8CC23E73");

                entity.ToTable("User_role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<VatTax>(entity =>
            {
                entity.ToTable("Vat_tax");

                entity.Property(e => e.VatTaxId).HasColumnName("Vat_tax_id");

                entity.Property(e => e.VatTaxValue)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("Vat_tax_value");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
