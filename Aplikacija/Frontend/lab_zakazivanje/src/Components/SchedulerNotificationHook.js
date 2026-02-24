import { useEffect } from "react";
import * as signalR from "@microsoft/signalr";

export default function useSchedulerNotifications(callbackSucces, callbackFail, selectedRoom) {

    useEffect(() => {

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7213/schedulerHub", { withCredentials: true })
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => console.log("SignalR connected"))
            .catch(err => console.error(err));

        connection.on("ReceiveSchedulerNotification", (notification) => {

            console.log("Notification:", notification);

            if (notification.success) {
                callbackSucces(notification.roomId);
            } else {
                callbackFail(notification.message);
            }
        });

        return () => {
            connection.stop();
        };

    }, [selectedRoom]);
}