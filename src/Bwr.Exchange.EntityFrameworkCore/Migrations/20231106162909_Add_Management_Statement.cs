using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bwr.Exchange.Migrations
{
    public partial class Add_Management_Statement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagementStatement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "double", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<double>(type: "double", nullable: true),
                    TreasuryActionType = table.Column<int>(type: "int", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: true),
                    MainAccount = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BeforChange = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AfterChange = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AmountOfFirstCurrency = table.Column<double>(type: "double", nullable: true),
                    AmoutOfSecondCurrency = table.Column<double>(type: "double", nullable: true),
                    PaidAmountOfFirstCurrency = table.Column<double>(type: "double", nullable: true),
                    ReceivedAmountOfFirstCurrency = table.Column<double>(type: "double", nullable: true),
                    PaidAmountOfSecondCurrency = table.Column<double>(type: "double", nullable: true),
                    ReceivedAmountOfSecondCurrency = table.Column<double>(type: "double", nullable: true),
                    Commission = table.Column<double>(type: "double", nullable: true),
                    FirstCurrencyId = table.Column<int>(type: "int", nullable: true),
                    SecondCurrencyId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    ToCompanyId = table.Column<int>(type: "int", nullable: true),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    BeneficiaryId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementStatement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementStatement_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Companies_ToCompanyId",
                        column: x => x.ToCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Currencies_FirstCurrencyId",
                        column: x => x.FirstCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Currencies_SecondCurrencyId",
                        column: x => x.SecondCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Customers_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManagementStatement_Customers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_BeneficiaryId",
                table: "ManagementStatement",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_ClientId",
                table: "ManagementStatement",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_CompanyId",
                table: "ManagementStatement",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_CurrencyId",
                table: "ManagementStatement",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_FirstCurrencyId",
                table: "ManagementStatement",
                column: "FirstCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_SecondCurrencyId",
                table: "ManagementStatement",
                column: "SecondCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_SenderId",
                table: "ManagementStatement",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_ToCompanyId",
                table: "ManagementStatement",
                column: "ToCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementStatement_UserId",
                table: "ManagementStatement",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagementStatement");
        }
    }
}
