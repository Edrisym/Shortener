import { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [longUrl, setLongUrl] = useState('');
  const [shortUrl, setShortUrl] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [copySuccess, setCopySuccess] = useState('');
  const [recentUrls, setRecentUrls] = useState([]);
  
  // API base URL
  const baseUrl = 'http://localhost:5255';

  // Get recent URLs from localStorage on component mount
  useEffect(() => {
    const savedUrls = localStorage.getItem('recentUrls');
    if (savedUrls) {
      setRecentUrls(JSON.parse(savedUrls));
    }
  }, []);

  // Save recent URLs to localStorage when updated
  useEffect(() => {
    localStorage.setItem('recentUrls', JSON.stringify(recentUrls));
  }, [recentUrls]);

  const isValidUrl = (url) => {
    try {
      new URL(url);
      return true;
    } catch (e) {
      return false;
    }
  };

  const shortenUrl = async (e) => {
    e.preventDefault();
    
    if (!longUrl) {
      setError('Please enter a URL');
      return;
    }

    if (!isValidUrl(longUrl)) {
      setError('Please enter a valid URL');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      // const encodedUrl = encodeURIComponent(longUrl);
      const response = await fetch(`${baseUrl}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json', 
        },
        body: JSON.stringify({
          longUrl: longUrl,
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to shorten URL');
      }

      const data = await response.json();
      
      // Based on your response format
      // const shortCode = data.shortUrl || data.code || "";
      // const newShortUrl = `${baseUrl}/${shortCode}`;
      const newShortUrl = data.longUrl
      setShortUrl(newShortUrl);
      
      // Add to recent URLs
      const newRecentUrl = {
        longUrl,
        shortUrl: newShortUrl,
        // shortCode,
        createdAt: new Date().toISOString()
      };
      
      setRecentUrls(prev => [newRecentUrl, ...prev.slice(0, 4)]);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const copyToClipboard = async (text) => {
    try {
      await navigator.clipboard.writeText(text);
      setCopySuccess('Copied!');
      setTimeout(() => setCopySuccess(''), 2000);
    } catch (err) {
      setCopySuccess('Failed to copy!');
    }
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  };

  // Function to extract domain from URL for display
  const extractDomain = (url) => {
    try {
      const urlObj = new URL(url);
      return urlObj.hostname;
    } catch (e) {
      return url;
    }
  };

  return (
    <div className="app-container">
      <div className="url-shortener-card">
        {/* Header */}
        <div className="header">
          <h1 className="title">BLink</h1>
          <p className="subtitle">Transform your long URLs into short, shareable links</p>
        </div>

        {/* URL Input Form */}
        <form onSubmit={shortenUrl} className="form-container">
          <div className="input-row">
            <input
              type="text"
              value={longUrl}
              onChange={(e) => setLongUrl(e.target.value)}
              placeholder="Enter your long URL here..."
              className="url-input"
            />
            <button
              type="submit"
              disabled={isLoading}
              className="shorten-button"
            >
              {isLoading ? 'Shortening...' : 'Shorten URL'}
            </button>
          </div>
          {error && <p className="error-message">{error}</p>}
        </form>

        {/* Result Section */}
        {shortUrl && (
          <div className="result-section">
            <h2 className="section-title">Your shortened URL:</h2>
            <div className="result-box">
              <a 
                href={shortUrl} 
                target="_blank" 
                rel="noopener noreferrer"
                className="short-url-link"
              >
                {shortUrl}
              </a>
              <button
                onClick={() => copyToClipboard(shortUrl)}
                className="copy-button"
              >
                {copySuccess || 'Copy'}
              </button>
            </div>
            <p className="help-text">
              When someone clicks this link, they'll be redirected to your original URL.
            </p>
          </div>
        )}

        {/* Recent URLs */}
        {recentUrls.length > 0 && (
          <div className="recent-urls-section">
            <h2 className="section-title">Recent URLs</h2>
            <div className="table-container">
              <table className="urls-table">
                <thead>
                  <tr>
                    <th className="table-header">Original URL</th>
                    <th className="table-header">Short URL</th>
                    <th className="table-header">Created</th>
                    <th className="table-header">Action</th>
                  </tr>
                </thead>
                <tbody>
                  {recentUrls.map((url, index) => (
                    <tr key={index} className="table-row">
                      <td className="table-cell url-cell" title={url.longUrl}>
                        <a href={url.longUrl} className="original-url" target="_blank" rel="noopener noreferrer">
                          {extractDomain(url.longUrl)}
                        </a>
                      </td>
                      <td className="table-cell">
                        <a href={url.shortUrl} className="short-url" target="_blank" rel="noopener noreferrer">
                          {url.shortUrl.split('/').pop()}
                        </a>
                      </td>
                      <td className="table-cell date-cell">
                        {formatDate(url.createdAt)}
                      </td>
                      <td className="table-cell">
                        <button
                          onClick={() => copyToClipboard(url.shortUrl)}
                          className="small-copy-button"
                        >
                          Copy
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* Stats and Analytics Preview */}
        <div className="stats-section">
          <h2 className="section-title">Get More from Your Links</h2>
          <div className="stats-grid">
            <div className="stat-card">
              <div className="stat-value">{recentUrls.length}</div>
              <div className="stat-label">Links Created</div>
            </div>
            <div className="stat-card">
              <div className="stat-value">0</div>
              <div className="stat-label">Total Clicks</div>
            </div>
            <div className="stat-card">
              <div className="stat-value">0%</div>
              <div className="stat-label">Click Rate</div>
            </div>
          </div>
          <p className="upgrade-text">
            Sign up for a free account to unlock click tracking and analytics!
          </p>
        </div>

        {/* Footer */}
        <div className="footer">
          <p>© 2025 LinkSnip - Share links smarter</p>
        </div>
      </div>
    </div>
  );
}

export default App;