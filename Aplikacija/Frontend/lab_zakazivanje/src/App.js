import './App.css';
import Tanstack from './Components/Tanstack.js';
import React, { useState, useEffect } from 'react';
import { SessionView } from './DTOs/session.js';
import { RoomView } from './DTOs/room.js';

function App() {
    const [data, setData] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [selectedRoom, setSelectedRoom] = useState(null);
    const [loading, setLoading] = useState(true);
    const [roomsLoading, setRoomsLoading] = useState(true);

    useEffect(() => {
        async function fetchRooms() {
            try {
                const res = await fetch("http://localhost:5131/api/rooms/GetRooms");
                if (!res.ok) 
                    throw new Error("Greska pri ucitavanju prostorija");

                const json = await res.json();

                const loadedRooms = json.map(r => new RoomView(r.id, r.naziv, r.kapacitet, r.raspored));

                setRooms(loadedRooms);

                if (loadedRooms.length > 0) {
                    setSelectedRoom(loadedRooms[0].id);
                }

            } catch (err) 
            {
                console.error(err);
            } finally 
            {
                setRoomsLoading(false);
            }
        }

        fetchRooms();
    }, []);

    useEffect(() => {

        if(!selectedRoom)
            return;

        async function fetchData() {
        try 
        {
            const response = await fetch(`http://localhost:5131/api/sessions/GetSessionsInRoom?roomId=${selectedRoom}`);
            if (!response.ok) 
            {
                throw new Error("Greska pri ucitavanju sesija.");
            }
            const json = await response.json();
            console.log(json);

            const data = json.map(s => new SessionView(s.id, s.nazivAktivnosti, s.nazivAktivnosti, s.datum, s.vremePocetka, s.vremeKraja, s.stanje));

            console.log(data);
            setData(data);
        } 
        catch (error) 
        {
            console.error("Fetch error:", error);
        } 
        finally 
        {
            setLoading(false);
        }
        }

        fetchData();
    }, [selectedRoom]);

    const handleRoomChange = (roomId) => {
        setSelectedRoom(roomId);
        setData([]);
        setLoading(true);
    };

    if (roomsLoading) return <div>Učitavanje prostorija...</div>;
    if (loading) return <div>Učitavanje aktivnosti...</div>;

    const plannedData = data.filter(session => session.status === "planned");
    const finishedData = data.filter(session => session.status === "finished");
  
    /*const data = [
        {
            aktivnost: "AOR LV1",
            klasa: "AOR LAB VEZBA",
            datum: "2026-11-09",
            vremePoc: "18:00:00",
            vremeKraja: "19:30:00",
            status: "active"
        },
        {
            aktivnost: "AIP LV3",
            klasa: "AIP LAB VEZBA",
            datum: "2026-02-20",
            vremePoc: "14:00:00",
            vremeKraja: "15:30:00",
            status: "fading"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-06-25",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "planned"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-01-02",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "next"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-01-02",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "finished"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-06-25",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "planned"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-01-02",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "next"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-01-02",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "finished"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-01-02",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "next"
        },
        {
            aktivnost: "UUR LV5",
            klasa: "UUR LAB VEZBA",
            datum: "2026-01-02",
            vremePoc: "08:00:00",
            vremeKraja: "09:00:00",
            status: "finished"
        }
    ];*/

    return (
        <div className="oglasnaTabla">
        <div className="oglasnaTabla-header"></div>
        <div className="oglasnaTabla-paper">
            <h1>Zakazivanje laboratorijskih vežbi</h1>
            <div className='mainAktivnosti'>
                <div className='aktivnostiLeft'>
                    <div className="mb-3">
                        <label htmlFor="roomSelect" className="form-label fw-bold me-2">
                            Izaberite prostoriju:
                        </label>
                        <select
                            id="roomSelect"
                            className="form-select"
                            value={selectedRoom || ""}
                            onChange={(e) => handleRoomChange(e.target.value)}
                        >
                            <option value="">Odaberite...</option>
                            {rooms.map((room) => (
                            <option key={room.id} value={room.id}>
                                {room.naziv}
                            </option>
                            ))}
                        </select>
                        </div>
                    <div>
                        <Tanstack tableData={data} naslov="Pregled aktivnosti" enableFeatures = {false}/>
                    </div>
                </div>
                <div className='aktivnostiRight'>
                    <div>
                        <Tanstack tableData={plannedData} naslov="Planirane aktivnosti"/>
                    </div>
                    <div>
                        <Tanstack tableData={finishedData} naslov="Istorija aktivnosti"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
    );
}

export default App;
