import Tanstack from "./Tanstack.js";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faComputer, faPlus, faCalendarDays, faClockRotateLeft, faRectangleList } from "@fortawesome/free-solid-svg-icons";

export default function Overview({ data, rooms, selectedRoom, handleRoomChange, setMode, newlyAddedId }) {

    const sortedData = [...data].sort((a, b) => b.id - a.id);

    const plannedData = sortedData.filter(x => x.status === "planned");
    const finishedData = sortedData.filter(x => x.status === "finished");

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

                <Tanstack
                    tableData={finishedData}
                    naslov={<><FontAwesomeIcon icon={faRectangleList} className="me-2" /> Pregled trenutnih aktivnosti</>}
                    enableFeatures={false}
                />

                <div className='dodajAktDiv'>
                    <button onClick={() => setMode("add")}>
                        <FontAwesomeIcon icon={faPlus} className="me-2" />
                    </button>
                    <h4>Zakažite novu aktivnost</h4>
                </div>
            </div>

            <div className='aktivnostiRight'>
                <Tanstack 
                    tableData={plannedData} 
                    naslov={<><FontAwesomeIcon icon={faCalendarDays} className="me-2" /> Planirane aktivnosti</>}
                    newlyAddedId={newlyAddedId} 
                />
                <Tanstack 
                    tableData={finishedData} 
                    naslov={<><FontAwesomeIcon icon={faClockRotateLeft} className="me-2" /> Istorija aktivnosti</>} 
                />
            </div>
        </div>
    );
}