using RentalManagement.Entities;
using RentalManagement.Contexts;

namespace RentalManagement
{
    public static class Utils
    {
        public static bool HasConflictingReservations(AppDbContext context, int placeId, DateTime startDate, DateTime endDate)
        {
            bool isConflicting = context.Reservations
                .Any(r => r.Place.Id == placeId &&
                            r.Status == Status.Approved &&
                            startDate < r.EndDate &&
                            endDate > r.StartDate);

            return isConflicting;
        }

        public static bool PlaceExists(AppDbContext context, int placeId)
        {
            return context.Places.Any(p => p.Id == placeId);
        }

        public static bool ReservationExists(AppDbContext context, int reservationId)
        {
            return context.Reservations.Any(r => r.Id == reservationId);
        }

        public static bool ReviewExists(AppDbContext context, int reviewId)
        {
            return context.Reviews.Any(r => r.Id == reviewId);
        }
    }
}
