import React, { useState, useEffect } from "react";
import axiosInstance from "../../utils/axiosInstance";
import { Reservation, Review } from "../../types";

interface ReviewModalProps {
  reservation: Reservation;
  review: Review | null;
  isOpen: boolean;
  onClose: () => void;
  onUpdate: () => void;
}

const ReviewModal: React.FC<ReviewModalProps> = ({
  reservation,
  review,
  isOpen,
  onClose,
  onUpdate,
}) => {
  const [rating, setRating] = useState<number>(review ? review.rating : 0);
  const [comment, setComment] = useState<string>(review ? review.comment : "");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    if (review) {
      setRating(review.rating);
      setComment(review.comment);
    } else {
      setRating(0);
      setComment("");
    }
  }, [review]);

  const handleReviewSubmit = async () => {
    try {
      if (review) {
        await axiosInstance.put(
          `/Places/${reservation.placeId}/Reservations/${reservation.id}/Reviews/${review.id}`,
          { rating, comment }
        );
        setSuccess("Review updated successfully.");
      } else {
        await axiosInstance.post(
          `/Places/${reservation.placeId}/Reservations/${reservation.id}/Reviews`,
          { rating, comment }
        );
        setSuccess("Review submitted successfully.");
      }
      setError(null);
      onUpdate();
    } catch (error) {
      console.error("Submit review", error);
      setError("An error occurred while submitting the review.");
      setSuccess(null);
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
        <h2 className="text-2xl font-bold mb-4">
          {review ? "Edit Review" : "Leave a Review"}
        </h2>
        {error && <p className="text-red-600 mb-4">{error}</p>}
        {success && <p className="text-green-600 mb-4">{success}</p>}
        <div className="mb-4">
          <label className="block text-gray-700 mb-2">Rating</label>
          <select
            value={rating}
            onChange={(e) => setRating(Number(e.target.value))}
            className="w-full px-3 py-2 border rounded-md"
          >
            <option value={0}>Select Rating</option>
            {[1, 2, 3, 4, 5].map((value) => (
              <option key={value} value={value}>
                {value} Star{value > 1 && "s"}
              </option>
            ))}
          </select>
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 mb-2">Comment</label>
          <textarea
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            className="w-full px-3 py-2 border rounded-md"
          />
        </div>
        <div className="flex justify-end">
          <button
            className="px-4 py-2 bg-gray-500 text-white rounded-md mr-2"
            onClick={onClose}
          >
            Cancel
          </button>
          <button
            className="px-4 py-2 bg-green-500 text-white rounded-md"
            onClick={handleReviewSubmit}
          >
            Submit
          </button>
        </div>
      </div>
    </div>
  );
};

export default ReviewModal;
