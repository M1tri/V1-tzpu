import RoomLayout from "./RoomLayout";
import { parseDate, parseTime } from "../Utils/helper";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleInfo, faToggleOn, faCircleChevronLeft } from "@fortawesome/free-solid-svg-icons";
import { useState, useEffect } from "react";

export default function SessionManager({session, room, mode, setMode, onSetFading, onSetFinished})
{
    const [selectedSeatIDs, setSelectedSeatIDs] = useState([]);
    const [VLRStatuses, setVLRStatuses] = useState({});
    const [VLRStatusesLoading, setVLRStatusesLoading] = useState(true);
    const [freeUserId, setFreeUserId] = useState(1);

    const allSeatIDs = room.raspored
    .flat()
    .filter(x => x.seatID && x.seatIP)
    .map(x => x.seatID);

    function toggleSeat(seatID) 
    {
        setSelectedSeatIDs(prev => prev.includes(seatID) ? prev.filter(id => id !== seatID)   : [...prev, seatID]);
    }

    function selectAllSeats() 
    {
        setSelectedSeatIDs(allSeatIDs);
    }

    function unselectAllSeats() 
    {
        setSelectedSeatIDs([]);
    }

    const allSelected = selectedSeatIDs.length === allSeatIDs.length;

    useEffect(() => {
        async function fetchStatuses() {
            try {
                const res = await fetch(`https://localhost:7213/api/sessions/GetSessionResourceStatus?sessionId=${session.id}`);
                if (!res.ok) 
                    throw new Error("Greska pri ucitavanju statusa");

                const json = await res.json();

                for (const seat in json)
                {
                    VLRStatuses[seat]={status: json[seat].vlrStatus, userId : json[seat].userId};
                }

                setVLRStatuses(VLRStatuses);

            } catch (err) 
            {
                console.error(err);
            } finally 
            {
                setVLRStatusesLoading(false);
            }
        }

        fetchStatuses();
    }, []);

    const PrepareResources = async () => {
        for (const seatId of selectedSeatIDs) {
            if (VLRStatuses[seatId].status != "null")
            {
                alert(`Mesto ${seatId}: Ova radnja primenjiva je samo na NULL resurse`);
                return;
            }

            const user = freeUserId;
            setFreeUserId(freeUserId + 1);

            const response = await fetch(`https://localhost:7213/api/vlr/Prepare?sessionId=${session.id}&seatId=${seatId}&roomId=${room.id}`,
            {
                method: "PUT"
            });

            if (!response.ok)
            {
                alert(await response.text());
                continue;
            }
            
            const data = await response.json();
            console.log(data);

            setVLRStatuses(prev => {
                const updated = { ...prev };
                updated[seatId].status = data.status;
                updated[seatId].userId = null;
                return updated;
            });
        }
    }

    const ProvideResources = async () => {
        for (const seatId of selectedSeatIDs) {
            const user = freeUserId;
            setFreeUserId(freeUserId + 1);

            const response = await fetch(`https://localhost:7213/api/vlr/Provide?sessionId=${session.id}&seatId=${seatId}&userId=${user}`,
            {
                method: "PUT"
            });

            if (!response.ok)
            {
                alert(await response.text());
                continue;
            }

            const data = await response.json();
            console.log(data);
            
            setVLRStatuses(prev => {
                const updated = { ...prev };
                updated[seatId].status = data.status;
                updated[seatId].userId = user;
                return updated;
            });
        }
    }

    const ReleaseResources = async () => {
        console.log(VLRStatuses);

        for (const seatId of selectedSeatIDs) {
            const user = VLRStatuses[seatId].userId;

            if (user == null)
            {
                alert("Nema lol");
                continue;
            }

            const response = await fetch(`https://localhost:7213/api/vlr/Release?sessionId=${session.id}&seatId=${seatId}&userId=${user}`,
            {
                method: "PUT"
            });

            if (!response.ok)
            {
                alert(await response.text());
                continue;
            }

            const data = await response.json();
            console.log(data);

            setVLRStatuses(prev => {
                const updated = { ...prev };
                updated[seatId].status = data.status;
                updated[seatId].userId = null;
                return updated;
            });
        }
    }

    const KillResources = async () => {
        for (const seatId of selectedSeatIDs) {
            if (VLRStatuses[seatId].status == "null")
            {
                alert(`Mesto ${seatId}: Ova radnja nije primenjiva na NULL resurse`);
                return;
            }

            const response = await fetch(`https://localhost:7213/api/vlr/Kill?sessionId=${session.id}&seatId=${seatId}`,
            {
                method: "PUT"
            });

            if (!response.ok)
            {
                alert(await response.text());
                continue;
            }
            
            const data = await response.json();
            console.log(data);

            setVLRStatuses(prev => {
                const updated = { ...prev };
                updated[seatId].status = data.status;
                updated[seatId].userId = null;
                return updated;
            });
        }
    }

    if (VLRStatusesLoading) return <div>Učitavanje statusa...</div>;

    return (
        <div>
            <h1>Upravljanje resursima</h1>
            <div className="manager">
                <div className="managerLeft">
                    <div className="info">
                        <FontAwesomeIcon icon={faCircleInfo} className="me-2" />
                        <h4>Osnovne informacije o sesiji</h4>
                    </div>
                    <div className="sesijaInfo">
                        <label>Naziv aktivnosti:</label>
                        <label className="infoLabel">{session.naziv}</label>

                        <label>Tip aktivnosti:</label>
                        <label className="infoLabel">{session.tip}</label>

                        <label>Datum održavanja:</label>
                        <label className="infoLabel">{parseDate(session.datum)}</label>

                        <label>Vreme početka:</label>
                        <label className="infoLabel">{parseTime(session.datum, session.vremePoc)}</label>

                        <label>Vreme kraja:</label>
                        <label className="infoLabel">{parseTime(session.datum, session.vremeKraja)}</label>

                        <label>Status:</label>
                        <label className="infoLabel">{session.status.toUpperCase()}</label>

                        <label>Automatski prelazi u krajnje stanje:</label>
                        <label className="infoLabel">{session.autoEnd ? "Da" : "Ne"}</label>

                        {session.autoEnd && (
                            <>
                                <label>Prelazi u stanje:</label>
                                <label className="infoLabel">{session.autoState === 3 ? "FADING" : "FINISHED"}</label>
                            </>
                        )}
                    </div> 
                    <div className="info">
                        <FontAwesomeIcon icon={faToggleOn} className="me-2" />
                        <h4>Opcije</h4>
                    </div>      
                    <div className="infoDugmici">
                        {session.status == "active" && (<button className="btnForm btnFading btnInfo"
                        onClick={async () => await onSetFading(session.id)}
                        >
                            Prebaci sesiju u FADING
                        </button>
                        )}
                        <button className="btnForm btnFinished btnInfo"
                        onClick={async () => await onSetFinished(session.id)}>
                            Prebaci sesiju u FINISHED
                        </button>
                        <button 
                            className="btnForm btnInfo"
                            onClick = {() => setMode("list")}>
                            <><FontAwesomeIcon icon={faCircleChevronLeft} className="me-2" /> Vrati se nazad</>
                        </button>
                    </div>           
                </div>
                <div className="managerRight">
                    <div className="seatsLabelsDiv" style={{height: "fit-content"}}>
                        <h4>Raspored prostorije {room.naziv}</h4>
                        <h4>Kapacitet: {room.kapacitet}</h4>
                    </div>
                    <RoomLayout
                        selectedRoom={room}
                        mode={mode}
                        selectedSeatIDs={selectedSeatIDs}
                        toggleSeat={toggleSeat}
                        vrlStatuses={VLRStatuses}
                    />
                    <div className="seatsLabelsDiv">
                        <p>Broj selektovanih stavki: {selectedSeatIDs.length}</p>
                        <button className="btnForm"
                                onClick={() => allSelected ? unselectAllSeats() : selectAllSeats()}>
                            {allSelected ? "Poništi sve" : "Izaberi sve"}
                        </button>
                    </div>
                    <div className="resourcesButtons">
                        <h4>Primeni operaciju:</h4>
                        <button onClick={PrepareResources} className="btnForm">Prepare</button>
                        <button onClick={ProvideResources} className="btnForm">Provide</button>
                        <button onClick={ReleaseResources} className="btnForm">Release</button>
                        <button onClick={KillResources} className="btnForm">Kill</button>
                    </div>
                </div>
            </div>
        </div>
    )
}