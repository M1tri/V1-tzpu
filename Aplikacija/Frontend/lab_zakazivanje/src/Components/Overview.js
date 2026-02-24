import Tanstack from "./Tanstack.js";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faComputer, faPlus, faCalendarDays, faClockRotateLeft, faRectangleList, faHourglassHalf, faAnglesRight} from "@fortawesome/free-solid-svg-icons";
import Select from "react-select";
import { SessionView } from "../DTOs/session.js";
import Message from "./Message.js";

export default function Overview({ data, setData, rooms, selectedRoom, handleRoomChange, setMode, newlyAddedId, setNewlyAddedId, setSessionEdit, setManageSessionId, onSessionAdded, onSessionDeleted }) {

    const sortedData = [...data].sort((a, b) => b.id - a.id);

    const nextData = sortedData.filter(x => x.status === "next");
    const activeData = sortedData.filter(x => x.status === "active");
    const fadingData = sortedData.filter(x => x.status === "fading");
    const plannedData = sortedData.filter(x => x.status === "planned");
    const finishedData = sortedData.filter(x => x.status === "finished");

    const optionsR = rooms.map(room => ({
        value: room.id,
        label: room.naziv
    }));

    const handleSetNext = async (id) =>
    {
        const token = localStorage.getItem("jwt");
        if (!token)
        {
            console.log("Nema token!");
            return;
        }

        const response = await fetch(`https://localhost:7213/api/vlr/PromoteAsNext?sessionId=${id}`,
            {
                method: "PUT",
                headers: {
                        "Authorization" : "Bearer " + token,
                    }
            });

        if (!response.ok)
        {
            alert(await response.text());
            return;
        }

        const updated = data.map(item =>
            item.id === id ? { ...item, status: "next" } : item
        );

        setData(updated);
    }

    const handleSetPlanned = async (id) =>
    {
        const token = localStorage.getItem("jwt");
        if (!token)
        {
            console.log("Nema token!");
            return;
        }

        const response = await fetch(`https://localhost:7213/api/vlr/DemoteToPlanned?sessionId=${id}`,
            {
                method: "PUT",
                headers: {
                        "Authorization" : "Bearer " + token,
                    }
            });

        if (!response.ok)
        {
            alert(await response.text());
            return;
        }

        const updated = data.map(item =>
            item.id === id ? { ...item, status: "planned" } : item
        );
        setData(updated);
    }

    const handleSetActive = async (id) => 
    {
        const token = localStorage.getItem("jwt");
        if (!token)
        {
            console.log("Nema token!");
            return;
        }

        const response = await fetch(`https://localhost:7213/api/vlr/Activate?sessionId=${id}`,
            {
                method: "PUT",
                headers: {
                        "Authorization" : "Bearer " + token,
                    }
            });

        if (!response.ok)
        {
            alert(await response.text());
            return;
        }

        const updated = data.map(item =>
            item.id === id ? { ...item, status: "active" } : item
        );
        setData(updated);
    }

    function handleViewResources(id) 
    {
        setManageSessionId(id);
        setMode("sessionManager");
    }

    const handleClonedSession = async (id) => 
    {
        const token = localStorage.getItem("jwt");
        if (!token)
        {
            console.log("Nema token!");
            return;
        }

        const response = await fetch(`https://localhost:7213/api/sessions/CloneSession?sessionId=${id}`,
            {
                method: "POST", 
                headers: {
                        "Authorization" : "Bearer " + token,
                    }
            });

        if (!response.ok)
        {
            alert(await response.text());
            return;
        }

        const savedSession = await response.json();
        
        onSessionAdded(new SessionView(
            savedSession.id,
            savedSession.nazivAktivnosti,
            savedSession.tipAktivnosti,
            savedSession.datum,
            savedSession.vremePocetka,
            savedSession.vremeKraja,
            savedSession.stanje,
            savedSession.automatskiPocetak,
            savedSession.automatskiKraj,
            savedSession.automatskoKrajnjeStanje
        ));
    };

    const handleDeletedSession = async (id) => {

        const session = data.find(s => s.id == id);

        const confirmDelete = window.confirm(
            `Da li ste sigurni da želite da obrišete sesiju ${session.naziv}?`
        );
        if (!confirmDelete) return;

        const token = localStorage.getItem("jwt");
        if (!token)
        {
            console.log("Nema token!");
            return;
        }

        const response = await fetch(`https://localhost:7213/api/sessions/DeleteSession?sessionId=${id}`,
            {
                method: "DELETE", 
                headers: {
                        "Authorization" : "Bearer " + token,
                    }
            });

        if (!response.ok)
        {
            alert(await response.text());
            return;
        }

        onSessionDeleted(id);
    };


    return (
        <div className='mainAktivnosti'>
            <div className='aktivnostiLeft'>
                <h1>Zakazivanje laboratorijskih vežbi</h1>

                <div className="room-select-container">
                    <label className="form-label fw-bold me-2">
                        <FontAwesomeIcon icon={faComputer} className="me-2" />
                        Izaberite prostoriju:
                    </label>

                    <Select
                        options={optionsR}
                        value={optionsR.find(o => o.value === selectedRoom)}
                        onChange={(selectedOption) => handleRoomChange(selectedOption.value)}
                        isSearchable={true}
                    />
                </div>

                <div className="trAkt">
                    <FontAwesomeIcon icon={faRectangleList} className="me-2" /> 
                    <h4 className="m-0 trAktNaslov">Pregled trenutnih aktivnosti</h4>
                </div>

                {nextData.length == 0 && activeData.length == 0 && fadingData.length == 0 && (
                    <Message
                        message = {"Nema trenutno aktivnih sesija."}
                        photo = {"/trenutne.png"}
                        klasa={"messageDiv"}
                    />
                )
                }

                {nextData.length > 0 && ( <Tanstack
                    tableData={nextData}
                    naslov={<><FontAwesomeIcon icon={faAnglesRight} className="me-2" /> Sledeća sesija</>}
                    enableFeatures={false}
                    onSetActive={handleSetActive}
                    onSetPlanned={handleSetPlanned}
                />)}

                {(activeData.length > 0 || fadingData.length > 0) && (
                    <Tanstack
                        tableData={[...activeData, ...fadingData]}
                        naslov={
                        <>
                            <FontAwesomeIcon icon={faHourglassHalf} className="me-2" /> 
                            {fadingData.length > 0 ? "Trenutno aktivne sesije" : "Trenutno aktivna sesija"}
                        </>
                        }
                        enableFeatures={false}
                        onViewResources={handleViewResources}
                    />
                )}

            </div>

            <div className='aktivnostiRight'>
                <div className='dodajAktDiv'>
                    <button onClick={() => setMode("add")}>
                        <FontAwesomeIcon icon={faPlus} className="me-2" />
                    </button>
                    <h4>Zakažite novu aktivnost</h4>
                </div>

                {plannedData.length == 0  && (
                    <div className="slikaDiv">
                        <div className="trAkt">
                            <FontAwesomeIcon icon={faCalendarDays} className="me-2" /> 
                            <h4 className="m-0 trAktNaslov">Planirane aktivnosti</h4>
                        </div>
                        <Message
                            message = {"Nema planiranih sesija."}
                            photo = {"/planned.png"}
                            klasa = {"nijeTrSlika"}
                        />
                    </div>
                )}

                {plannedData.length > 0 && (<Tanstack 
                    tableData={plannedData} 
                    naslov={<><FontAwesomeIcon icon={faCalendarDays} className="me-2" /> Planirane aktivnosti</>}
                    newlyAddedId={newlyAddedId} 
                    setNewlyAddedId={setNewlyAddedId}
                    onEditClicked={(id) => {setSessionEdit(id); setMode("edit")}}
                    onSetNext={handleSetNext}
                />)}

                {finishedData.length == 0  && (
                    <div className="slikaDiv">
                        <div className="trAkt">
                            <FontAwesomeIcon icon={faClockRotateLeft} className="me-2" /> 
                            <h4 className="m-0 trAktNaslov">Istorija aktivnosti</h4>
                        </div>
                        <Message
                            message = {"Nema završenih sesija."}
                            photo = {"/finished.png"}
                            klasa = {"nijeTrSlika"}
                        />
                    </div>
                )}
                
                {finishedData.length > 0 && (<Tanstack 
                    tableData={finishedData} 
                    naslov={<><FontAwesomeIcon icon={faClockRotateLeft} className="me-2" /> Istorija aktivnosti</>} 
                    onSessionCloned={handleClonedSession}
                    onSessionDeleted={handleDeletedSession}
                />)}
            </div>
        </div>
    );
}