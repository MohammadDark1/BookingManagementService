import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:44302/api/Booking",
    headers: {
        "Content-Type": "application/json",
    },
});

export const createBooking = (booking) =>
    api.post("/CreateBooking", booking);

export const getBookings = (params) =>
    api.get("/GetBookings", { params });

export const cancelBooking = (id, rowVersion) =>
    api.delete(`/${id}`, { data: { rowVersion } });

export default api;