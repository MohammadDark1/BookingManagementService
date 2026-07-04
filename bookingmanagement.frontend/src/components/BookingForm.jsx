import { useState } from "react";
import { createBooking } from "../api/bookingApi";

function BookingForm({ onBookingCreated }) {
    const [formData, setFormData] = useState({
        resourceId: "",
        userId: "",
        startDateTime: "",
        endDateTime: ""
    });

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            await createBooking(formData);

            alert("Booking created successfully!");

            setFormData({
                resourceId: "",
                userId: "",
                startDateTime: "",
                endDateTime: ""
            });

            if (onBookingCreated) {
                onBookingCreated();
            }

        } catch (error) {
            alert(error.response?.data?.message || "Failed to create booking.");
        }
    };

    return (
        <div className="card p-4 mb-4">
            <h4>Create Booking</h4>

            <form onSubmit={handleSubmit}>

                <div className="mb-3">
                    <label className="form-label">Resource ID</label>
                    <input
                        type="text"
                        className="form-control"
                        name="resourceId"
                        value={formData.resourceId}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label">User ID</label>
                    <input
                        type="text"
                        className="form-control"
                        name="userId"
                        value={formData.userId}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label">Start Date</label>
                    <input
                        type="datetime-local"
                        className="form-control"
                        name="startDateTime"
                        value={formData.startDateTime}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label">End Date</label>
                    <input
                        type="datetime-local"
                        className="form-control"
                        name="endDateTime"
                        value={formData.endDateTime}
                        onChange={handleChange}
                        required
                    />
                </div>

                <button className="btn btn-primary">
                    Create Booking
                </button>

            </form>
        </div>
    );
}

export default BookingForm;