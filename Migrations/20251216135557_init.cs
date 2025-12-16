using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NYC_Taxi_Data_Search_Engine.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxiTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DropoffDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassengerCount = table.Column<int>(type: "int", nullable: false),
                    TripDistance = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DropoffLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FareAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TollsAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxiTrips", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxiTrips_FareAmount",
                table: "TaxiTrips",
                column: "FareAmount");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiTrips_PassengerCount",
                table: "TaxiTrips",
                column: "PassengerCount");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiTrips_PaymentType",
                table: "TaxiTrips",
                column: "PaymentType");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiTrips_PickupDateTime",
                table: "TaxiTrips",
                column: "PickupDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiTrips_TripDistance",
                table: "TaxiTrips",
                column: "TripDistance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxiTrips");
        }
    }
}
