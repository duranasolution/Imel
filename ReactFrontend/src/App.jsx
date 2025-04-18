import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import Users from './components/Users';
import Login from './components/Login';
import Home from './components/Home';
import { Route } from 'react-router-dom';
import { BrowserRouter as Router, Routes } from 'react-router-dom';
import Navbar from './components/partials/nav';
import LoginForm from './components/Login';
import UserList from './components/Users';
import ActiveUsers from './components/ActiveUsers';
import InactiveUsers from './components/InactiveUsers';
import UserVersions from './components/UserVersions';

function App() {

  return (
    <Router>
      <Navbar/>
      <Routes>
      <Route path="/login" element={<LoginForm />} />
        <Route path="/users" element={<UserList />} />
        <Route path="/" element={<Home />} /> 
        <Route path="/users/aktivni" element={<ActiveUsers />} />
        <Route path="/users/neaktivni" element={<InactiveUsers />} />
        <Route path="/users/verzije" element={<UserVersions />} />
      </Routes>
    </Router>
  );
}

export default App
