import React, { useState } from "react";
import axiosInstance from "../../utils/axiosInstance";
import { Reservation } from "../../types";

interface ReservationModalProps {
  reservation: Reservation;
  isOpen: boolean;
  onClose: () => void;
  onUpdate: () => void;
}

const ReservationModal: React.FC<ReservationModalProps> = ({
  reservation,
  isOpen,
  onClose,
  onUpdate,
}) => {
  const [status, setStatus] = useState(reservation.status);
  const [error, setError] = useState<string | null>(null);

  const handleStatusChange = async () => {
    try {
      await axiosInstance.put(
        `/Places/${reservation.placeId}/Reservations/${reservation.id}`,
        { ...reservation, status }
      );
      onUpdate();
      onClose();
    } catch (error) {
      console.error("Update reservation", error);
      setError("An error occurred while updating the reservation.");
    }
  };

  const handleDelete = async () => {
    try {
      await axiosInstance.delete(
        `/Places/${reservation.placeId}/Reservations/${reservation.id}`
      );
      onUpdate();
      onClose();
    } catch (error) {
      console.error("Delete reservation", error);
      setError("An error occurred while deleting the reservation.");
    }
  };

  return (
    <div
      className={`fixed inset-0 flex items-center justify-center z-50 ${
        isOpen ? "block" : "hidden"
      }`}
    >
      <div
        className="fixed inset-0 bg-black opacity-50"
        onClick={onClose}
      ></div>
      <div className="bg-white rounded-lg shadow-lg p-6 z-10 max-w-md w-full">
        <h2 className="text-2xl font-bold mb-4">Edit Reservation</h2>
        {error && <p className="text-red-600 mb-4">{error}</p>}
        <div className="mb-4">
          <label className="block text-gray-700 mb-2">Status</label>
          <select
            value={status}
            onChange={(e) => setStatus(e.target.value)}
            className="w-full px-3 py-2 border rounded-md"
          >
            <option value="Pending">Pending</option>
            <option value="Approved">Approved</option>
            <option value="Finished">Finished</option>
            <option value="Canceled">Canceled</option>
          </select>
        </div>
        <div className="flex justify-end">
          <button
            className="px-4 py-2 bg-gray-500 text-white rounded-md mr-2"
            onClick={onClose}
          >
            Cancel
          </button>
          <button
            className="px-4 py-2 bg-red-500 text-white rounded-md mr-2"
            onClick={handleDelete}
          >
            Delete
          </button>
          <button
            className="px-4 py-2 bg-green-500 text-white rounded-md"
            onClick={handleStatusChange}
          >
            Update
          </button>
        </div>
      </div>
    </div>
  );
};

export default ReservationModal;
