import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../../src/Auth';

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light px-4">
      <Link className="navbar-brand" to="/">Aplikacija</Link>
      <div className="collapse navbar-collapse">
        <ul className="navbar-nav me-auto">
          <li className="nav-item">
            <Link className="nav-link" to="/">Početna</Link>
          </li>
          {user && (
            <li className="nav-item">
              <Link className="nav-link" to="/Users">Korisnici</Link>
            </li>
          )}
        </ul>
        <ul className="navbar-nav ms-auto">
          {user ? (
            <>
              <li className="nav-item me-3">
                <span className="navbar-text">👤 {user.name}</span>
              </li>
              <li className="nav-item">
                <button className="btn btn-outline-danger btn-sm" onClick={handleLogout}>
                  Logout
                </button>
              </li>
            </>
          ) : (
            <li className="nav-item">
              <Link className="btn btn-outline-primary btn-sm" to="/login">Login</Link>
            </li>
          )}
        </ul>
      </div>
    </nav>
  );
};

export default Navbar;
