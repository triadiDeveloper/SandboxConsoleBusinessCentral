using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleBusinessCentral.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    No = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsibility_Center = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Post_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country_Region_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone_No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IC_Partner_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salesperson_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Posting_Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allow_Multiple_Posting_Groups = table.Column<bool>(type: "bit", nullable: true),
                    Gen_Bus_Posting_Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VAT_Bus_Posting_Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Price_Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Disc_Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payment_Terms_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reminder_Terms_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fin_Charge_Terms_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Search_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credit_Limit_LCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Blocked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Privacy_Blocked = table.Column<bool>(type: "bit", nullable: true),
                    Last_Date_Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Application_Method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Combine_Shipments = table.Column<bool>(type: "bit", nullable: true),
                    Reserve = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ship_to_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipping_Advice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipping_Agent_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Base_Calendar_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance_LCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Balance_Due_LCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Sales_LCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Payments_LCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Coupled_to_CRM = table.Column<bool>(type: "bit", nullable: true),
                    Coupled_to_Dataverse = table.Column<bool>(type: "bit", nullable: true),
                    Global_Dimension_1_Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Global_Dimension_2_Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency_Filter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date_Filter = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.No);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
