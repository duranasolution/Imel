// src/api.js
export const apiFetch = async (url, options = {}) => {
    const token = sessionStorage.getItem('token');
  
    const headers = {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` })
    };
  
    const res = await fetch(url, {
      ...options,
      headers: {
        ...headers,
        ...(options.headers || {})
      }
    });
  
    return res;
  };
  