import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faComputer, faCheck } from "@fortawesome/free-solid-svg-icons";
import { useState, useEffect } from "react";

export default function RoomLayout({ selectedRoom, mode, selectedSeatIDs = [], toggleSeat, vrlStatuses }) {

    const [statusInfo, setStatusInfo] = useState({});
    const [InfoLoading, setInfoLoading] = useState(true);

    useEffect(() => {
        async function fetchAllInfo() {
            try {
                const res = await fetch(`https://localhost:7213/api/vlr/GetAllInfo`);
                if (!res.ok) 
                    throw new Error("Greska pri ucitavanju statusa");

                const json = await res.json();

                for (const info of json)
                {
                    statusInfo[info.naziv] = {simbol: info.symbol, boja: info.color};
                }
                console.log(statusInfo);

                setStatusInfo(statusInfo);

            } catch (err) 
            {
                console.error(err);
            } finally 
            {
                setInfoLoading(false);
            }
        }

        fetchAllInfo();
    }, []);

    if (InfoLoading) return <div>Učitavanje info...</div>;

    const vrste = selectedRoom.raspored.length;
    const kolone = Math.max(...selectedRoom.raspored.map(podniz => podniz.length));

    return (
        <div 
            className="roomLayout"
            style={{
                gridTemplateRows: `repeat(${vrste}, 1fr)`,
                gridTemplateColumns: `repeat(${kolone}, 1fr)`
            }}
        >
        {selectedRoom.raspored.map((red, rowIndex) =>
            red.map((celija, colIndex) => {
                const postoji = celija.seatID && celija.seatIP;
                const isSelected = selectedSeatIDs.includes(celija.seatID);

                return (
                    <div
                        key={`${rowIndex}-${colIndex}`}
                        className={`
                            grid-item seat
                            ${!postoji ? "empty-seat" : ""}
                            ${mode == "sessionManager" && isSelected ? "seat-selected" : ""}
                        `}
                    >
                        {postoji && (
                            <>
                                {mode === "sessionManager" && (
                                    <div 
                                        className={`seat-checkbox ${isSelected ? "checked" : ""}`}
                                        onClick={() => toggleSeat(celija.seatID)}
                                    >
                                        {isSelected && <FontAwesomeIcon icon={faCheck} className="seat-check-icon" />}
                                    </div>
                                )}

                                <FontAwesomeIcon icon={faComputer} className="me-2 seat-icon" />
                                <div className="seat-overlay">
                                    <p>ID: {celija.seatID}</p>
                                    <p>IP:</p>
                                    <p>{celija.seatIP}</p>
                                    {mode === "sessionManager" && celija.seatID != null && (
                                        <p>Status: {statusInfo[vrlStatuses[celija.seatID]].simbol}</p>
                                    )}
                                </div>
                            </>
                        )}
                    </div>
                );
            })
        )}
        </div>
    );
}