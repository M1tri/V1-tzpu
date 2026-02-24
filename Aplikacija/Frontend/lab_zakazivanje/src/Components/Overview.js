import Tanstack from "./Tanstack.js";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faComputer, faPlus, faCalendarDays, faClockRotateLeft, faRectangleList, faHourglassHalf, faAnglesRight} from "@fortawesome/free-solid-svg-icons";
import Select from "react-select";
import { SessionView } from "../DTOs/session.js";

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
        const response = await fetch(`https://localhost:7213/api/vlr/PromoteAsNext?sessionId=${id}`,
            {
                method: "PUT"
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
        const response = await fetch(`https://localhost:7213/api/vlr/DemoteToPlanned?sessionId=${id}`,
            {
                method: "PUT"
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
        const response = await fetch(`https://localhost:7213/api/vlr/Activate?sessionId=${id}`,
            {
                method: "PUT"
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

    const handleClonedSession = async (id) => {
        
        const response = await fetch(`https://localhost:7213/api/sessions/CloneSession?sessionId=${id}`,
            {
                method: "POST"
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
        const confirmDelete = window.confirm(
            `Da li ste sigurni da želite da obrišete sesiju ?}"?`
        );
        if (!confirmDelete) return;

        const response = await fetch(`https://localhost:7213/api/sessions/DeleteSession?sessionId=${id}`,
            {
                method: "DELETE"
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

                <Tanstack 
                    tableData={plannedData} 
                    naslov={<><FontAwesomeIcon icon={faCalendarDays} className="me-2" /> Planirane aktivnosti</>}
                    newlyAddedId={newlyAddedId} 
                    setNewlyAddedId={setNewlyAddedId}
                    onEditClicked={(id) => {setSessionEdit(id); setMode("edit")}}
                    onSetNext={handleSetNext}
                />
                
                <Tanstack 
                    tableData={finishedData} 
                    naslov={<><FontAwesomeIcon icon={faClockRotateLeft} className="me-2" /> Istorija aktivnosti</>} 
                    onSessionCloned={handleClonedSession}
                    onSessionDeleted={handleDeletedSession}
                />
            </div>
        </div>
    );
}