-- ======================================================================
-- SMART PARKING MANAGEMENT SYSTEM - DATABASE SCHEMA
-- Target Database: MySQL 8.0+
-- Database Name: parking_system_db
-- ======================================================================

CREATE DATABASE IF NOT EXISTS parking_system_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE parking_system_db;

-- 1. PARKING SPOTS TABLE
-- Represents physical parking spaces in the lot or garage.
CREATE TABLE IF NOT EXISTS parking_spots (
    spot_id INT AUTO_INCREMENT PRIMARY KEY,
    spot_code VARCHAR(20) NOT NULL UNIQUE,          -- e.g. 'A-101', 'B-205'
    floor_number INT NOT NULL DEFAULT 1,            -- e.g. 1, 2, 3
    spot_type ENUM('STANDARD', 'COMPACT', 'EV_CHARGING', 'HANDICAPPED', 'VIP') NOT NULL DEFAULT 'STANDARD',
    is_occupied BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_spot_status (is_occupied, is_active),
    INDEX idx_spot_type (spot_type)
) ENGINE=InnoDB;

-- 2. PRICING RATES TABLE
-- Configurable pricing plans based on vehicle type and duration rules.
CREATE TABLE IF NOT EXISTS pricing_rates (
    rate_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,                      -- e.g. 'Standard Car Rate'
    vehicle_type ENUM('MOTORCYCLE', 'CAR', 'SUV', 'TRUCK') NOT NULL DEFAULT 'CAR',
    grace_period_mins INT NOT NULL DEFAULT 15,      -- Free exit if leaving within X minutes
    first_hour_rate DECIMAL(8, 2) NOT NULL DEFAULT 5.00,  -- Base charge for 1st hour
    additional_hourly_rate DECIMAL(8, 2) NOT NULL DEFAULT 3.00, -- Rate for every subsequent hour
    max_daily_rate DECIMAL(8, 2) NOT NULL DEFAULT 35.00,  -- Maximum cap per 24 hours
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_vehicle_rate (vehicle_type, is_active)
) ENGINE=InnoDB;

-- 3. VEHICLES TABLE (Optional registry for recurring cars / lookup)
CREATE TABLE IF NOT EXISTS vehicles (
    vehicle_id INT AUTO_INCREMENT PRIMARY KEY,
    license_plate VARCHAR(20) NOT NULL UNIQUE,
    vehicle_type ENUM('MOTORCYCLE', 'CAR', 'SUV', 'TRUCK') NOT NULL DEFAULT 'CAR',
    make_model VARCHAR(50) NULL,
    color VARCHAR(30) NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_plate (license_plate)
) ENGINE=InnoDB;

-- 4. PARKING TICKETS / SESSIONS TABLE (Core Entity)
-- Tracks when a car enters, which spot it occupies, and the exit calculation.
CREATE TABLE IF NOT EXISTS parking_tickets (
    ticket_id INT AUTO_INCREMENT PRIMARY KEY,
    ticket_number VARCHAR(50) NOT NULL UNIQUE,      -- e.g. 'TKT-20261019-1001'
    license_plate VARCHAR(20) NOT NULL,
    spot_id INT NOT NULL,
    rate_id INT NOT NULL,
    entry_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    exit_time DATETIME NULL,
    total_duration_minutes INT NULL,                -- Stored duration in minutes upon checkout
    total_amount DECIMAL(10, 2) NULL,               -- Calculated fee upon exit
    status ENUM('ACTIVE', 'PAID', 'COMPLETED', 'CANCELLED') NOT NULL DEFAULT 'ACTIVE',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (spot_id) REFERENCES parking_spots(spot_id) ON DELETE RESTRICT,
    FOREIGN KEY (rate_id) REFERENCES pricing_rates(rate_id) ON DELETE RESTRICT,
    INDEX idx_ticket_status (status),
    INDEX idx_ticket_plate (license_plate),
    INDEX idx_ticket_entry (entry_time)
) ENGINE=InnoDB;

