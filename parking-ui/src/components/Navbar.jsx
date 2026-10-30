import { Link } from 'react-router-dom';

export default function Navbar() {
  return (
    <nav style={{ display:'flex', gap:'20px', padding:'15px 30px', background:'#1e293b', color:'white', alignItems:'center' }}>
      <strong style={{ marginRight:'auto', fontSize:'18px' }}>🅿️ Smart Parking</strong>
      <Link style={{ color:'white', textDecoration:'none' }} to="/dashboard">Dashboard</Link>
      <Link style={{ color:'white', textDecoration:'none' }} to="/checkin">Check-In</Link>
      <Link style={{ color:'white', textDecoration:'none' }} to="/sessions">Active Sessions</Link>
      <Link style={{ color:'white', textDecoration:'none' }} to="/payments">Payments</Link>
      <Link style={{ color:'white', textDecoration:'none' }} to="/collections">Collections</Link>
      <Link style={{ color:'#94a3b8', textDecoration:'none', marginLeft:'20px' }} to="/settings">⚙️ Settings</Link>
    </nav>
  );
}