import { useEffect, useState } from "react";
import Select from "react-select";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faXmark, faCheck } from "@fortawesome/free-solid-svg-icons";
import { SessionView } from "../DTOs/session";

export default function Form({ setMode, room, rooms, activities, onSessionAdded, editMode, editSessionData}) {

    const [date, setDate] = useState("");
    const [startTime, setStartTime] = useState("12:00");
    const [endTime, setEndTime] = useState("13:30");

    const [selectedActivity, setSelectedActivity] = useState(null);

    const [autoStart, setAutoStart] = useState(null);
    const [autoEnd, setAutoEnd] = useState(null);
    const [autoState, setAutoState] = useState(null); 

    const [selectedRoom, setSelectedRoom] = useState({value: room.id, label: room.naziv});

    const optionsA = activities.map(a => ({ value: a.id, label: a.naziv, tip: a.tip }));
    const optionsR = rooms.map(r => ({ value: r.id, label: r.naziv}));

    useEffect(() => {
        if (!editMode || !editSessionData)
            return;

        setDate(editSessionData.datum);
        setStartTime(editSessionData.vremePoc.slice(0, 5));
        setEndTime(editSessionData.vremeKraja.slice(0,5));

        const activity = activities.find(a => a.naziv == editSessionData.naziv);

        if (activity) {
            setSelectedActivity({
                value: activity.id,
                label: activity.naziv,
                tip: activity.tip
            });
        }

    }, [editMode, editSessionData, rooms, activities]);

    const isFormValid = () => {
        return (
            selectedActivity !== null &&
            date !== "" &&
            startTime !== "" &&
            endTime !== "" &&
            autoStart !== null &&
            autoEnd !== null &&
            (autoEnd === "no" || (autoEnd === "yes" && autoState !== null))
        );
    };

    const handleSave = async () => {
        const data = 
        {
            roomId: selectedRoom.value,
            aktivnostId: selectedActivity.value,
            datum: date,
            vremePocetka: startTime,
            vremeKraja: endTime,
            automatskiPocetak: autoStart === "yes",
            automatskiKraj: autoEnd === "yes",
            automatskoKrajnjeStanje: autoEnd === "yes" ? Number(autoState) : 0
        };

        console.log(data);

        try 
        {
            const response = await fetch("http://localhost:5131/api/sessions/AddSession", 
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            });

            if (!response.ok) 
            {
                console.log(await response.text())
                throw new Error("Nije se dodala sesija.");
            }

            const savedSession = await response.json();

            onSessionAdded(new SessionView(
                savedSession.id,
                savedSession.nazivAktivnosti,
                savedSession.tipAktivnosti,
                savedSession.datum,
                savedSession.vremePocetka,
                savedSession.vremeKraja,
                savedSession.stanje
            ));

            setMode("list");

        } 
        catch (err) 
        {
            console.error(err);
            alert("Doslo je do greske prilikom cuvanja sesije.");
        }
    };

    return (
        <div className="formaWrapper">
            <h1>Dodavanje nove aktivnosti - Prostorija {selectedRoom.label}</h1>
            <div className="formaDiv">
                <div className="formaLeft">

                    <label className="form-label">Izaberite prostoriju:</label>
                    <Select
                        options={optionsR}
                        value={selectedRoom}
                        onChange={setSelectedRoom}
                        isSearchable={true}
                    />

                    <label className="form-label">Izaberite aktivnost:</label>
                    <Select
                        options={optionsA}
                        value={selectedActivity}
                        onChange={setSelectedActivity}
                        isSearchable={true}
                        placeholder={editMode ? null : "🔍︎ Pretraži aktivnosti..."}
                    />

                    <label className="form-label">Tip aktivnosti:</label>
                    <input 
                        type="text" 
                        className="form-control search-box" 
                        disabled={true} 
                        value={selectedActivity ? selectedActivity.tip : ""}/>

                    <label className="form-label">Datum održavanja:</label>
                    <input 
                        type="date" 
                        className="form-control search-box" 
                        value={date} 
                        onChange={(e) => setDate(e.target.value)}
                    />

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
                        value={endTime}
                        onChange={(e) => setEndTime(e.target.value)}
                    />

                    <label className="form-label">Da li se aktivnost automatski aktivira u vreme početka?</label>
                    <div className="radioButtons">
                        <div>
                            <input 
                                type="radio" 
                                name="autoStart" 
                                value="yes" 
                                checked={autoStart === "yes"} 
                                onChange={() => setAutoStart("yes")} 
                            />
                            <label>Da</label>
                        </div>
                        <div>
                        <input 
                            type="radio" 
                            name="autoStart" 
                            value="no" 
                            checked={autoStart === "no"} 
                            onChange={() => setAutoStart("no")} 
                        />
                        <label>Ne</label>
                        </div>
                    </div>

                    <label className="form-label">Da li aktivnost automatski menja stanje u vreme kraja?</label>
                    <div className="radioButtons">
                        <div>
                            <input 
                                type="radio" 
                                name="autoEnd" 
                                value="yes" 
                                checked={autoEnd === "yes"} 
                                onChange={() => setAutoEnd("yes")} 
                            />
                            <label>Da</label>
                        </div>
                        <div>
                            <input 
                                type="radio" 
                                name="autoEnd" 
                                value="no" 
                                checked={autoEnd === "no"} 
                                onChange={() => setAutoEnd("no")} 
                            />
                            <label>Ne</label>
                        </div>
                    </div>

                    {autoEnd === "yes" && (
                        <>
                            <label className="form-label">Aktivnost prelazi u stanje: </label>
                            <div className="radioButtons">
                                <div>
                                <input 
                                    type="radio" 
                                    name="autoState" 
                                    value="3"
                                    checked={autoState === "3"} 
                                    onChange={() => setAutoState("3")} 
                                />
                                <label>FADING</label>
                                </div>
                                <div>
                                    <input 
                                        type="radio" 
                                        name="autoState" 
                                        value="4"
                                        checked={autoState === "4"} 
                                        onChange={() => setAutoState("4")} 
                                    />
                                    <label>FINISHED</label>
                                </div>
                            </div>
                        </>
                    )}

                    <button 
                        className="btn btn-primary btnForm"
                        onClick={handleSave}
                        disabled={!isFormValid()}
                    >
                    <>Sačuvaj <FontAwesomeIcon icon={faCheck} className="me-2" /></>
                    </button>

                    <button 
                        className="btn btn-secondary btnForm"
                        onClick={() => setMode("list")}
                    >
                    <>Odustani <FontAwesomeIcon icon={faXmark} className="me-2" /></>
                    </button>
                </div>

            <div className="addRight">
                <h3>Dodatne informacije</h3>
                <p>Ovde može da ide preview, kalendar, uputstva...</p>
            </div>
            </div>
        </div>
    );
}