-- 5. PAYMENTS TABLE
-- Keeps an audit record of customer payments upon checkout.
CREATE TABLE IF NOT EXISTS payments (
    payment_id INT AUTO_INCREMENT PRIMARY KEY,
    ticket_id INT NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    payment_method ENUM('CASH', 'CREDIT_CARD', 'DEBIT_CARD', 'MOBILE_PAY') NOT NULL,
    payment_status ENUM('SUCCESS', 'FAILED', 'REFUNDED') NOT NULL DEFAULT 'SUCCESS',
    payment_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reference_number VARCHAR(100) NULL,             -- Transaction receipt or auth code
    
    FOREIGN KEY (ticket_id) REFERENCES parking_tickets(ticket_id) ON DELETE CASCADE,
    INDEX idx_payment_ticket (ticket_id),
    INDEX idx_payment_time (payment_time)
) ENGINE=InnoDB;

-- ======================================================================
-- STORED FUNCTIONS & PROCEDURES (AUTOMATED CALCULATION & WORKFLOWS)
-- ======================================================================

DELIMITER $$

-- Function to calculate fee based on entry time, exit time, and pricing rules
DROP FUNCTION IF EXISTS fn_calculate_parking_fee$$
CREATE FUNCTION fn_calculate_parking_fee(
    p_entry_time DATETIME,
    p_exit_time DATETIME,
    p_rate_id INT
)
RETURNS DECIMAL(10, 2)
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_grace_mins INT;
    DECLARE v_first_hour_rate DECIMAL(8, 2);
    DECLARE v_additional_rate DECIMAL(8, 2);
    DECLARE v_max_daily_rate DECIMAL(8, 2);
    
    DECLARE v_total_mins INT;
    DECLARE v_billable_hours INT;
    DECLARE v_fee DECIMAL(10, 2);
    DECLARE v_days INT;
    DECLARE v_rem_hours INT;

    -- Fetch pricing rule
    SELECT grace_period_mins, first_hour_rate, additional_hourly_rate, max_daily_rate
    INTO v_grace_mins, v_first_hour_rate, v_additional_rate, v_max_daily_rate
    FROM pricing_rates
    WHERE rate_id = p_rate_id;

    -- If no rule found, fallback to defaults
    IF v_first_hour_rate IS NULL THEN
        SET v_grace_mins = 15;
        SET v_first_hour_rate = 5.00;
        SET v_additional_rate = 3.00;
        SET v_max_daily_rate = 35.00;
    END IF;

    -- Compute duration in minutes
    SET v_total_mins = TIMESTAMPDIFF(MINUTE, p_entry_time, p_exit_time);

    -- 1. Free within grace period
    IF v_total_mins <= v_grace_mins THEN
        RETURN 0.00;
    END IF;

    -- 2. Convert to billable hours (ceiling: e.g. 61 mins = 2 hours)
    SET v_billable_hours = CEIL(v_total_mins / 60.0);

    -- 3. Calculate with 24-hour daily cap consideration
    SET v_days = FLOOR(v_billable_hours / 24);
    SET v_rem_hours = v_billable_hours % 24;

    IF v_rem_hours = 0 THEN
        SET v_fee = v_days * v_max_daily_rate;
    ELSE
        -- 1st hour + subsequent hours for the remainder
        SET v_fee = (v_days * v_max_daily_rate) + v_first_hour_rate + (GREATEST(v_rem_hours - 1, 0) * v_additional_rate);
        -- Ensure remaining hours don't exceed the daily cap
        IF (v_fee - (v_days * v_max_daily_rate)) > v_max_daily_rate THEN
            SET v_fee = (v_days + 1) * v_max_daily_rate;
        END IF;
    END IF;

    RETURN v_fee;
END$$

