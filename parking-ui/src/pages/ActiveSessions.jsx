import { useEffect, useState } from 'react';
import { getActiveSessions, checkOut } from '../api/parkingApi';

export default function ActiveSessions() {
  const [sessions, setSessions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState('');
  const [breakdown, setBreakdown] = useState(null);
  
  const [cashier, setCashier] = useState('Cashier 1');
  const [paymentMethod, setPaymentMethod] = useState('CASH');

  const load = () => {
    getActiveSessions().then(res => { setSessions(res.data); setLoading(false); }).catch(console.error);
  };

  useEffect(() => {
    load();
    const interval = setInterval(load, 10000);
    return () => clearInterval(interval);
  }, []);

  const handleCheckout = async (ticketNumber) => {
    setMessage(''); setBreakdown(null);
    try {
      const res = await checkOut(ticketNumber, paymentMethod, cashier);
      setMessage(`✅ Checkout successful! Ticket: ${ticketNumber}`);
      setBreakdown(res.data.breakdown);
      load();
    } catch (err) {
      setMessage(`❌ Checkout failed: ${err.response?.data?.error || err.message}`);
    }
  };

  if (loading) return <p>Loading sessions...</p>;

  return (
    <div>
      <h2>Active Parking Sessions</h2>
      
      <div style={{ display: 'flex', gap: '20px', marginBottom: '20px', padding: '15px', background: '#f1f5f9', borderRadius: '8px' }}>
        <div>
          <label style={{ display: 'block', fontSize: '12px', fontWeight: 'bold', color: '#475569', marginBottom: '5px' }}>Current Cashier</label>
          <input value={cashier} onChange={e => setCashier(e.target.value)} style={{ padding: '8px', border: '1px solid #cbd5e1', borderRadius: '4px' }} />
        </div>
        <div>
          <label style={{ display: 'block', fontSize: '12px', fontWeight: 'bold', color: '#475569', marginBottom: '5px' }}>Payment Method</label>
          <select value={paymentMethod} onChange={e => setPaymentMethod(e.target.value)} style={{ padding: '8px', border: '1px solid #cbd5e1', borderRadius: '4px' }}>
            <option value="CASH">Cash</option>
            <option value="CREDIT_CARD">Credit Card</option>
            <option value="DEBIT_CARD">Debit Card</option>
            <option value="MOBILE_PAY">Mobile Pay</option>
          </select>
        </div>
      </div>

      {message && <p style={{ fontWeight: 'bold', color: message.includes('✅') ? 'green' : 'red' }}>{message}</p>}
      {breakdown && (
        <div style={{ background: '#e0f2fe', padding: '15px', borderRadius: '8px', marginBottom: '20px', border: '1px solid #7dd3fc', boxShadow: '0 2px 4px rgba(0,0,0,0.05)' }}>
          <strong>Calculation Receipt:</strong><br/>
          {breakdown}
        </div>
      )}

      <div style={{ background: 'white', borderRadius: '8px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.1)', overflow: 'hidden' }}>
        <table style={{ borderCollapse: 'collapse', width: '100%' }}>
          <thead>
            <tr style={{ background: '#1e293b', color: '#ffffff' }}>
              <th style={cell}>Ticket Number</th>
              <th style={cell}>Driver Name</th>
              <th style={cell}>License Plate</th>
              <th style={cell}>Spot Code</th>
              <th style={cell}>Entry Time</th>
              <th style={cell}>Elapsed</th>
              <th style={cell}>Current Fee</th>
              <th style={cell}>Action</th>
            </tr>
          </thead>
          <tbody>
            {sessions.map(s => (
              <tr key={s.ticketId} style={{ borderBottom: '1px solid #e2e8f0' }}>
                <td style={cell}>{s.ticketNumber}</td>
                <td style={cell}>{s.driverName}</td>
                <td style={cell}>{s.licensePlate}</td>
                <td style={cell}>{s.spotCode}</td>
                <td style={cell}>{new Date(s.entryTime).toLocaleTimeString()}</td>
                <td style={cell}>{s.elapsedMinutes} min</td>
                <td style={cell}>${s.currentAccumulatedFee.toFixed(2)}</td>
                <td style={cell}>
                  <button 
                    className="hover-pop"
                    onClick={() => handleCheckout(s.ticketNumber)} 
                    style={{ padding: '8px 12px', background: '#dc2626', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
                  >
                    Check Out
                  </button>
                </td>
              </tr>
            ))}
            {sessions.length === 0 && <tr><td colSpan="8" style={{ ...cell, textAlign: 'center', padding: '30px' }}>No active sessions</td></tr>}
          </tbody>
        </table>
      </div>
    </div>
  );
}
const cell = { padding: '12px 15px', fontSize: '14px' };