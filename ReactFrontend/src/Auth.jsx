import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);

  // Kad se učita app, provjeri da li postoji token
  useEffect(() => {
    const token = sessionStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        setUser({
          name: payload.name || payload.email,
          email: payload.email,
        });
      } catch (e) {
        console.error('Nevažeći token');
      }
    }
  }, []);

  const login = (token) => {
    sessionStorage.setItem('token', token); // Spremi token u sessionStorage
    const payload = JSON.parse(atob(token.split('.')[1]));
    setUser({
      name: payload.name || payload.email,
      email: payload.email,
    });
  };

  const logout = () => {
    sessionStorage.removeItem('token');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
