import axios from 'axios';

const api = axios.create({ baseURL: '/api/Parking' });

export const getOccupancy = () => api.get('/occupancy');
export const getActiveSessions = () => api.get('/active-sessions');
export const checkIn = (data) => api.post('/checkin', data);
export const checkOut = (ticketNumber, paymentMethod, cashierName) =>
  api.post('/checkout', { ticketNumber: ticketNumber.trim(), paymentMethod, cashierName });
export const getPaymentHistory = () => api.get('/payments');
export const getDailyCollections = () => api.get('/daily-collections');
export const getTodayTotal = () => api.get('/today-total');
export const getTotalRevenue = () => api.get('/total-revenue');
export const getPricingConfig = () => api.get('/pricing-config');
export const savePricingConfig = (data) => api.post('/pricing-config', data);