-- Stored Procedure: Check-in a vehicle
DROP PROCEDURE IF EXISTS sp_checkin_vehicle$$
CREATE PROCEDURE sp_checkin_vehicle(
    IN p_license_plate VARCHAR(20),
    IN p_spot_code VARCHAR(20),
    IN p_vehicle_type VARCHAR(20),
    OUT p_ticket_number VARCHAR(50)
)
BEGIN
    DECLARE v_spot_id INT;
    DECLARE v_rate_id INT;
    DECLARE v_is_occ BOOLEAN;

    -- Find spot
    SELECT spot_id, is_occupied INTO v_spot_id, v_is_occ
    FROM parking_spots
    WHERE spot_code = p_spot_code AND is_active = TRUE;

    IF v_spot_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Parking spot does not exist.';
    END IF;

    IF v_is_occ = TRUE THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Selected parking spot is already occupied.';
    END IF;

    -- Find matching rate
    SELECT rate_id INTO v_rate_id
    FROM pricing_rates
    WHERE vehicle_type = p_vehicle_type AND is_active = TRUE
    LIMIT 1;

    IF v_rate_id IS NULL THEN
        SELECT rate_id INTO v_rate_id FROM pricing_rates LIMIT 1;
    END IF;

    -- Generate unique ticket number
    SET p_ticket_number = CONCAT('TKT-', DATE_FORMAT(NOW(), '%Y%m%d%H%i%s'), '-', LPAD(FLOOR(RAND() * 1000), 3, '0'));

    -- Insert active ticket
    INSERT INTO parking_tickets (ticket_number, license_plate, spot_id, rate_id, entry_time, status)
    VALUES (p_ticket_number, UPPER(TRIM(p_license_plate)), v_spot_id, v_rate_id, NOW(), 'ACTIVE');

    -- Mark spot occupied
    UPDATE parking_spots SET is_occupied = TRUE WHERE spot_id = v_spot_id;
END$$

-- Stored Procedure: Checkout vehicle and compute cost
DROP PROCEDURE IF EXISTS sp_checkout_vehicle$$
CREATE PROCEDURE sp_checkout_vehicle(
    IN p_ticket_number VARCHAR(50),
    OUT p_duration_mins INT,
    OUT p_total_amount DECIMAL(10, 2)
)
BEGIN
    DECLARE v_ticket_id INT;
    DECLARE v_spot_id INT;
    DECLARE v_rate_id INT;
    DECLARE v_entry_time DATETIME;
    DECLARE v_now DATETIME;

    SET v_now = NOW();

    -- Find ticket
    SELECT ticket_id, spot_id, rate_id, entry_time
    INTO v_ticket_id, v_spot_id, v_rate_id, v_entry_time
    FROM parking_tickets
    WHERE ticket_number = p_ticket_number AND status = 'ACTIVE';

    IF v_ticket_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Active ticket not found.';
    END IF;

    -- Compute duration and fee
    SET p_duration_mins = TIMESTAMPDIFF(MINUTE, v_entry_time, v_now);
    SET p_total_amount = fn_calculate_parking_fee(v_entry_time, v_now, v_rate_id);

    -- Update ticket
    UPDATE parking_tickets
    SET exit_time = v_now,
        total_duration_minutes = p_duration_mins,
        total_amount = p_total_amount,
        status = 'PAID'
    WHERE ticket_id = v_ticket_id;

    -- Free up parking spot
    UPDATE parking_spots SET is_occupied = FALSE WHERE spot_id = v_spot_id;
END$$

DELIMITER ;

-- ======================================================================
-- VIEWS FOR MONITORING AND REPORTING
-- ======================================================================

-- View: All currently parked vehicles with live elapsed time and real-time fee
CREATE OR REPLACE VIEW vw_active_parking_sessions AS
SELECT 
    t.ticket_id,
    t.ticket_number,
    t.license_plate,
    s.spot_code,
    s.floor_number,
    s.spot_type,
    r.name AS pricing_plan,
    t.entry_time,
    TIMESTAMPDIFF(MINUTE, t.entry_time, NOW()) AS elapsed_minutes,
    fn_calculate_parking_fee(t.entry_time, NOW(), t.rate_id) AS current_accumulated_fee
FROM parking_tickets t
JOIN parking_spots s ON t.spot_id = s.spot_id
JOIN pricing_rates r ON t.rate_id = r.rate_id
WHERE t.status = 'ACTIVE';

-- View: Parking lot capacity and occupancy breakdown
CREATE OR REPLACE VIEW vw_occupancy_summary AS
SELECT 
    COUNT(*) AS total_spots,
    SUM(CASE WHEN is_occupied = TRUE THEN 1 ELSE 0 END) AS occupied_spots,
    SUM(CASE WHEN is_occupied = FALSE AND is_active = TRUE THEN 1 ELSE 0 END) AS available_spots,
    ROUND((SUM(CASE WHEN is_occupied = TRUE THEN 1 ELSE 0 END) / COUNT(*)) * 100, 1) AS occupancy_percentage
FROM parking_spots
WHERE is_active = TRUE;
