import { useEffect, useState } from 'react';
import { getOccupancy, getTodayTotal, getTotalRevenue } from '../api/parkingApi';
import backgroundImage from '../assets/parking.jpg';

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [todayTotal, setTodayTotal] = useState(0);
  const [allTimeTotal, setAllTimeTotal] = useState(0);

  const load = () => {
    getOccupancy().then(res => setData(res.data[0])).catch(console.error);
    getTodayTotal().then(res => setTodayTotal(res.data.total)).catch(console.error);
    getTotalRevenue().then(res => setAllTimeTotal(res.data.total)).catch(console.error);
  };

  useEffect(() => {
    load();
    const interval = setInterval(load, 10000);
    return () => clearInterval(interval);
  }, []);

  if (!data) return <p>Loading...</p>;

  const cardStyle = { 
    flex: '1 1 180px', 
    border: '1px solid #e2e8f0', 
    borderRadius: '12px', 
    padding: '25px 20px', 
    textAlign: 'center', 
    background: 'white',
    position: 'relative',
    zIndex: 2
  };

  const labelStyle = {
    fontSize: '15px',
    color: '#64748b',
    fontWeight: '600',
    textTransform: 'uppercase',
    letterSpacing: '1px',
    marginBottom: '10px'
  };

  const valueStyle = {
    fontSize: '42px',
    fontWeight: '800',
    lineHeight: '1'
  };

  return (
    <div style={{
      backgroundImage: `url(${backgroundImage})`,
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      backgroundAttachment: 'fixed',
      height: 'calc(100vh - 66px)', 
      overflow: 'hidden',
      margin: '-20px',
      padding: '30px 40px',
      position: 'relative'
    }}>
      
      <div style={{
        position: 'absolute',
        top: 0, left: 0, right: 0, bottom: 0,
        backgroundColor: 'rgba(248, 250, 252, 0.30)', 
        zIndex: 1
      }}></div>

      <div style={{ position: 'relative', zIndex: 2, maxWidth: '1200px', margin: '0 auto' }}>
        
        <div style={{ background: 'white', padding: '20px 30px', borderRadius: '16px', display: 'inline-block', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.1)', marginBottom: '30px' }}>
            <h1 style={{ fontSize: '26px', margin: '0 0 5px 0', color: '#1e293b' }}>Parking Dashboard</h1>
            <p style={{ color: '#64748b', margin: 0, fontSize: '14px' }}>Live overview of your parking facility.</p>
        </div>
        
        <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap' }}>
          
          {/* All-Time Revenue Card */}
          <div className="modern-card" style={{ ...cardStyle, background: '#f5f3ff', borderColor: '#ddd6fe', flex: '1 1 100%', padding: '30px 20px' }}>
            <div style={{ ...labelStyle, fontSize: '16px' }}>Total Revenue (All Time)</div>
            <div style={{ ...valueStyle, fontSize: '48px', color: '#7c3aed' }}>
              ${allTimeTotal.toFixed(2)}
            </div>
          </div>

          {/* Today's Revenue Card */}
          <div className="modern-card" style={{ ...cardStyle, background: '#f0fdf4', borderColor: '#bbf7d0' }}>
            <div style={labelStyle}>Today's Revenue</div>
            <div style={{ ...valueStyle, color: '#16a34a' }}>
              ${todayTotal.toFixed(2)}
            </div>
          </div>

          {/* Total Spots Card */}
          <div className="modern-card" style={cardStyle}>
            <div style={labelStyle}>Total Spots</div>
            <div style={{ ...valueStyle, color: '#334155' }}>
              {data.totalSpots}
            </div>
          </div>

          {/* Occupied Card */}
          <div className="modern-card" style={{ ...cardStyle, background: '#fef2f2', borderColor: '#fecaca' }}>
            <div style={labelStyle}>Occupied</div>
            <div style={{ ...valueStyle, color: '#dc2626' }}>
              {data.occupiedSpots}
            </div>
          </div>

          {/* Available Card */}
          <div className="modern-card" style={{ ...cardStyle, background: '#eff6ff', borderColor: '#bfdbfe' }}>
            <div style={labelStyle}>Available</div>
            <div style={{ ...valueStyle, color: '#2563eb' }}>
              {data.availableSpots}
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}