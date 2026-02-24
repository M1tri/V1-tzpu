export default function LogIn()
{
    const prijavaKorisnika = async () =>
    {
        window.location.href = "https://localhost:7126/api/auth/authenticate-google";
    }

    return (
        <div>
            <button
                onClick={prijavaKorisnika}
            >
                Prijavi se
            </button>
        </div>
    )
}