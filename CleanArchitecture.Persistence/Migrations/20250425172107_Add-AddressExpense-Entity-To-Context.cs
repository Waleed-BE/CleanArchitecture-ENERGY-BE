using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressExpenseEntityToContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblAddressExpenses",
                columns: table => new
                {
                    UserAddressExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RatePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseForDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAddressExpenses", x => x.UserAddressExpenseId);
                    table.ForeignKey(
                        name: "FK_TblAddressExpenses_TblAddress_AddressId",
                        column: x => x.AddressId,
                        principalTable: "TblAddress",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAddressExpenses_TblExpenseType_ExpenseTypeId",
                        column: x => x.ExpenseTypeId,
                        principalTable: "TblExpenseType",
                        principalColumn: "ExpenseTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblAddressExpenses_AddressId",
                table: "TblAddressExpenses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAddressExpenses_ExpenseTypeId",
                table: "TblAddressExpenses",
                column: "ExpenseTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblAddressExpenses");
        }
    }
}
