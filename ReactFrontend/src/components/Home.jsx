import React, { useEffect, useState } from "react";

const Home = () => {
    const [userName, setUserName] = useState("");

    useEffect(() => {
        const token = sessionStorage.getItem("token");

        if (token) {
            try {
                const payload = JSON.parse(atob(token.split(".")[1]));
                const name = payload.name || payload.email || "Korisnik";
                setUserName(name);
            } catch (error) {
                console.error("Neispravan token");
                setUserName("gost");
            }
        } else {
            setUserName("gost");
        }
    }, []);

    return (
        <div className="container mt-5">
            <div className="text-center">
                <h1 className="display-4">Dobro došli{userName ? `, ${userName}` : ""}!</h1>
                <p className="lead">Ovo je početna stranica vaše aplikacije.</p>
            </div>
        </div>
    );
};

export default Home;
