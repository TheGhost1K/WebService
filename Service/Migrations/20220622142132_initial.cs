using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44546e06-8719-4ad8-b88a-f271ae9d6eab",
                column: "ConcurrencyStamp",
                value: "aabff344-62be-4a02-9049-b71bfafbff15");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3b62472e-4f66-49fa-a20f-e7685b9565d8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "35804ddd-ebcc-4a2f-a08d-3d9a4790e349", "AQAAAAEAACcQAAAAEMrjv95/bclL7zOBtbI0MwX8i7YcLudZNK8FePyBKQy4sZKWtnPdIc8hJGQkKh44iw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44546e06-8719-4ad8-b88a-f271ae9d6eab",
                column: "ConcurrencyStamp",
                value: "09b05fc6-5152-4cd5-b105-818159b8c781");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3b62472e-4f66-49fa-a20f-e7685b9565d8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "14343c44-7807-4e15-8039-771963165c10", "AQAAAAEAACcQAAAAEPzKNw27ZiTNbmEMb6rOSQlqQb60LC/0TIYtGww+MlllLD4Q4SmA22vVg7B1vQJ6vg==" });
        }
    }
}
