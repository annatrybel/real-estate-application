using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateImageModelToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Images",
                table: "Image",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "ImagesId",
                table: "Image",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ImageModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ImagesId",
                table: "Image",
                column: "ImagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_ImageModel_ImagesId",
                table: "Image",
                column: "ImagesId",
                principalTable: "ImageModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_ImageModel_ImagesId",
                table: "Image");

            migrationBuilder.DropTable(
                name: "ImageModel");

            migrationBuilder.DropIndex(
                name: "IX_Image_ImagesId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ImagesId",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Image",
                newName: "Images");
        }
    }
}
