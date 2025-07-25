using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobIdToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnqueuedCompletionJobId",
                table: "UserChats",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnqueuedCompletionJobId",
                table: "UserChats");
        }
    }
}
