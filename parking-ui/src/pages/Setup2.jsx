import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { savePricingConfig } from '../api/parkingApi';

export default function Setup({ setConfigured }) {
  const [custom, setCustom] = useState(false);
  // Removed maxDailyRate from initial state
  const [form, setForm] = useState({ firstHourRate: 7, additionalHourlyRate: 4, gracePeriodMinutes: 15 });
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSave = async (isDefault) => {
    try {
      const data = isDefault 
        ? { firstHourRate: 5, additionalHourlyRate: 3, gracePeriodMinutes: 15 }
        : form;
      
      await savePricingConfig(data);
      setConfigured(true);
      navigate('/dashboard');
    } catch (err) {
      setError('Failed to save configuration.');
    }
  };

  return (
    <div style={{ maxWidth: '500px', margin: '50px auto', textAlign: 'center' }}>
      <h1>Welcome to Smart Parking</h1>
      <p>Before you can start using the system, you need to set your pricing plan.</p>
      
      {!custom ? (
        <div style={{ display: 'flex', gap: '20px', justifyContent: 'center', marginTop: '30px' }}>
          <button onClick={() => handleSave(true)} style={{ padding: '15px 30px', fontSize: '16px', background: '#2563eb', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer' }}>
            Use Default (5/3)
          </button>
          <button onClick={() => setCustom(true)} style={{ padding: '15px 30px', fontSize: '16px', background: '#e2e8f0', border: 'none', borderRadius: '8px', cursor: 'pointer' }}>
            Set Custom Prices
          </button>
        </div>
      ) : (
        <div style={{ textAlign: 'left', background: '#f8fafc', padding: '20px', borderRadius: '8px', marginTop: '20px' }}>
          <label>First Hour Rate ($)</label>
          <input type="number" step="0.5" value={form.firstHourRate} onChange={e => setForm({...form, firstHourRate: parseFloat(e.target.value)})} style={{ width: '100%', padding: '8px', marginBottom: '15px' }} />
          
          <label>Additional Hourly Rate ($)</label>
          <input type="number" step="0.5" value={form.additionalHourlyRate} onChange={e => setForm({...form, additionalHourlyRate: parseFloat(e.target.value)})} style={{ width: '100%', padding: '8px', marginBottom: '15px' }} />
          
          {/* Max Daily Rate input removed */}
          
          <label>Grace Period (Minutes)</label>
          <input type="number" value={form.gracePeriodMinutes} onChange={e => setForm({...form, gracePeriodMinutes: parseInt(e.target.value)})} style={{ width: '100%', padding: '8px', marginBottom: '20px' }} />

          <button onClick={() => handleSave(false)} style={{ width: '100%', padding: '12px', background: '#16a34a', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer', fontSize: '16px' }}>
            Save & Start System
          </button>
        </div>
      )}
      {error && <p style={{ color: 'red' }}>{error}</p>}
    </div>
  );
}