import { useEffect, useState } from 'react';
import { getPaymentHistory } from '../api/parkingApi';

export default function Payments() {
  const [payments, setPayments] = useState([]);

  useEffect(() => {
    getPaymentHistory().then(res => setPayments(res.data)).catch(console.error);
  }, []);

  const cell = { border: '1px solid #ddd', padding: '8px', fontSize: '14px' };

  return (
    <div>
      <h2>Payment History</h2>
      <table style={{ borderCollapse: 'collapse', width: '100%', marginTop: '20px' }}>
        <thead>
          <tr style={{ background: '#f8fafc', color: '#000000', borderBottom: '2px solid #ccc' }}>
            <th style={cell}>Ticket Number</th>
            <th style={cell}>Driver Name</th>
            <th style={cell}>Cashier Name</th>
            <th style={cell}>Date/Time</th>
            <th style={cell}>License Plate</th>
            <th style={cell}>Method</th>
            <th style={cell}>Amount</th>
            <th style={cell}>Receipt Ref</th>
          </tr>
        </thead>
        <tbody>
          {payments.map(p => (
            <tr key={p.paymentId}>
              <td style={cell}>{p.ticketNumber}</td>
              <td style={cell}>{p.driverName}</td>
              <td style={cell}>{p.cashierName}</td>
              <td style={cell}>{new Date(p.paymentTime).toLocaleString()}</td>
              <td style={cell}>{p.licensePlate}</td>
              <td style={cell}>{p.paymentMethod}</td>
              <td style={{ ...cell, fontWeight: 'bold', color: 'green' }}>${p.amount.toFixed(2)}</td>
              <td style={cell}>{p.referenceNumber}</td>
            </tr>
          ))}
          {payments.length === 0 && <tr><td colSpan="8" style={{ ...cell, textAlign: 'center' }}>No payments yet</td></tr>}
        </tbody>
      </table>
    </div>
  );
}