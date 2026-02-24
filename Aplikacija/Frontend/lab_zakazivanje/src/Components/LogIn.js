export default function LogIn()
{
    const prijavaKorisnika = async () =>
    {
        window.location.href = "https://localhost:7126/api/auth/authenticate-google";
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
    )
}