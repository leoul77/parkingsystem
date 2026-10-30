-- ======================================================================
-- SMART PARKING MANAGEMENT SYSTEM - SEED DATA SCRIPT
-- Populates default spots, rates, and sample parking sessions
-- ======================================================================

USE parking_system_db;

-- 1. Insert Default Pricing Plans
INSERT INTO pricing_rates (name, vehicle_type, grace_period_mins, first_hour_rate, additional_hourly_rate, max_daily_rate)
VALUES 
    ('Standard Car Plan', 'CAR', 15, 5.00, 3.00, 35.00),
    ('Motorcycle Budget Plan', 'MOTORCYCLE', 15, 3.00, 1.50, 20.00),
    ('SUV / Truck Plan', 'SUV', 15, 6.00, 4.00, 45.00),
    ('Heavy Truck Plan', 'TRUCK', 15, 8.00, 5.00, 60.00)
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- 2. Insert Parking Spots (Floor 1: Ground Level)
INSERT INTO parking_spots (spot_code, floor_number, spot_type, is_occupied)
VALUES
    ('A-101', 1, 'HANDICAPPED', FALSE),
    ('A-102', 1, 'HANDICAPPED', FALSE),
    ('A-103', 1, 'EV_CHARGING', TRUE),
    ('A-104', 1, 'EV_CHARGING', FALSE),
    ('A-105', 1, 'STANDARD', TRUE),
    ('A-106', 1, 'STANDARD', FALSE),
    ('A-107', 1, 'STANDARD', FALSE),
    ('A-108', 1, 'STANDARD', TRUE),
    ('A-109', 1, 'COMPACT', FALSE),
    ('A-110', 1, 'COMPACT', FALSE),
    ('A-111', 1, 'VIP', FALSE),
    ('A-112', 1, 'VIP', FALSE)
ON DUPLICATE KEY UPDATE spot_code = VALUES(spot_code);

-- 3. Insert Parking Spots (Floor 2: Upper Level)
INSERT INTO parking_spots (spot_code, floor_number, spot_type, is_occupied)
VALUES
    ('B-201', 2, 'STANDARD', FALSE),
    ('B-202', 2, 'STANDARD', FALSE),
    ('B-203', 2, 'STANDARD', FALSE),
    ('B-204', 2, 'STANDARD', FALSE),
    ('B-205', 2, 'STANDARD', TRUE),
    ('B-206', 2, 'STANDARD', FALSE),
    ('B-207', 2, 'COMPACT', FALSE),
    ('B-208', 2, 'COMPACT', FALSE),
    ('B-209', 2, 'EV_CHARGING', FALSE),
    ('B-210', 2, 'EV_CHARGING', FALSE),
    ('B-211', 2, 'STANDARD', FALSE),
    ('B-212', 2, 'STANDARD', FALSE)
ON DUPLICATE KEY UPDATE spot_code = VALUES(spot_code);

-- 4. Sample Vehicles
INSERT INTO vehicles (license_plate, vehicle_type, make_model, color)
VALUES
    ('ABC-1234', 'CAR', 'Toyota Camry', 'Silver'),
    ('EV-8899', 'CAR', 'Tesla Model 3', 'White'),
    ('MOTO-44', 'MOTORCYCLE', 'Yamaha R6', 'Blue'),
    ('TRK-9000', 'SUV', 'Ford Explorer', 'Black')
ON DUPLICATE KEY UPDATE license_plate = VALUES(license_plate);

-- 5. Insert Active Parked Cars (Simulated entry earlier today to test live counters!)
-- Car 1: Parked 45 minutes ago at A-103
INSERT INTO parking_tickets (ticket_number, license_plate, spot_id, rate_id, entry_time, status)
VALUES (
    'TKT-20261019-1001',
    'EV-8899',
    (SELECT spot_id FROM parking_spots WHERE spot_code = 'A-103'),
    (SELECT rate_id FROM pricing_rates WHERE vehicle_type = 'CAR'),
    DATE_SUB(NOW(), INTERVAL 45 MINUTE),
    'ACTIVE'
);

-- Car 2: Parked 2 hours 15 minutes ago at A-105
INSERT INTO parking_tickets (ticket_number, license_plate, spot_id, rate_id, entry_time, status)
VALUES (
    'TKT-20261019-1002',
    'ABC-1234',
    (SELECT spot_id FROM parking_spots WHERE spot_code = 'A-105'),
    (SELECT rate_id FROM pricing_rates WHERE vehicle_type = 'CAR'),
    DATE_SUB(NOW(), INTERVAL 135 MINUTE),
    'ACTIVE'
);

-- Car 3: Parked 5 hours ago at A-108
INSERT INTO parking_tickets (ticket_number, license_plate, spot_id, rate_id, entry_time, status)
VALUES (
    'TKT-20261019-1003',
    'TRK-9000',
    (SELECT spot_id FROM parking_spots WHERE spot_code = 'A-108'),
    (SELECT rate_id FROM pricing_rates WHERE vehicle_type = 'SUV'),
    DATE_SUB(NOW(), INTERVAL 300 MINUTE),
    'ACTIVE'
);

-- 6. Insert Completed Past Ticket (Yesterday)
INSERT INTO parking_tickets (ticket_number, license_plate, spot_id, rate_id, entry_time, exit_time, total_duration_minutes, total_amount, status)
VALUES (
    'TKT-20261018-9999',
    'MOTO-44',
    (SELECT spot_id FROM parking_spots WHERE spot_code = 'A-106'),
    (SELECT rate_id FROM pricing_rates WHERE vehicle_type = 'MOTORCYCLE'),
    DATE_SUB(NOW(), INTERVAL 26 HOUR),
    DATE_SUB(NOW(), INTERVAL 24 HOUR),
    120,
    4.50,
    'COMPLETED'
);

INSERT INTO payments (ticket_id, amount, payment_method, payment_status, payment_time, reference_number)
VALUES (
    LAST_INSERT_ID(),
    4.50,
    'CREDIT_CARD',
    'SUCCESS',
    DATE_SUB(NOW(), INTERVAL 24 HOUR),
    'TXN-984321-VISA'
);
