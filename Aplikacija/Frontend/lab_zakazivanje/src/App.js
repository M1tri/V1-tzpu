import './App.css';
import React, { useState, useEffect, act } from 'react';
import { SessionView } from './DTOs/session.js';
import { RoomView } from './DTOs/room.js';
import Overview from './Components/Overview.js';
import Form from './Components/Form.js';
import { ActivityView } from './DTOs/activity.js';

function App() {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [rooms, setRooms] = useState([]);
    const [selectedRoom, setSelectedRoom] = useState(null);
    const [roomsLoading, setRoomsLoading] = useState(true);
    const [activities, setActivities] = useState([]);
    const [activitiesLoading, setActivitiesLoading] = useState(true);
    const [mode, setMode] = useState("list"); 
    const [newlyAddedId, setNewlyAddedId] = useState(null);
    const [editSessionId, setEditSessionId] = useState(-1);
    const [editSessionData, setEditSessionData] = useState(null);
    const [editSessionLoading, setEditSessionLoading] = useState(false);

    useEffect(() => {
        async function fetchRooms() {
            try {
                const res = await fetch("https://localhost:7213/api/rooms/GetRooms");
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
            const response = await fetch(`https://localhost:7213/api/sessions/GetSessionsInRoom?roomId=${selectedRoom}`);
            if (!response.ok) 
            {
                throw new Error("Greska pri ucitavanju sesija.");
            }
            const json = await response.json();
            console.log(json);

            const data = json.map(s => new SessionView(s.id, s.nazivAktivnosti, s.tipAktivnosti, s.datum, s.vremePocetka, s.vremeKraja, s.stanje));

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

    useEffect(() => {
        async function fetchActivities() {
            try {
                const res = await fetch("https://localhost:7213/api/activities/GetActivities");
                if (!res.ok) 
                    throw new Error("Greska pri ucitavanju aktivnosti");

                const json = await res.json();

                const loadedActivities = json.map(a => new ActivityView(a.id, a.naziv, a.tip, a.vlriDs));

                setActivities(loadedActivities);

            } 
            catch (err) 
            {
                console.error(err);
            } 
            finally 
            {
                setActivitiesLoading(false);
            }
        }

        fetchActivities();
    }, []);

    useEffect(() => {
        async function fetcSessionData(sessionId) {
            if (sessionId == -1)
            {
                setEditSessionData(null);
                return;
            }
            
            setEditSessionLoading(true);
            try {
                const res = await fetch(`https://localhost:7213/api/sessions/GetSession?sessionId=${sessionId}`);
                if (!res.ok) 
                    throw new Error("Greska pri pribavljanju sesije");

                const json = await res.json();

                const view = new SessionView(json.id, json.nazivAktivnosti, json.tipAktivnosti, json.datum, json.vremePocetka, json.vremeKraja, json.stanje);
                setEditSessionData(view);
            } 
            catch (err) 
            {
                console.error(err);
            } 
            finally 
            {
                setEditSessionLoading(false);
            }
        };
        
        fetcSessionData(editSessionId);
    }, [editSessionId]);

    const handleRoomChange = (roomId) => 
    {
        setSelectedRoom(Number(roomId));
        setData([]);
        setLoading(true);
    };

    const handleSessionAdded = (session) =>
    {
        setData(prev => [...prev, session]);
        setNewlyAddedId(session.id);
    }

    if (roomsLoading) return <div>Učitavanje prostorija...</div>;
    if (loading) return <div>Učitavanje sesija...</div>;
    if (mode == "edit" && editSessionLoading) return <div>Učitavanje sesija...</div>;

    let content;

    if (mode === "list")
    {
        content = (
            <Overview 
                data={data}
                rooms={rooms}
                selectedRoom={selectedRoom}
                handleRoomChange={handleRoomChange}
                setMode={setMode}
                newlyAddedId={newlyAddedId}
                setNewlyAddedId={setNewlyAddedId}
                setSessionEdit={setEditSessionId}
            />
        );
    }
    else if (mode == "add")
    {
        content = (
            <Form 
                setMode={setMode}
                room={rooms.find(r => r.id === selectedRoom)}
                rooms = {rooms}
                activities={activities}
                onSessionAdded={handleSessionAdded}
                editMode={false}
                editSessionId={null}
            />
        )
    }
    else if (mode == "edit")
    {
        if (!editSessionData)
            content = (<div>Ucitavanja podataka sesije</div>);
        else {
            content = (
                <Form 
                    setMode={setMode}
                    room={rooms.find(r => r.id === selectedRoom)}
                    rooms = {rooms}
                    activities={activities}
                    onSessionAdded={handleSessionAdded}
                    editMode={true}
                    editSessionData={editSessionData}
                />
            )     
        }   
    }

    return (
        <div className="oglasnaTabla">
            <div className="oglasnaTabla-header"></div>
            <div className="oglasnaTabla-paper">
                {content}
            </div>
        </div>
    );
}

export default App;
