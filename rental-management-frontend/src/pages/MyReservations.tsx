import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import axiosInstance from "../utils/axiosInstance";
import axios from "axios";
import { Place, Reservation, Review } from "../types";
import ReservationItem from "../components/Reservation/ReservationItem";
import ReviewModal from "../components/Review/ReviewModal";
import ConfirmationModal from "../components/ui/ConfirmationModal";
import { useAuth } from "../utils/AuthContext";
import { jwtDecode } from "jwt-decode";

const MyReservations: React.FC = () => {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [selectedReservation, setSelectedReservation] =
    useState<Reservation | null>(null);
  const [selectedReview, setSelectedReview] = useState<Review | null>(null);
  const [isReviewModalOpen, setIsReviewModalOpen] = useState(false);
  const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { isAuthenticated } = useAuth();

  const fetchMyReservations = async () => {
    try {
      const token = localStorage.getItem("accessToken");
      if (token) {
        const decodedToken = jwtDecode<{ sub: string }>(token);
        const userId = decodedToken.sub;

        // Fetch all places
        const placesResponse = await axiosInstance.get("/Places");
        const allPlaces: Place[] = placesResponse.data;

        // Fetch reservations for each place
        const reservationsWithDetails = await Promise.all(
          allPlaces.map(async (place: Place) => {
            const reservationsResponse = await axiosInstance.get(
              `/Places/${place.id}/Reservations`
            );
            const reservations: Reservation[] = reservationsResponse.data;

            const reservationsWithReviews = await Promise.all(
              reservations.map(async (reservation) => {
                try {
                  const reviewResponse = await axiosInstance.get(
                    `/Places/${reservation.placeId}/Reservations/${reservation.id}/Reviews`
                  );
                  const review: Review = reviewResponse.data;
                  return { ...reservation, review };
                } catch (error) {
                  if (
                    axios.isAxiosError(error) &&
                    error.response?.status === 404
                  ) {
                    return { ...reservation, review: null };
                  } else {
                    throw error;
                  }
                }
              })
            );

            return reservationsWithReviews.map((reservation) => ({
              ...reservation,
              place,
            }));
          })
        );

        // Flatten the array of reservations
        const allReservations = reservationsWithDetails.flat();

        // Filter reservations by user ID
        const userReservations = allReservations.filter(
          (reservation) => reservation.user.id === userId
        );

        setReservations(userReservations);
      }
    } catch (error) {
      console.error("Fetch my reservations", error);
      setError("An error occurred while fetching your reservations.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isAuthenticated) {
      fetchMyReservations();
    }
  }, [isAuthenticated]);

  const handleCancelClick = (reservation: Reservation) => {
    setSelectedReservation(reservation);
    setIsConfirmationModalOpen(true);
  };

  const handleReviewClick = async (reservation: Reservation) => {
    setSelectedReservation(reservation);
    try {
      const response = await axiosInstance.get(
        `/Places/${reservation.placeId}/Reservations/${reservation.id}/Reviews`
      );
      const review: Review = response.data;
      setSelectedReview(review);
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 404) {
        setSelectedReview(null);
      } else {
        console.error("Fetch review", error);
        setError("An error occurred while fetching the review.");
      }
    }
    setIsReviewModalOpen(true);
  };

  const handleConfirmCancel = async () => {
    if (selectedReservation) {
      try {
        await axiosInstance.put(
          `/Places/${selectedReservation.placeId}/Reservations/${selectedReservation.id}`,
          { ...selectedReservation, status: "Canceled" }
        );
        fetchMyReservations();
        setIsConfirmationModalOpen(false);
        setSelectedReservation(null);
      } catch (error) {
        console.error("Cancel reservation", error);
        setError("An error occurred while canceling the reservation.");
      }
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Oh no! {error}</div>;
  }

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">My Reservations</h1>
      {reservations.length > 0 ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {reservations.map((reservation) => (
            <div
              key={reservation.id}
              className="bg-white p-4 rounded-lg shadow-md mb-4"
            >
              <Link to={`/places/${reservation.place.id}`}>
                <ReservationItem
                  reservation={reservation}
                  place={reservation.place}
                  user={reservation.user}
                />
              </Link>
              <div className="flex justify-end space-x-2 mt-4">
                <button
                  className="px-4 py-2 bg-red-500 text-white rounded-md"
                  onClick={() => handleCancelClick(reservation)}
                >
                  Cancel
                </button>
                <button
                  className="px-4 py-2 bg-blue-500 text-white rounded-md"
                  onClick={() => handleReviewClick(reservation)}
                >
                  {reservation.review ? "Edit Review" : "Review"}
                </button>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p>No reservations found.</p>
      )}
      {selectedReservation && (
        <>
          <ReviewModal
            reservation={selectedReservation}
            review={selectedReview}
            isOpen={isReviewModalOpen}
            onClose={() => setIsReviewModalOpen(false)}
            onUpdate={fetchMyReservations}
          />
          <ConfirmationModal
            isOpen={isConfirmationModalOpen}
            onClose={() => setIsConfirmationModalOpen(false)}
            onConfirm={handleConfirmCancel}
            message="Are you sure you want to cancel this reservation?"
          />
        </>
      )}
    </div>
  );
};

export default MyReservations;
