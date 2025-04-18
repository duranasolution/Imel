import React, { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom'; // Uvozimo useHistory za redirekciju

const EditUser = ({ userId }) => {
    const [user, setUser] = useState({
        id: '',
        name: '',
        surname: '',
        email: '',
        role: '',
        status: ''
    });
    const history = useHistory(); // Inicijalizacija useHistory

    // Fetch korisnika prilikom učitavanja stranice
    useEffect(() => {
        const fetchUserData = async () => {
            const response = await fetch(`/api/users/${userId}`); // API endpoint za fetch korisnika
            const data = await response.json();
            setUser(data);
        };
        
        fetchUserData();
    }, [userId]);

    // Rukovanje promenama u formi
    const handleChange = (e) => {
        const { name, value } = e.target;
        setUser({
            ...user,
            [name]: value
        });
    };

    // Funkcija za slanje podataka na server
    const handleSubmit = async (e) => {
        e.preventDefault();
        
        const response = await fetch(`/api/users/${userId}`, {
            method: 'PUT', // PUT za ažuriranje korisnika
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        if (response.ok) {
            alert('Korisnik je uspešno ažuriran!');
            history.push(`/user/${userId}`); // Redirekcija na stranicu korisnika nakon uspešnog ažuriranja
        } else {
            alert('Došlo je do greške prilikom ažuriranja korisnika.');
        }
    };

    return (
        <div className="container mt-5">
            <div className="card shadow rounded">
                <div className="card-header bg-primary text-white">
                    <h4>Detalji korisnika</h4>
                </div>
                <div className="card-body">
                    <form onSubmit={handleSubmit}>
                        <input name="id" type="hidden" value={user.id} />
                        <div className="mb-3">
                            <label className="form-label">Ime</label>
                            <input 
                                type="text" 
                                className="form-control" 
                                name="name" 
                                value={user.name} 
                                onChange={handleChange} 
                            />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Prezime</label>
                            <input 
                                type="text" 
                                className="form-control" 
                                name="surname" 
                                value={user.surname} 
                                onChange={handleChange} 
                            />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Email</label>
                            <input 
                                type="email" 
                                className="form-control" 
                                name="email" 
                                value={user.email} 
                                onChange={handleChange} 
                            />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Uloga</label>
                            <select 
                                name="role" 
                                className="form-select"
                                value={user.role} 
                                onChange={handleChange}
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
                                value={user.status} 
                                onChange={handleChange}
                            >
                                <option value="Active">Active</option>
                                <option value="Inactive">Inactive</option>
                            </select>
                        </div>
                        <div className="mb-3">
                            <button type="submit" className="btn btn-primary">Spremi</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default EditUser;
