using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPriceAndQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Products', 'Price') IS NULL
    ALTER TABLE [Products] ADD [Price] nvarchar(max) NOT NULL CONSTRAINT [DF_Products_Price] DEFAULT N'';

IF COL_LENGTH('Products', 'Quantity') IS NULL
    ALTER TABLE [Products] ADD [Quantity] nvarchar(max) NOT NULL CONSTRAINT [DF_Products_Quantity] DEFAULT N'';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @sql nvarchar(max) = N'';

SELECT @sql += N'ALTER TABLE [Products] DROP CONSTRAINT ' + QUOTENAME(dc.name) + N';'
FROM sys.default_constraints dc
JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID('Products') AND c.name IN ('Price', 'Quantity');

EXEC sp_executesql @sql;

IF COL_LENGTH('Products', 'Price') IS NOT NULL
    ALTER TABLE [Products] DROP COLUMN [Price];

IF COL_LENGTH('Products', 'Quantity') IS NOT NULL
    ALTER TABLE [Products] DROP COLUMN [Quantity];
");
        }
    }
}
