import React, { useState, useEffect } from 'react';
import { apiFetch } from '../api';
import { Link } from 'react-router-dom';

const UserList = ({ defaultRoleFilter = 'active', title = 'Korisnici' }) => 
  {
  const [users, setUsers] = useState([]);
  const [newUser, setNewUser] = useState({ name: '', surname: '', email: '', role: '', status: '' });
  const [searchInput, setSearchInput] = useState('');
  const [roleFilter, setRoleFilter] = useState(defaultRoleFilter);
  const [error, setError] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState(null);
  const [userToEdit, setUserToEdit] = useState(null); 
  

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await apiFetch(`https://localhost:7243/api/user/GetUsers`);
        if (!response.ok) {
          throw new Error('Neuspio dohvat korisnika');
        }
        const data = await response.json();
        setUsers(data);
      } catch (err) {
        setError(err.message);
      }
    };

    fetchUsers();
  }, [searchInput, roleFilter]);

  const handleAddUser = async () => {
    if (!newUser.name || !newUser.surname || !newUser.email || !newUser.role) return;
    try {
      const response = await apiFetch('https://localhost:7243/api/user/Add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newUser),
      });

      if (!response.ok) {
        throw new Error('Greška prilikom dodavanja korisnika');
      }
      const addedUser = await response.json();
      setUsers([...users, addedUser]);
      setNewUser({ name: '', surname: '', email: '', role: '', status: '' });
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDeleteUser = async () => {
    if (userToDelete) {
      try {
        const response = await apiFetch(`https://localhost:7243/api/user/Delete/${userToDelete.id}`, {
          method: 'DELETE',
        });

        if (!response.ok) {
          throw new Error('Greška prilikom brisanja korisnika');
        }

        setUsers(users.filter((user) => user.id !== userToDelete.id));
        setIsModalOpen(false);
        setUserToDelete(null);
      } catch (err) {
        setError(err.message);
      }
    }
  };

  const openDeleteModal = (user) => {
    setUserToDelete(user);
    setIsModalOpen(true);
  };

  const closeDeleteModal = () => {
    setIsModalOpen(false);
    setUserToDelete(null);
  };

  const openEditModal = (user) => {
    setUserToEdit(user);
    setIsModalOpen(true);
  };

  const closeEditModal = () => {
    setIsModalOpen(false);
    setUserToEdit(null);
  };

  const handleEditChange = (e) => {
    const { name, value } = e.target;
    setUserToEdit({
      ...userToEdit,
      [name]: value,
    });
  };

  const handleEditSubmit = async (e) => {
    e.preventDefault();

    if (!userToEdit.name || !userToEdit.surname || !userToEdit.email || !userToEdit.role) return;

    try {
      const response = await apiFetch(`https://localhost:7243/api/user/Update`, {
        method: 'Post',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userToEdit),
      });

      if (!response.ok) {
        throw new Error('Greška prilikom uređivanja korisnika');
      }

      setUsers(users.map((user) => (user.id === userToEdit.id ? userToEdit : user)));
      closeEditModal();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="container mt-5">
      <div className="d-flex justify-content-between">
      <h2>{title}</h2>
        <button className="btn btn-primary">Novi korisnik</button>
      </div>

      <div className="d-flex my-3">
        <Link to="/users/aktivni" className="btn btn-primary mx-1">Aktivni korisnici</Link>
        <Link to="/users/verzije" className="btn btn-primary mx-1">Prikazi sve verzije</Link>

        <div className="d-flex align-items-center mb-3">
          <input
            className="form-control mx-1"
            type="text"
            placeholder="Traži korisnika"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            style={{ width: '200px' }}
          />

          <select
            name="role"
            className="form-select mx-1"
            style={{ width: '200px' }}
            value={roleFilter}
            onChange={(e) => setRoleFilter(e.target.value)}
          >
            <option value="active">Aktivni korisnici</option>
            <option value="inactive">Deaktivirani korisnici</option>
            <option value="version">Sve verzije</option>
          </select>

          <button type="submit" className="btn btn-primary mx-1">
            Traži
          </button>
        </div>
      </div>

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Ime</th>
            <th>Prezime</th>
            <th>Email</th>
            <th>Uloga</th>
            <th>Status</th>
            <th>Uredi</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.name}</td>
              <td>{user.surname}</td>
              <td>{user.email}</td>
              <td>{user.role}</td>
              <td>{user.status}</td>
              <td>
                <button className="btn btn-primary" onClick={() => openEditModal(user)}>
                  Uredi
                </button>
                <button className="btn btn-danger" onClick={() => openDeleteModal(user)}>
                  Izbriši
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {isModalOpen && userToDelete && (
        <div className="modal" tabIndex="-1" style={{ display: 'block' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Potvrda brisanja</h5>
                <button type="button" className="btn-close" onClick={closeDeleteModal}></button>
              </div>
              <div className="modal-body">
                <p>Da li ste sigurni da želite obrisati korisnika <strong>{userToDelete?.name}</strong>?</p>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={closeDeleteModal}>
                  Zatvori
                </button>
                <button type="button" className="btn btn-danger" onClick={handleDeleteUser}>
                  Izbriši
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {isModalOpen && userToEdit && (
        <div className="modal" tabIndex="-1" style={{ display: 'block' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Uredi korisnika</h5>
                <button type="button" className="btn-close" onClick={closeEditModal}></button>
              </div>
              <div className="modal-body">
                <form onSubmit={handleEditSubmit}>
                  <div className="mb-3">
                    <label className="form-label">Ime</label>
                    <input
                      type="text"
                      className="form-control"
                      name="name"
                      value={userToEdit.name}
                      onChange={handleEditChange}
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label">Prezime</label>
                    <input
                      type="text"
                      className="form-control"
                      name="surname"
                      value={userToEdit.surname}
                      onChange={handleEditChange}
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label">Email</label>
                    <input
                      type="email"
                      className="form-control"
                      name="email"
                      value={userToEdit.email}
                      onChange={handleEditChange}
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label">Uloga</label>
                    <select
                      name="role"
                      className="form-select"
                      value={userToEdit.role}
                      onChange={handleEditChange}
                    >
                      <option value="User">User</option>
                      <option value="Admin">Admin</option>
                    </select>
                  </div>
                  <div className="mb-3">
                    <label className="form-label">Status</label>
                    <select
                      name="status"
                      className="form-select"
                      value={userToEdit.status}
                      onChange={handleEditChange}
                    >
                      <option value="Active">Active</option>
                      <option value="Inactive">Inactive</option>
                    </select>
                  </div>
                  <div className="mb-3">
                    <button type="submit" className="btn btn-primary">
                      Spremi
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      )}

      {error && <div className="alert alert-danger">{error}</div>}
    </div>
  );
};

export default UserList;
