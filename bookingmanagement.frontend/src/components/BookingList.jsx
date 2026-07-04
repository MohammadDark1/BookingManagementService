import { useEffect, useState } from "react";
import { getBookings, cancelBooking } from "../api/bookingApi";

function BookingList({ refresh }) {
    const [resourceId, setResourceId] = useState("");
    const [bookings, setBookings] = useState([]);

    const loadBookings = async () => {
        try {
            const response = await getBookings({
                resourceId
            });

            setBookings(response.data);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        loadBookings();
    }, [refresh]);

   const handleCancel = async (id, rowVersion) => {
    if (!window.confirm("Cancel this booking?"))
        return;

    try {
        await cancelBooking(id, rowVersion);
        loadBookings();
    } catch (error) {
        alert(
            error.response?.data?.message ||
            "Failed to cancel booking."
        );
    }
};

    return (
        <div className="card p-4">

            <h4>Bookings</h4>

            <div className="input-group mb-3">

                <input
                    className="form-control"
                    placeholder="Resource ID"
                    value={resourceId}
                    onChange={(e) => setResourceId(e.target.value)}
                />

                <button
                    className="btn btn-secondary"
                    onClick={loadBookings}
                >
                    Search
                </button>

            </div>

            <table className="table table-bordered">

                <thead>
                    <tr>
                        <th>Resource</th>
                        <th>User</th>
                        <th>Start</th>
                        <th>End</th>
                        <th>Status</th>
                        <th></th>
                    </tr>
                </thead>

                <tbody>

                    {bookings.map((booking) => (

                        <tr key={booking.id}>

                            <td>{booking.resourceId}</td>

                            <td>{booking.userId}</td>

                            <td>{booking.startDateTime}</td>

                            <td>{booking.endDateTime}</td>

                            <td>{booking.status}</td>

                            <td>
                                <button
                                    className="btn btn-danger btn-sm"
                                    onClick={() => handleCancel(booking.id, booking.rowVersion)}
                                >
                                    Cancel
                                </button>
                            </td>

                        </tr>

                    ))}

                </tbody>

            </table>

        </div>
    );
}

export default BookingList;