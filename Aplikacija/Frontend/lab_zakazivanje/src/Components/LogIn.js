import { useEffect, useState } from "react";
import App from "../App";

export default function LogIn()
{
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    const prijavaKorisnika = async () =>
    {
        window.location.href = "https://localhost:7213/api/auth/authenticate-google";
    }

    useEffect(() => {
        const token = localStorage.getItem("jwt");

        if (token)
        {
            setIsLoggedIn(true);
            return;
        }
        
        const params = new URLSearchParams(window.location.search);
        const neWtoken = params.get("token");

        if (token)
        {
            localStorage.setItem("jwt", token);
            console.log("Imam tokena lmfao lol");
            window.history.replaceState({}, document.title, window.location.pathname);
            setIsLoggedIn(true);
        }
        else
        {
            console.log("nema token");
        }

    }, []);

    if (isLoggedIn) {
        return <App />;
    }

    return (
        <div className="oglasnaTabla">
            <div className="oglasnaTabla-header"></div>
            <div className="oglasnaTabla-paper2">
                <div className="loginPage">
                    <h1>Aplikacija za zakazivanje laboratorijskih vežbi</h1>
                    <div className="loginDiv">
                        <img src="/login.png" alt="login" />
                        <button
                            className="btnForm"
                            onClick={prijavaKorisnika}>
                            Prijavi se
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}