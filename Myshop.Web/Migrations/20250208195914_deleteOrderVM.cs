using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Myshop.Web.Migrations
{
    /// <inheritdoc />
    public partial class deleteOrderVM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderVM_OrderVMId",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderVM");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderVMId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderVMId",
                table: "OrderDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderVMId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderVM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderVM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderVM_OrderHeaders_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderVMId",
                table: "OrderDetails",
                column: "OrderVMId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderVM_OrderHeaderId",
                table: "OrderVM",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderVM_OrderVMId",
                table: "OrderDetails",
                column: "OrderVMId",
                principalTable: "OrderVM",
                principalColumn: "Id");
        }
    }
}
