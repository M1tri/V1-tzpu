import { useState } from "react";

export default function RoomLayout(/*{ layout }*/) {
  const [selectedSeats, setSelectedSeats] = useState([]);

    const layout = [
    [
        { id: 1, status: "free" },
        { id: 2, status: "taken" },
        null,
        { id: 3, status: "free" }
    ],
    [
        { id: 4, status: "free" },
        null,
        { id: 5, status: "taken" },
        { id: 6, status: "free" }
    ]
    ];


  const toggleSeat = (seat) => {
    if (seat.status === "taken") return;

    setSelectedSeats(prev =>
      prev.includes(seat.id)
        ? prev.filter(id => id !== seat.id)
        : [...prev, seat.id]
    );
  };

  return (
    <div style={{ display: "inline-block" }}>
      {layout.map((row, rowIndex) => (
        <div key={rowIndex} style={{ display: "flex" }}>
          {row.map((seat, colIndex) => {
            if (!seat)
              return <div key={colIndex} style={{ width: 40, height: 40 }} />;

            const isSelected = selectedSeats.includes(seat.id);

            return (
              <div
                key={seat.id}
                onClick={() => toggleSeat(seat)}
                style={{
                  width: 40,
                  height: 40,
                  margin: 4,
                  backgroundColor:
                    seat.status === "taken"
                      ? "gray"
                      : isSelected
                      ? "green"
                      : "lightblue",
                  cursor:
                    seat.status === "taken" ? "not-allowed" : "pointer",
                  borderRadius: 6,
                }}
              />
            );
          })}
        </div>
      ))}
    </div>
  );
}