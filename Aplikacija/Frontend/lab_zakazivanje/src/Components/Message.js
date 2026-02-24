export default function Message({message, photo, klasa})
{
    return (
        <div className={klasa}>
            <p>{message}</p>
            <img
                src={photo}>
            </img>
        </div>
    )
}