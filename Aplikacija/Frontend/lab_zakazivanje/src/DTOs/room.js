export class RoomView
{
    constructor(id, naziv, kapacitet, raspored)
    {
        this.id = id;
        this.naziv = naziv;
        this.kapacitet = kapacitet;
        this.raspored = [];
        for (const row of raspored)
        {
            const red = [];
            for (const seat of row)
            {
                red.push(new Seat(seat.id, seat.ip));
            }
            this.raspored.push(red);
        }
    }
}

export class Seat
{
    constructor(seatID, seatIP)
    {
        this.seatID = seatID;
        this.seatIP = seatIP;
    }
}