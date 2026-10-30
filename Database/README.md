# 🅿️ Smart Parking System - Database Documentation

This directory contains the complete database setup for the **Smart Parking Management System** using **MySQL 8.0+**.

---

## 📁 Files Included

- **[`schema.sql`](schema.sql):** Database creation (`parking_system_db`), tables, foreign keys, indexes, functions, stored procedures, and views.
- **[`seed.sql`](seed.sql):** Initial demo data (24 parking spaces across 2 floors, 4 vehicle pricing plans, registered cars, and live active parking sessions).

---

## 📊 Database Architecture Overview

### 1. Tables

| Table | Description |
| :--- | :--- |
| **`parking_spots`** | Physical spaces (Floor 1 & 2), spot type (`STANDARD`, `EV_CHARGING`, `HANDICAPPED`, `VIP`), and occupancy flag (`is_occupied`). |
| **`pricing_rates`** | Configurable pricing tiers with base first-hour rate, subsequent hourly rate, 15-minute grace period, and 24h daily maximum cap. |
| **`vehicles`** | Optional vehicle registry (license plate, make, model, color, vehicle type). |
| **`parking_tickets`** | **Core Session Table:** Tracks license plate, spot ID, entry timestamp, exit timestamp, elapsed minutes, calculated fee, and status (`ACTIVE`, `PAID`, `COMPLETED`). |
| **`payments`** | Audit log of checkout payments (`amount`, `payment_method`, `transaction_reference`, `payment_time`). |

### 2. Automated Functions & Stored Procedures

- **`fn_calculate_parking_fee(entry_time, exit_time, rate_id)`:**
  Calculates the fee:
  1. Checks if within the 15-minute grace period (returns `$0.00`).
  2. Rounds up to the next billable hour (e.g., 65 mins = 2 hours).
  3. Applies first-hour base rate + subsequent hourly rates.
  4. Automatically caps at the 24-hour daily maximum rate.
- **`sp_checkin_vehicle(plate, spot, type, OUT ticket_number)`:**
  Marks the spot as occupied and creates a new active parking ticket with entry timestamp.
- **`sp_checkout_vehicle(ticket_number, OUT duration, OUT amount)`:**
  Computes total elapsed time, computes exact total money owed, marks ticket paid, and frees up the parking space.

### 3. Monitoring Views

- **`vw_active_parking_sessions`:** Real-time query showing all currently parked cars, spot code, entry time, elapsed minutes, and live accumulated fee.
- **`vw_occupancy_summary`:** Real-time summary showing total spots, occupied spots, available spots, and occupancy percentage.

---

## 🚀 How to Apply to MySQL

Since **MySQL 8.0 is already running** on your system (`MySQL80` service on port `3306`), you can apply the scripts in two ways:

### Option A: Via Command Line (PowerShell)

Open PowerShell and run:

```powershell
# 1. Run the schema creation script
mysql -u root -p < "C:\Users\15712\Desktop\ParkingSystem\Database\schema.sql"

# 2. (Optional) Run the sample seed data script
mysql -u root -p < "C:\Users\15712\Desktop\ParkingSystem\Database\seed.sql"
```
*(Enter your MySQL root password when prompted).*

---

### Option B: Via MySQL Workbench / DBeaver / VS Code

1. Open **MySQL Workbench** and connect to your `Local instance 3306`.
2. Go to **File** > **Open SQL Script...**
3. Select and execute [`schema.sql`](schema.sql) (click the ⚡ Execute button).
4. Select and execute [`seed.sql`](seed.sql).

---

## 🧪 Testing Queries

Once loaded, run these queries to see the system in action:

```sql
USE parking_system_db;

-- 1. Check all live cars currently in the lot and their real-time bill:
SELECT * FROM vw_active_parking_sessions;

-- 2. Check total lot capacity and available spaces:
SELECT * FROM vw_occupancy_summary;

-- 3. Check out a car and see how much money is owed:
CALL sp_checkout_vehicle('TKT-20261019-1002', @duration, @total_money);
SELECT @duration AS minutes_parked, @total_money AS amount_to_pay;
```
