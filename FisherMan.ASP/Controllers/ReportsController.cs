using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ships-expiring-permits")]
        public async Task<IActionResult> GetShipsWithExpiringPermits()
        {
            var report = await _context.Database.SqlQueryRaw<ExpiringPermitReportRow>(@"
                SELECT s.Id AS ShipId,
                       s.InternationalNumber,
                       s.CallSign,
                       s.Marking,
                       p.ValidUntil AS PermitValidUntil
                FROM Ships s
                INNER JOIN Permits p ON p.ShipId = s.Id
                WHERE p.IsRevoked = 0
                  AND p.ValidUntil >= CAST(GETDATE() AS date)
                  AND p.ValidUntil < DATEADD(MONTH, 1, CAST(GETDATE() AS date))
                ORDER BY p.ValidUntil, s.InternationalNumber")
                .ToListAsync();

            return Ok(report);
        }

        [HttpGet("amateur-catch-ranking")]
        public async Task<IActionResult> GetAmateurCatchRanking()
        {
            var report = await _context.Database.SqlQueryRaw<AmateurCatchRankingRow>(@"
                SELECT ac.AmateurId,
                       CONCAT(u.FirstName, ' ', u.LastName) AS FullName,
                       SUM(ac.QuantityKg) AS TotalCatchKg
                FROM AmateurCatches ac
                INNER JOIN ApplicationUsers u ON u.Id = ac.AmateurId
                WHERE ac.DateCaught >= DATEADD(YEAR, -1, CAST(GETDATE() AS date))
                GROUP BY ac.AmateurId, u.FirstName, u.LastName
                ORDER BY SUM(ac.QuantityKg) DESC")
                .ToListAsync();

            return Ok(report);
        }

        [HttpGet("ship-trip-statistics")]
        public async Task<IActionResult> GetShipTripStatistics([FromQuery] int? year)
        {
            var targetYear = year ?? DateTime.UtcNow.Year;

            var report = await _context.Database.SqlQueryRaw<ShipTripStatisticsRow>($@"
                SELECT s.Id AS ShipId,
                       s.InternationalNumber,
                       AVG(ft.DurationInHours) AS AvgTripHours,
                       MIN(ft.DurationInHours) AS MinTripHours,
                       MAX(ft.DurationInHours) AS MaxTripHours,
                       AVG(tripTotals.TripCatchKg) AS AvgCatchKgPerTrip,
                       MIN(tripTotals.TripCatchKg) AS MinCatchKgPerTrip,
                       MAX(tripTotals.TripCatchKg) AS MaxCatchKgPerTrip,
                       COUNT(DISTINCT ft.Id) AS TripsCount,
                       SUM(tripTotals.TripCatchKg) AS TotalCatchKg
                FROM Ships s
                INNER JOIN FishingTrips ft ON ft.ShipId = s.Id
                INNER JOIN (
                    SELECT cc.FishingTripId,
                           SUM(cc.QuantityKg) AS TripCatchKg
                    FROM CommercialCatches cc
                    GROUP BY cc.FishingTripId
                ) tripTotals ON tripTotals.FishingTripId = ft.Id
                WHERE YEAR(ft.StartTime) = {targetYear}
                GROUP BY s.Id, s.InternationalNumber
                ORDER BY TotalCatchKg DESC")
                .ToListAsync();

            return Ok(report);
        }

        [HttpGet("carbon-footprint")]
        public async Task<IActionResult> GetCarbonFootprint([FromQuery] int? year)
        {
            var targetYear = year ?? DateTime.UtcNow.Year;

            var report = await _context.Database.SqlQueryRaw<CarbonFootprintRow>($@"
                WITH ValidShips AS (
                    SELECT DISTINCT p.ShipId
                    FROM Permits p
                    WHERE p.IsRevoked = 0
                      AND p.ValidUntil >= CAST(GETDATE() AS date)
                ),
                TripStats AS (
                    SELECT ft.ShipId,
                           SUM(ft.DurationInHours) AS TotalHours
                    FROM FishingTrips ft
                    WHERE YEAR(ft.StartTime) = {targetYear}
                    GROUP BY ft.ShipId
                ),
                CatchStats AS (
                    SELECT ft.ShipId,
                           SUM(cc.QuantityKg) AS TotalCatchKg
                    FROM FishingTrips ft
                    INNER JOIN CommercialCatches cc ON cc.FishingTripId = ft.Id
                    WHERE YEAR(ft.StartTime) = {targetYear}
                    GROUP BY ft.ShipId
                )
                SELECT s.Id AS ShipId,
                       s.InternationalNumber,
                       e.FuelType,
                       ts.TotalHours,
                       e.AverageFuelConsumptionPerHour,
                       cs.TotalCatchKg,
                       CASE
                           WHEN cs.TotalCatchKg IS NULL OR cs.TotalCatchKg = 0 THEN NULL
                           ELSE (ts.TotalHours * e.AverageFuelConsumptionPerHour) / cs.TotalCatchKg
                       END AS FuelPerKgCatch
                FROM Ships s
                INNER JOIN ValidShips vs ON vs.ShipId = s.Id
                INNER JOIN Engines e ON e.Id = s.EngineId
                LEFT JOIN TripStats ts ON ts.ShipId = s.Id
                LEFT JOIN CatchStats cs ON cs.ShipId = s.Id
                WHERE cs.TotalCatchKg IS NOT NULL AND cs.TotalCatchKg > 0
                ORDER BY FuelPerKgCatch")
                .ToListAsync();

            return Ok(report);
        }

        public sealed class ExpiringPermitReportRow
        {
            public int ShipId { get; set; }
            public string InternationalNumber { get; set; } = string.Empty;
            public string CallSign { get; set; } = string.Empty;
            public string Marking { get; set; } = string.Empty;
            public DateTime PermitValidUntil { get; set; }
        }

        public sealed class AmateurCatchRankingRow
        {
            public int AmateurId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public decimal TotalCatchKg { get; set; }
        }

        public sealed class ShipTripStatisticsRow
        {
            public int ShipId { get; set; }
            public string InternationalNumber { get; set; } = string.Empty;
            public decimal AvgTripHours { get; set; }
            public decimal MinTripHours { get; set; }
            public decimal MaxTripHours { get; set; }
            public decimal AvgCatchKgPerTrip { get; set; }
            public decimal MinCatchKgPerTrip { get; set; }
            public decimal MaxCatchKgPerTrip { get; set; }
            public int TripsCount { get; set; }
            public decimal TotalCatchKg { get; set; }
        }

        public sealed class CarbonFootprintRow
        {
            public int ShipId { get; set; }
            public string InternationalNumber { get; set; } = string.Empty;
            public string FuelType { get; set; } = string.Empty;
            public decimal? TotalHours { get; set; }
            public decimal AverageFuelConsumptionPerHour { get; set; }
            public decimal? TotalCatchKg { get; set; }
            public decimal? FuelPerKgCatch { get; set; }
        }
    }
}
