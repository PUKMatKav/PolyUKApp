using System.Configuration;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace PolyUKApp.Core
{
    /// <summary>
    /// Reads StockItem rows from Sage 200 via the existing "polysql01"
    /// connection string in App.config, and returns them already parsed
    /// into ProductSpec objects.
    ///
    /// NOTE: confirm StockItem / ItemCode / Name / Description match your
    /// schema — these are the standard Sage 200 names but customisations
    /// vary. Quick check:
    ///   SELECT TOP 5 ItemCode, Name, Description FROM StockItem
    /// </summary>
    public static class SageSpecRepository
    {
        private static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["polysql01"].ConnectionString;

        public static List<ProductSpec> SearchByCodeOrName(string searchTerm)
        {
            const string sql = @"
                SELECT TOP 100 Code, Name, Description, Weight, StockUnitName
                FROM StockItem
                WHERE (Code LIKE @Term OR Name LIKE @Term)
                  AND Description IS NOT NULL AND Description <> ''
                ORDER BY Code";

            using IDbConnection conn = new SqlConnection(ConnectionString);
            var rows = conn.Query(sql, new { Term = $"%{searchTerm}%" });

            var results = new List<ProductSpec>();
            foreach (var row in rows)
            {
                var spec = DescriptionParser.Parse(row.Code, row.Name, row.Description);
                spec.Weight = FormatWeight(row.Weight);
                spec.Unit = row.StockUnitName;
                results.Add(spec);
            }
            return results;
        }

        public static ProductSpec? GetByItemCode(string itemCode)
        {
            const string sql = @"
                SELECT Code, Name, Description, Weight, StockUnitName
                FROM StockItem
                WHERE Code = @Code";

            using IDbConnection conn = new SqlConnection(ConnectionString);
            var row = conn.QuerySingleOrDefault(sql, new { Code = itemCode });

            if (row == null) return null;

            var spec = DescriptionParser.Parse(row.Code, row.Name, row.Description ?? "");
            spec.Weight = FormatWeight(row.Weight);
            spec.Unit = row.StockUnitName;
            return spec;
        }

        private static string? FormatWeight(object? weight)
        {
            if (weight == null) return null;
            if (double.TryParse(weight.ToString(), out double kg))
                return $"{kg:0.###}kg";
            return weight.ToString();
        }
    }
}