import { useEffect, useState } from 'react';
import { getDailyCollections } from '../api/parkingApi';

export default function Collections() {
  const [data, setData] = useState([]);

  useEffect(() => {
    getDailyCollections().then(res => setData(res.data)).catch(console.error);
  }, []);

  const cell = { border: '1px solid #ddd', padding: '8px', fontSize: '14px' };

  return (
    <div>
      <h2>Daily Collections by Cashier</h2>
      <p>Summary of all money collected today and previous days.</p>
      
      <table style={{ borderCollapse: 'collapse', width: '100%', marginTop: '20px' }}>
        <thead>
         <tr style={{ background: '#f8fafc', color: '#000000', borderBottom: '2px solid #ccc' }}>
            <th style={cell}>Cashier Name</th>
            <th style={cell}>Date</th>
            <th style={cell}>Payment Method</th>
            <th style={cell}>Total Transactions</th>
            <th style={cell}>Total Collected</th>
          </tr>
        </thead>
        <tbody>
          {data.map((row, idx) => (
            <tr key={idx}>
              <td style={cell}>{row.cashierName}</td>
              <td style={cell}>{new Date(row.collectionDate).toLocaleDateString()}</td>
              <td style={cell}>{row.paymentMethod}</td>
              <td style={cell}>{row.transactionCount}</td>
              <td style={{ ...cell, fontWeight: 'bold', color: 'green' }}>${row.totalCollected.toFixed(2)}</td>
            </tr>
          ))}
          {data.length === 0 && <tr><td colSpan="5" style={{ ...cell, textAlign: 'center' }}>No collections yet</td></tr>}
        </tbody>
      </table>
    </div>
  );
}