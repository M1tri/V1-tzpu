import './App.css';
import React, { useState, useEffect, act } from 'react';
import { SessionView } from './DTOs/session.js';
import { RoomView } from './DTOs/room.js';
import Overview from './Components/Overview.js';
import Form from './Components/Form.js';
import { ActivityView } from './DTOs/activity.js';
import SessionManager from './Components/SessionManager.js';
import LogIn from './Components/LogIn.js';
import useNotifications from './Components/SchedulerNotificationHook.js';
import { ConsoleLogger } from '@microsoft/signalr/dist/esm/Utils.js';

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
    const [manageSessionId, setManageSessionId] = useState(-1);

    useEffect(() => {
        async function fetchRooms() {
            try {
                const res = await fetch("https://localhost:7213/api/rooms/GetRooms");
                if (!res.ok) 
                    throw new Error("Greska pri ucitavanju prostorija");

                const json = await res.json();

                const loadedRooms = json.map(r => new RoomView(r.id, r.naziv, r.capacity, r.raspored));

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


    const fetchData = async function fetchData() {
        try 
        {
            const response = await fetch(`https://localhost:7213/api/sessions/GetSessionsInRoom?roomId=${selectedRoom}`);
            if (!response.ok) 
            {
                throw new Error("Greska pri ucitavanju sesija.");
            }
            const json = await response.json();
            console.log(json);

            const data = json.map(s => new SessionView
                (s.id, s.nazivAktivnosti, 
                s.tipAktivnosti, s.datum, 
                s.vremePocetka, 
                s.vremeKraja, 
                s.stanje, 
                s.automatskiPocetak,
                s.automatskiKraj,
                s.automatskoKrajnjeStanje));

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

    useEffect(() => {
        if(!selectedRoom)
            return;
        
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

    const handleRoomChange = (roomId) => 
    {
        setSelectedRoom(Number(roomId));
        setData([]);
        setLoading(true);
    };

    const handleSessionAdded = (session) =>
    {
        setData(prev => prev.filter(s => s.id != session.id));
        setData(prev => [...prev, session]);
        setNewlyAddedId(session.id);
    }

    const handleSessionDeleted = (id) => {
        setData(prev => prev.filter(s => s.id !== id));
    };

    const handleSetFading = async (id) => {
        try {
            const response = await fetch(`https://localhost:7213/api/vlr/Fade?sessionId=${id}`, {
                method: "PUT"
            });

            if (!response.ok) {
                alert(await response.text());
                return;
            }

            setData(prev =>
                prev.map(item =>
                    item.id === id ? { ...item, status: "fading" } : item
                )
            );

            setMode("list");

        } 
        catch (err) {
            console.error("Error fading session:", err);
        }
    };

    const handleSetFinished = async (id) => {
        try {
            const response = await fetch(`https://localhost:7213/api/vlr/Terminate?sessionId=${id}`, {
                method: "PUT"
            });

            if (!response.ok) {
                alert(await response.text());
                return;
            }

            setData(prev =>
                prev.map(item =>
                    item.id === id ? { ...item, status: "finished" } : item
                )
            );

            setMode("list");

        } 
        catch (err) 
        {
            console.error("Error finished session:", err);
        }
    };

    const HandleNotificationSuccess = (id) => {
        console.log("stiglooo");
        console.log(id);
        if (selectedRoom === id)
        {
            fetchData();
        }
    }
    
    const HandleNotificationFail = (message) => {
        alert(message);
    }
    useNotifications(HandleNotificationSuccess, HandleNotificationFail, selectedRoom);

    if (roomsLoading) return <div>Učitavanje prostorija...</div>;
    if (loading) return <div>Učitavanje sesija...</div>;

    let content;

    if (mode === "list")
    {
        content = (
            <Overview 
                data={data}
                setData={setData}
                rooms={rooms}
                selectedRoom={selectedRoom}
                handleRoomChange={handleRoomChange}
                setMode={setMode}
                newlyAddedId={newlyAddedId}
                setNewlyAddedId={setNewlyAddedId}
                setSessionEdit={setEditSessionId}
                setManageSessionId={setManageSessionId}
                onSessionAdded={handleSessionAdded}
                onSessionDeleted = {handleSessionDeleted}
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
                editSessionData={null}
                onSelectedRoom={setSelectedRoom}
            />
        )
    }
    else if (mode == "edit")
    {
        const editSession = data.find(s => s.id == editSessionId);

        content = (
            <Form 
                setMode={setMode}
                room={rooms.find(r => r.id === selectedRoom)}
                rooms = {rooms}
                activities={activities}
                onSessionAdded={handleSessionAdded}
                onSessionDeleted={handleSessionDeleted}
                editMode={true}
                editSessionData={editSession}
                onSelectedRoom={setSelectedRoom}
            />
        )       
    }
    else if (mode == "sessionManager")
    {
        const manageSession = data.find(s => s.id == manageSessionId);

        content = (
            <SessionManager
                session={manageSession}
                room={rooms.find(r => r.id === selectedRoom)}
                mode = {mode}
                setMode={setMode}
                onSetFading={handleSetFading}
                onSetFinished={handleSetFinished}
            />
        )
    }
    else if(mode == "login")
    {
        content = (<LogIn/>)
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
