import { useState } from "react";

export default function Form({ setMode, roomName }) {

    const [startTime, setStartTime] = useState("12:00");

    return (
        <div className="formaWrapper">
            <h1>Dodavanje nove aktivnosti - Prostorija {roomName}</h1>
            <div className="formaDiv">
                <div className="formaLeft">

                    <label className="form-label">Izaberite aktivnost:</label>
                    <input type="text" className="form-control search-box" />

                    <label className="form-label">Tip aktivnosti:</label>
                    <input type="text" className="form-control search-box" disabled="true" />

                    <label className="form-label">Datum održavanja:</label>
                    <input type="date" className="form-control search-box" />

                    <label className="form-label">Vreme početka:</label>
                    <input
                        type="time"
                        className="form-control search-box"
                        value={startTime}
                        onChange={(e) => setStartTime(e.target.value)}
                    />

                    <label className="form-label">Vreme kraja:</label>
                    <input
                        type="time"
                        className="form-control search-box"
                        value={startTime}
                        onChange={(e) => setStartTime(e.target.value)}
                    />

                    <label className="form-label">Da li se aktivnost automatski aktivira u vreme početka?</label>
                    <div className="radioButtons">
                        <div>
                            <input type="radio" name="autoStart"/>
                            <label>Da</label>
                        </div>
                        <div>
                            <input type="radio" name="autoStart"/>
                            <label>Ne</label>
                        </div>
                    </div>

                    <label className="form-label">Da li aktivnost automatski menja stanje u vreme kraja?</label>
                    <div className="radioButtons">
                        <div>
                            <input type="radio" name="autoEnd"/>
                            <label>Da</label>
                        </div>
                        <div>
                            <input type="radio" name="autoEnd"/>
                            <label>Ne</label>
                        </div>
                    </div>

                    <label className="form-label">Aktivnost prelazi u stanje: </label>
                    <div className="radioButtons">
                        <div>
                            <input type="radio" name="autoState"/>
                            <label>FADING</label>
                        </div>
                        <div>
                            <input type="radio" name="autoState"/>
                            <label>FINISHED</label>
                        </div>
                    </div>

                    <div className="d-flex gap-3 mt-3">
                        <button 
                            className="btn btn-primary"
                            onClick={() => setMode("list")}
                        >
                            Sačuvaj
                        </button>

                        <button 
                            className="btn btn-secondary"
                            onClick={() => setMode("list")}
                        >
                            Odustani
                        </button>
                    </div>
                </div>

            <div className="addRight">
                <h3>Dodatne informacije</h3>
                <p>Ovde može da ide preview, kalendar, uputstva...</p>
            </div>
            </div>
        </div>
    );
}