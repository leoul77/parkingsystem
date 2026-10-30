import { useState } from 'react';
import { checkIn } from '../api/parkingApi';

export default function CheckIn() {
  const [form, setForm] = useState({ licensePlate: '', spotCode: '', vehicleType: 'CAR', driverName: '', driverPhone: '' });
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage(''); setError('');
    try {
      const res = await checkIn(form);
      setMessage(`✅ Check-in successful. Ticket: ${res.data.ticketNumber}`);
      setForm({ licensePlate:'', spotCode:'', vehicleType:'CAR', driverName:'', driverPhone:'' });
    } catch (err) {
      setError(err.response?.data?.error || 'Check-in failed');
    }
  };

  return (
    <div className="container">
      <h2>Vehicle Check-In</h2>
      <div style={{ background: 'white', padding: '30px', borderRadius: '12px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.1)', maxWidth: '500px' }}>
        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
          
          <div>
            <label style={{ fontWeight: 'bold', fontSize: '14px', color: '#475569' }}>License Plate</label>
            <input value={form.licensePlate} onChange={e => setForm({ ...form, licensePlate: e.target.value })} required placeholder="e.g. ABC-1234" style={{ width: '100%', marginTop: '5px', boxSizing: 'border-box' }} />
          </div>
          
          <div>
            <label style={{ fontWeight: 'bold', fontSize: '14px', color: '#475569' }}>Spot Code</label>
            <input value={form.spotCode} onChange={e => setForm({ ...form, spotCode: e.target.value })} required placeholder="e.g. A-106" style={{ width: '100%', marginTop: '5px', boxSizing: 'border-box' }} />
          </div>

          <div>
            <label style={{ fontWeight: 'bold', fontSize: '14px', color: '#475569' }}>Vehicle Type</label>
            <select value={form.vehicleType} onChange={e => setForm({ ...form, vehicleType: e.target.value })} style={{ width: '100%', marginTop: '5px', boxSizing: 'border-box' }}>
              <option value="CAR">Car</option>
              <option value="MOTORCYCLE">Motorcycle</option>
              <option value="SUV">SUV</option>
              <option value="TRUCK">Truck</option>
            </select>
          </div>

          <div>
            <label style={{ fontWeight: 'bold', fontSize: '14px', color: '#475569' }}>Driver Name</label>
            <input value={form.driverName} onChange={e => setForm({ ...form, driverName: e.target.value })} placeholder="e.g. John Doe" style={{ width: '100%', marginTop: '5px', boxSizing: 'border-box' }} />
          </div>

          <div>
            <label style={{ fontWeight: 'bold', fontSize: '14px', color: '#475569' }}>Driver Phone</label>
            <input value={form.driverPhone} onChange={e => setForm({ ...form, driverPhone: e.target.value })} placeholder="e.g. 0911223344" style={{ width: '100%', marginTop: '5px', boxSizing: 'border-box' }} />
          </div>

          <button type="submit" className="hover-pop" style={{ padding: '15px', background: '#2563eb', color: 'white', border: 'none', borderRadius: '8px', fontSize: '16px', fontWeight: 'bold', marginTop: '10px' }}>
            Check In Vehicle
          </button>
        </form>
        
        {message && <p style={{ color: 'green', marginTop: '15px', fontWeight: 'bold' }}>{message}</p>}
        {error && <p style={{ color: 'red', marginTop: '15px', fontWeight: 'bold' }}>{error}</p>}
      </div>
    </div>
  );
}