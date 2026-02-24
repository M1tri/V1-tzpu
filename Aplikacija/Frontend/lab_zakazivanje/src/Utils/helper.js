export function parseDate(datum)
{
    let date = new Date(datum);

    let dan = String(date.getDate()).padStart(2, '0');
    let mesec = String(date.getMonth()+1).padStart(2,'0');
    let godina = date.getFullYear();

    let formatirano = `${dan}. ${mesec}. ${godina}.`;
    return formatirano;
}

export function parseTime(datum, vreme)
{
    let time = new Date(`${datum}T${vreme}`); 

    let hh = String(time.getHours()).padStart(2, '0');
    let mm = String(time.getMinutes()).padStart(2, '0');

    let formatirano = `${hh}:${mm}`;

    return formatirano;
}