import { useState } from "react";
import BookingForm from "../components/BookingForm";
import BookingList from "../components/BookingList";

function Home() {

    const [refresh, setRefresh] = useState(false);

    return (
        <div className="container mt-5">

            <h2 className="mb-4">
                Booking Management System
            </h2>

            <BookingForm
                onBookingCreated={() => setRefresh(!refresh)}
            />

            <BookingList
                refresh={refresh}
            />

        </div>
    );
}

export default Home;