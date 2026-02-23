export class SessionView
{
    static Statuses = Object.freeze({
        0:"planned",
        1:"next",
        2:"active",
        3:"fading",
        4:"finished"
    })

    constructor(id, naziv, tip, datum, vremePoc, vremeKraja, status, autoStart, autoEnd, autoState)
    {
        this.id = id;
        this.naziv = naziv;
        this.tip = tip;
        this.datum = datum;
        this.vremePoc = vremePoc;
        this.vremeKraja = vremeKraja;
        this.status = SessionView.Statuses[status];
        this.autoStart = autoStart;
        this.autoEnd = autoEnd;
        this.autoState = autoState;
    }
}