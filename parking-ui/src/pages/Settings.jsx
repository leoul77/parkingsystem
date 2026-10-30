import { useEffect, useState } from 'react';
import { getPricingConfig, savePricingConfig } from '../api/parkingApi';

export default function Settings() {
  // Removed maxDailyRate from initial state
  const [form, setForm] = useState({ firstHourRate: 0, additionalHourlyRate: 0, gracePeriodMinutes: 0 });
  const [message, setMessage] = useState('');

  useEffect(() => {
    getPricingConfig().then(res => {
      if (res.data.pricingConfigured) {
        setForm({
          firstHourRate: res.data.firstHourRate,
          additionalHourlyRate: res.data.additionalHourlyRate,
          gracePeriodMinutes: res.data.gracePeriodMinutes
        });
      }
    });
  }, []);

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      await savePricingConfig(form);
      setMessage('✅ Pricing updated successfully!');
    } catch (err) {
      setMessage('❌ Failed to update pricing.');
    }
  };

  return (
    <div style={{ maxWidth: '400px' }}>
      <h2>Pricing Settings</h2>
      <p>Update the pricing plan for the parking lot. New tickets will use these rates.</p>
      <form onSubmit={handleSave} style={{ display: 'flex', flexDirection: 'column', gap: '10px', marginTop: '20px' }}>
        <label>First Hour Rate ($)</label>
        <input type="number" step="0.5" value={form.firstHourRate} onChange={e => setForm({...form, firstHourRate: parseFloat(e.target.value)})} />
        
        <label>Additional Hourly Rate ($)</label>
        <input type="number" step="0.5" value={form.additionalHourlyRate} onChange={e => setForm({...form, additionalHourlyRate: parseFloat(e.target.value)})} />
        
        {/* Max Daily Rate input removed */}
        
        <label>Grace Period (Minutes)</label>
        <input type="number" value={form.gracePeriodMinutes} onChange={e => setForm({...form, gracePeriodMinutes: parseInt(e.target.value)})} />

        <button type="submit" style={{ padding: '10px', background: '#2563eb', color: 'white', border: 'none', borderRadius: '5px', marginTop: '10px' }}>
          Save Changes
        </button>
      </form>
      {message && <p style={{ marginTop: '15px', fontWeight: 'bold' }}>{message}</p>}
    </div>
  );
}