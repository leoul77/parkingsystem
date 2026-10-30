import { useEffect, useState } from 'react';
import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Dashboard from './pages/Dashboard';
import CheckIn from './pages/CheckIn';
import ActiveSessions from './pages/ActiveSessions';
import Payments from './pages/Payments';
import Collections from './pages/Collections';
import Setup from './pages/Setup2';
import Settings from './pages/Settings';
import { getPricingConfig } from './api/parkingApi';

export default function App() {
  const [configured, setConfigured] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    getPricingConfig()
      .then(res => {
        setConfigured(res.data.pricingConfigured);
        if (!res.data.pricingConfigured) {
          navigate('/setup');
        }
      })
      .catch(err => console.error("Could not check config", err));
  }, [navigate]);

  if (configured === null) return <h2 style={{ padding: '20px' }}>Loading System...</h2>;

  return (
    <div>
      <Navbar />
      <div style={{ padding: '20px' }}>
        <Routes>
          <Route path="/" element={configured ? <Navigate to="/dashboard" /> : <Navigate to="/setup" />} />
          <Route path="/setup" element={<Setup setConfigured={setConfigured} />} />
          <Route path="/settings" element={<Settings />} />
          
          <Route path="/dashboard" element={configured ? <Dashboard /> : <Navigate to="/setup" />} />
          <Route path="/checkin" element={configured ? <CheckIn /> : <Navigate to="/setup" />} />
          <Route path="/sessions" element={configured ? <ActiveSessions /> : <Navigate to="/setup" />} />
          <Route path="/payments" element={configured ? <Payments /> : <Navigate to="/setup" />} />
          <Route path="/collections" element={configured ? <Collections /> : <Navigate to="/setup" />} />
        </Routes>
      </div>
    </div>
  );
}
