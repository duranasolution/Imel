import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiFetch } from "../api";
import { useAuth } from "../Auth";

const LoginForm = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { login } = useAuth();  // Importuj login funkciju iz AuthContext

    const isValidEmail = (email) => {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    };

    const handleLogin = async (e) => {
        e.preventDefault();
        setError("");

        // Validacija inputa
        if (!email || !password) {
            setError("Sva polja su obavezna.");
            return;
        }

        if (!isValidEmail(email)) {
            setError("Unesite ispravnu email adresu.");
            return;
        }

        if (password.length < 3) {
            setError("Lozinka mora imati najmanje 6 karaktera.");
            return;
        }

        setLoading(true);

        try {
            const response = await apiFetch("https://localhost:7243/api/Auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ email, password })
            });

            if (!response.ok) {
                throw new Error("Neispravni podaci za prijavu.");
            }

            const data = await response.json();
            const token = data.token;

            sessionStorage.setItem("token", token);  // Pohranjujemo token u sessionStorage
            login(token);  // Pozivamo login funkciju iz AuthContext za pohranu korisnika
            navigate("/");

        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mt-5" style={{ maxWidth: "400px" }}>
            <h2 className="mb-4 text-center">Prijava</h2>

            {error && <div className="alert alert-danger">{error}</div>}

            <form onSubmit={handleLogin}>
                <div className="mb-3">
                    <label htmlFor="email" className="form-label">Email adresa</label>
                    <input
                        type="email"
                        className="form-control"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>

                <div className="mb-3">
                    <label htmlFor="password" className="form-label">Lozinka</label>
                    <input
                        type="password"
                        className="form-control"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>

                <button type="submit" className="btn btn-primary w-100" disabled={loading}>
                    {loading ? "Prijava..." : "Prijavi se"}
                </button>
            </form>
        </div>
    );
};

export default LoginForm;
