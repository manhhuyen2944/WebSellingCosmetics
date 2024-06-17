using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSellingCosmetics.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountType",
                columns: table => new
                {
                    AccountTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountType", x => x.AccountTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount_percent = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    UseNumber = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.DiscountId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodsId);
                });

            migrationBuilder.CreateTable(
                name: "Products_Inventorys",
                columns: table => new
                {
                    Product_InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    QuantitySold = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Inventory", x => x.Product_InventoryId);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    ProductTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.ProductTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ShippingMethods",
                columns: table => new
                {
                    ShippingMethodsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingMethods", x => x.ShippingMethodsId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Product_InventoryId = table.Column<int>(type: "int", nullable: true),
                    ProductTypeId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Product_Inventory",
                        column: x => x.Product_InventoryId,
                        principalTable: "Products_Inventorys",
                        principalColumn: "Product_InventoryId");
                    table.ForeignKey(
                        name: "FK_Product_ProductType",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "ProductTypeId");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avartar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Point = table.Column<int>(type: "int", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Account_AccountType",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "AccountTypeId");
                    table.ForeignKey(
                        name: "FK_Account_Role",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "Receipt_Products",
                columns: table => new
                {
                    Receipt_ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt_Products", x => x.Receipt_ProductId);
                    table.ForeignKey(
                        name: "FK_Receipt_Product_Product",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Address_Account",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFromUserId = table.Column<bool>(type: "bit", nullable: true),
                    IsToUserId = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Accounts_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "Oders",
                columns: table => new
                {
                    OdersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CreateDay = table.Column<DateTime>(type: "datetime", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    DiscountId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oders", x => x.OdersId);
                    table.ForeignKey(
                        name: "FK_Oders_Account",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_Oders_Discount",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "DiscountId");
                });

            migrationBuilder.CreateTable(
                name: "Oder_Items",
                columns: table => new
                {
                    OderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oder_Items", x => new { x.OderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Oder_Items_Oders",
                        column: x => x.OderId,
                        principalTable: "Oders",
                        principalColumn: "OdersId");
                    table.ForeignKey(
                        name: "FK_Oder_Items_Product",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OdersId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodsId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentsId);
                    table.ForeignKey(
                        name: "FK_Payments_Oders",
                        column: x => x.OdersId,
                        principalTable: "Oders",
                        principalColumn: "OdersId");
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods",
                        column: x => x.PaymentMethodsId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodsId");
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    ShipmentsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OdersId = table.Column<int>(type: "int", nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingMethodsId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.ShipmentsId);
                    table.ForeignKey(
                        name: "FK_Shipments_Oders",
                        column: x => x.OdersId,
                        principalTable: "Oders",
                        principalColumn: "OdersId");
                    table.ForeignKey(
                        name: "FK_Shipments_ShippingMethods",
                        column: x => x.ShippingMethodsId,
                        principalTable: "ShippingMethods",
                        principalColumn: "ShippingMethodsId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_AccountId",
                table: "Address",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FromUserId",
                table: "Messages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Oder_Items_ProductId",
                table: "Oder_Items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Oders_AccountId",
                table: "Oders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Oders_DiscountId",
                table: "Oders",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OdersId",
                table: "Payments",
                column: "OdersId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodsId",
                table: "Payments",
                column: "PaymentMethodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Product_InventoryId",
                table: "Products",
                column: "Product_InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_Products_ProductId",
                table: "Receipt_Products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OdersId",
                table: "Shipments",
                column: "OdersId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShippingMethodsId",
                table: "Shipments",
                column: "ShippingMethodsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Oder_Items");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Receipt_Products");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Oders");

            migrationBuilder.DropTable(
                name: "ShippingMethods");

            migrationBuilder.DropTable(
                name: "Products_Inventorys");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "AccountType");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
