import React, { useEffect, useState } from "react";
import axiosInstance from "../utils/axiosInstance";
import { Place, Review, Reservation } from "../types";
import PlaceItem from "../components/Place/PlaceItem";
import ReservationItem from "../components/Reservation/ReservationItem";
import ReservationModal from "../components/Reservation/ReservationModal";
import { useAuth } from "../utils/AuthContext";
import { jwtDecode } from "jwt-decode";

const MyPlaces: React.FC = () => {
  const [places, setPlaces] = useState<Place[]>([]);
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [selectedReservation, setSelectedReservation] =
    useState<Reservation | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { isAuthenticated } = useAuth();

  const fetchMyPlacesAndReservations = async () => {
    try {
      const response = await axiosInstance.get("/Places");
      const allPlaces = response.data;

      const token = localStorage.getItem("accessToken");
      if (token) {
        const decodedToken = jwtDecode<{ sub: string }>(token);
        const userId = decodedToken.sub;

        const myPlaces = allPlaces.filter(
          (place: Place) => place.userId === userId
        );

        // Fetch reviews for each place and calculate the average rating
        const placesWithRatings = await Promise.all(
          myPlaces.map(async (place: Place) => {
            const reviewsResponse = await axiosInstance.get(
              `/Places/${place.id}/Reviews`
            );
            const reviews: Review[] = reviewsResponse.data;
            const averageRating =
              reviews.reduce((sum, review) => sum + review.rating, 0) /
              reviews.length;

            return {
              ...place,
              averageRating: isNaN(averageRating) ? 0 : averageRating,
            };
          })
        );

        setPlaces(placesWithRatings);

        // Fetch active reservations for each place
        const reservationsWithDetails = await Promise.all(
          myPlaces.map(async (place: Place) => {
            const reservationsResponse = await axiosInstance.get(
              `/Places/${place.id}/Reservations`
            );
            const reservations: Reservation[] = reservationsResponse.data;

            return reservations.map((reservation) => ({
              ...reservation,
              place,
            }));
          })
        );

        // Flatten the array of reservations
        const allReservations = reservationsWithDetails.flat();

        // Sort reservations by recency (most recent first)
        allReservations.sort(
          (a, b) =>
            new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
        );

        setReservations(allReservations);
      }
    } catch (error) {
      console.error("Fetch my places and reservations", error);
      setError(
        "An error occurred while fetching your places and reservations."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isAuthenticated) {
      fetchMyPlacesAndReservations();
    }
  }, [isAuthenticated]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Oh no! {error}</div>;
  }

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4 text-center">
        My Places and Reservations
      </h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <h2 className="text-xl font-semibold mb-4">My Places</h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            {places.map((place) => (
              <PlaceItem
                key={place.id}
                title={place.address}
                price={place.price}
                averageRating={place.averageRating}
                link={`/places/${place.id}`}
              />
            ))}
          </div>
        </div>
        <div>
          <h2 className="text-xl font-semibold mb-4">Active Reservations</h2>
          {reservations.length > 0 ? (
            <div className="grid grid-cols-1 gap-4 cursor-pointer">
              {reservations.map((reservation) => (
                <div
                  key={reservation.id}
                  onClick={() => setSelectedReservation(reservation)}
                >
                  <ReservationItem
                    key={reservation.id}
                    reservation={reservation}
                    place={reservation.place}
                    user={reservation.user}
                  />
                </div>
              ))}
            </div>
          ) : (
            <p>No active reservations.</p>
          )}
        </div>
      </div>
      {selectedReservation && (
        <ReservationModal
          reservation={selectedReservation}
          isOpen={!!selectedReservation}
          onClose={() => setSelectedReservation(null)}
          onUpdate={fetchMyPlacesAndReservations}
        />
      )}
    </div>
  );
};

export default MyPlaces;
