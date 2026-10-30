using Microsoft.EntityFrameworkCore;
using parkingSystem.Api.DTOs;
using parkingSystem.Api.Models;
using System.Data;

namespace parkingSystem.Api.Services;

public class ParkingService : IParkingService
{
    private readonly ParkingDbContext _db;

    public ParkingService(ParkingDbContext db) { _db = db; }

    public async Task<List<OccupancySummaryDto>> GetOccupancySummaryAsync()
    {
        var result = new List<OccupancySummaryDto>();
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT total_spots, occupied_spots, available_spots, occupancy_percentage FROM vw_occupancy_summary";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new OccupancySummaryDto
            {
                TotalSpots = reader.IsDBNull(0) ? 0 : reader.GetInt64(0),
                OccupiedSpots = reader.IsDBNull(1) ? 0m : reader.GetDecimal(1),
                AvailableSpots = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2),
                OccupancyPercentage = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3)
            });
        }
        return result;
    }

    public async Task<List<ActiveSessionDto>> GetActiveSessionsAsync()
    {
        var result = new List<ActiveSessionDto>();
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT ticket_id, ticket_number, license_plate, driver_name, driver_phone,
                                   spot_code, floor_number, spot_type, pricing_plan, entry_time,
                                   elapsed_minutes, current_accumulated_fee
                            FROM vw_active_parking_sessions";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new ActiveSessionDto
            {
                TicketId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                TicketNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
                LicensePlate = reader.IsDBNull(2) ? "" : reader.GetString(2),
                DriverName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                DriverPhone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                SpotCode = reader.IsDBNull(5) ? "" : reader.GetString(5),
                FloorNumber = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                SpotType = reader.IsDBNull(7) ? "" : reader.GetString(7),
                PricingPlan = reader.IsDBNull(8) ? "" : reader.GetString(8),
                EntryTime = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9),
                ElapsedMinutes = reader.IsDBNull(10) ? 0 : Convert.ToInt64(reader.GetValue(10)),
                CurrentAccumulatedFee = reader.IsDBNull(11) ? 0m : Convert.ToDecimal(reader.GetValue(11))
            });
        }
        return result;
    }

    public async Task<string> CheckInAsync(CheckInRequest request)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_checkin_vehicle";
        cmd.CommandType = CommandType.StoredProcedure;

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@p_license_plate"; p1.Value = request.LicensePlate; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@p_spot_code"; p2.Value = request.SpotCode; cmd.Parameters.Add(p2);
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@p_vehicle_type"; p3.Value = request.VehicleType; cmd.Parameters.Add(p3);
        var p4 = cmd.CreateParameter(); p4.ParameterName = "@p_driver_name"; p4.Value = (object?)request.DriverName ?? DBNull.Value; cmd.Parameters.Add(p4);
        var p5 = cmd.CreateParameter(); p5.ParameterName = "@p_driver_phone"; p5.Value = (object?)request.DriverPhone ?? DBNull.Value; cmd.Parameters.Add(p5);
        var p6 = cmd.CreateParameter(); p6.ParameterName = "@p_ticket_number"; p6.Direction = ParameterDirection.Output; p6.Size = 50; cmd.Parameters.Add(p6);

        await cmd.ExecuteNonQueryAsync();
        return p6.Value?.ToString() ?? string.Empty;
    }

    public async Task<CheckOutResponse> CheckOutAsync(CheckOutRequest request)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

        var ticketInfo = new CheckOutResponse { TicketNumber = request.TicketNumber.Trim() };
        using (var q = conn.CreateCommand())
        {
            q.CommandText = @"SELECT t.license_plate, IFNULL(d.full_name,''), s.spot_code, t.entry_time
                              FROM parking_tickets t
                              JOIN parking_spots s ON t.spot_id = s.spot_id
                              LEFT JOIN drivers d ON t.driver_id = d.driver_id
                              WHERE t.ticket_number = @tkt";
            var p = q.CreateParameter(); p.ParameterName = "@tkt"; p.Value = request.TicketNumber.Trim(); q.Parameters.Add(p);
            using var r = await q.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                ticketInfo.LicensePlate = r.IsDBNull(0) ? "" : r.GetString(0);
                ticketInfo.DriverName = r.IsDBNull(1) ? "" : r.GetString(1);
                ticketInfo.SpotCode = r.IsDBNull(2) ? "" : r.GetString(2);
                ticketInfo.EntryTime = r.IsDBNull(3) ? DateTime.MinValue : r.GetDateTime(3);
            }
        }

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_checkout_vehicle";
        cmd.CommandType = CommandType.StoredProcedure;

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@p_ticket_number"; p1.Value = request.TicketNumber.Trim(); cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@p_payment_method"; p2.Value = request.PaymentMethod.ToUpperInvariant(); cmd.Parameters.Add(p2);

        var pCashier = cmd.CreateParameter();
        pCashier.ParameterName = "@p_cashier_name";
        pCashier.Value = string.IsNullOrWhiteSpace(request.CashierName) ? "Unassigned" : request.CashierName.Trim();
        cmd.Parameters.Add(pCashier);

        var p3 = cmd.CreateParameter(); p3.ParameterName = "@p_duration_mins"; p3.Direction = ParameterDirection.Output; p3.DbType = DbType.Int32; cmd.Parameters.Add(p3);
        var p4 = cmd.CreateParameter(); p4.ParameterName = "@p_total_amount"; p4.Direction = ParameterDirection.Output; p4.DbType = DbType.Decimal; cmd.Parameters.Add(p4);

        await cmd.ExecuteNonQueryAsync();

        ticketInfo.ExitTime = DateTime.Now;
        ticketInfo.DurationMinutes = p3.Value == DBNull.Value ? 0 : Convert.ToInt32(p3.Value);
        ticketInfo.TotalAmount = p4.Value == DBNull.Value ? 0m : Convert.ToDecimal(p4.Value);
        ticketInfo.PaymentMethod = request.PaymentMethod.ToUpperInvariant();
        ticketInfo.CashierName = pCashier.Value?.ToString() ?? "Unassigned";

        using (var q2 = conn.CreateCommand())
        {
            q2.CommandText = @"SELECT p.reference_number FROM payments p
                               JOIN parking_tickets t ON p.ticket_id = t.ticket_id
                               WHERE t.ticket_number = @tkt ORDER BY p.payment_id DESC LIMIT 1";
            var pp = q2.CreateParameter(); pp.ParameterName = "@tkt"; pp.Value = request.TicketNumber.Trim(); q2.Parameters.Add(pp);
            var refVal = await q2.ExecuteScalarAsync();
            ticketInfo.ReferenceNumber = refVal?.ToString() ?? "";
        }

        var hours = ticketInfo.DurationMinutes / 60.0;
        ticketInfo.Breakdown =
            $"Entry: {ticketInfo.EntryTime:yyyy-MM-dd HH:mm} | " +
            $"Exit: {ticketInfo.ExitTime:yyyy-MM-dd HH:mm} | " +
            $"Duration: {ticketInfo.DurationMinutes} min ({hours:F1} hrs) | " +
            $"Amount: {ticketInfo.TotalAmount:C} | " +
            $"Method: {ticketInfo.PaymentMethod} | " +
            $"Cashier: {ticketInfo.CashierName}";

        return ticketInfo;
    }

    public async Task<List<PaymentHistoryDto>> GetPaymentHistoryAsync()
    {
        var result = new List<PaymentHistoryDto>();
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT payment_id, payment_time, amount, payment_method, IFNULL(cashier_name,''),
                                   IFNULL(reference_number,''), ticket_number, license_plate,
                                   IFNULL(driver_name,''), IFNULL(driver_phone,''), spot_code,
                                   entry_time, exit_time, total_duration_minutes
                            FROM vw_payment_history";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new PaymentHistoryDto
            {
                PaymentId = reader.GetInt32(0),
                PaymentTime = reader.GetDateTime(1),
                Amount = reader.GetDecimal(2),
                PaymentMethod = reader.IsDBNull(3) ? "" : reader.GetString(3),
                CashierName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                ReferenceNumber = reader.IsDBNull(5) ? "" : reader.GetString(5),
                TicketNumber = reader.IsDBNull(6) ? "" : reader.GetString(6),
                LicensePlate = reader.IsDBNull(7) ? "" : reader.GetString(7),
                DriverName = reader.IsDBNull(8) ? "" : reader.GetString(8),
                DriverPhone = reader.IsDBNull(9) ? "" : reader.GetString(9),
                SpotCode = reader.IsDBNull(10) ? "" : reader.GetString(10),
                EntryTime = reader.GetDateTime(11),
                ExitTime = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                TotalDurationMinutes = reader.IsDBNull(13) ? null : reader.GetInt32(13)
            });
        }
        return result;
    }

    public async Task<List<DailyCollectionDto>> GetDailyCollectionsAsync()
    {
        var result = new List<DailyCollectionDto>();
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT collection_date, cashier_name, payment_method, transaction_count, total_collected
                            FROM vw_daily_collections ORDER BY collection_date DESC, cashier_name";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new DailyCollectionDto
            {
                CollectionDate = reader.GetDateTime(0),
                CashierName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                PaymentMethod = reader.IsDBNull(2) ? "" : reader.GetString(2),
                TransactionCount = Convert.ToInt64(reader.GetValue(3)),
                TotalCollected = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4)
            });
        }
        return result;
    }

    public async Task<decimal> GetTodayTotalAsync()
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT IFNULL(SUM(amount),0) FROM payments
                            WHERE payment_status='SUCCESS' AND DATE(payment_time)=CURDATE()";
        var val = await cmd.ExecuteScalarAsync();
        return val == null || val == DBNull.Value ? 0m : Convert.ToDecimal(val);
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT IFNULL(SUM(amount),0) FROM payments WHERE payment_status='SUCCESS'";
        var val = await cmd.ExecuteScalarAsync();
        return val == null || val == DBNull.Value ? 0m : Convert.ToDecimal(val);
    }

    public async Task<PricingConfigResponse> GetPricingConfigAsync()
    {
        var resp = new PricingConfigResponse();
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT setting_value FROM system_settings WHERE setting_key = 'pricing_configured'";
            var val = await cmd.ExecuteScalarAsync();
            resp.PricingConfigured = val?.ToString()?.ToLowerInvariant() == "true";
        }

        if (resp.PricingConfigured)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT name, first_hour_rate, additional_hourly_rate, grace_period_mins
                                FROM pricing_rates WHERE is_active = TRUE LIMIT 1";
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                resp.PlanName = reader.IsDBNull(0) ? "" : reader.GetString(0);
                resp.FirstHourRate = reader.IsDBNull(1) ? 0m : reader.GetDecimal(1);
                resp.AdditionalHourlyRate = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2);
                resp.GracePeriodMinutes = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
            }
        }

        return resp;
    }

    public async Task SavePricingConfigAsync(PricingConfigRequest request)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_configure_pricing";
        cmd.CommandType = CommandType.StoredProcedure;

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@p_first_hour"; p1.Value = request.FirstHourRate; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@p_additional_hour"; p2.Value = request.AdditionalHourlyRate; cmd.Parameters.Add(p2);
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@p_grace_mins"; p3.Value = request.GracePeriodMinutes; cmd.Parameters.Add(p3);

        await cmd.ExecuteNonQueryAsync();
    }
}