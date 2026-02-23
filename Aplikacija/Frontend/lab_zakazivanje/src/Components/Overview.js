import Tanstack from "./Tanstack.js";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faComputer, faPlus, faCalendarDays, faClockRotateLeft, faRectangleList, faHourglassHalf, faAnglesRight} from "@fortawesome/free-solid-svg-icons";

export default function Overview({ data, setData, rooms, selectedRoom, handleRoomChange, setMode, newlyAddedId, setNewlyAddedId, setSessionEdit }) {

    const sortedData = [...data].sort((a, b) => b.id - a.id);

    const nextData = sortedData.filter(x => x.status === "next");
    const activeData = sortedData.filter(x => x.status === "active");
    const fadingData = sortedData.filter(x => x.status === "fading");
    const plannedData = sortedData.filter(x => x.status === "planned");
    const finishedData = sortedData.filter(x => x.status === "finished");

    function handleSetNext(id) 
    {
        const updated = data.map(item =>
            item.id === id ? { ...item, status: "next" } : item
        );

        setData(updated);
    }

    function handleSetPlanned(id) 
    {
        const updated = data.map(item =>
            item.id === id ? { ...item, status: "planned" } : item
        );
        setData(updated);
    }

    function handleSetActive(id) 
    {
        const updated = data.map(item =>
            item.id === id ? { ...item, status: "active" } : item
        );
        setData(updated);
    }

    function handleViewResources(id) {
        console.log("Upravljaj resursima za:", id);
    }

    return (
        <div className='mainAktivnosti'>
            <div className='aktivnostiLeft'>
                <h1>Zakazivanje laboratorijskih vežbi</h1>

                <div className="room-select-container">
                    <label className="form-label fw-bold me-2">
                        <FontAwesomeIcon icon={faComputer} className="me-2" />
                        Izaberite prostoriju:
                    </label>

                    <select 
                        className="form-select"
                        value={selectedRoom}
                        onChange={(e) => handleRoomChange(e.target.value)}
                    >
                        {rooms.map((room) => (
                            <option 
                                key={room.id} 
                                value={room.id}>
                                {room.naziv}
                            </option>
                        ))}
                    </select>
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
                                <FontAwesomeIcon icon={faRectangleList} className="me-2" /> 
                                {fadingData.length > 0 ? "Trenutne aktivne sesije" : "Trenutno aktivna sesija"}
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
                />
            </div>
        </div>
    );
}