import React from "react";
import { Reservation, Place, User } from "../../types";

interface ReservationItemProps {
  reservation: Reservation;
  place: Place;
  user: User;
}

const ReservationItem: React.FC<ReservationItemProps> = ({
  reservation,
  place,
  user,
}) => {
  return (
    <div className="bg-white p-4 rounded-lg shadow-md mb-4 cursor-pointer hover:bg-gray-100 transition duration-200">
      <p>
        <b>Place:</b> {place.address}
      </p>
      <p>
        <b>User:</b> {user.userName}
      </p>
      <p>
        <b>Start Date:</b>{" "}
        {new Date(reservation.startDate).toLocaleDateString()}
      </p>
      <p>
        <b>End Date:</b> {new Date(reservation.endDate).toLocaleDateString()}
      </p>
      <p>
        <b>Status:</b> {reservation.status}
      </p>
      <p>
        <b>Price:</b> {reservation.price} €
      </p>
    </div>
  );
};

export default ReservationItem;
