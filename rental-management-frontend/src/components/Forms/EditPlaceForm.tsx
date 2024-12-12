import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axiosInstance from "../../utils/axiosInstance";
import { useAuth } from "../../utils/AuthContext";

const EditPlaceForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [roomsCount, setRoomsCount] = useState<number>(0);
  const [size, setSize] = useState<number>(0);
  const [address, setAddress] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [price, setPrice] = useState<number>(0);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const { isAuthenticated, roles } = useAuth();

  useEffect(() => {
    const fetchPlace = async () => {
      try {
        const response = await axiosInstance.get(
          `${process.env.REACT_APP_API_URL}/Places/${id}`
        );
        const place = response.data;
        setRoomsCount(place.roomsCount);
        setSize(place.size);
        setAddress(place.address);
        setDescription(place.description);
        setPrice(place.price);
      } catch (error) {
        console.error("Fetch place", error);
        setError("An error occurred while fetching the place details.");
      }
    };

    fetchPlace();
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await axiosInstance.put(
        `${process.env.REACT_APP_API_URL}/Places/${id}`,
        {
          roomsCount,
          size,
          address,
          description,
          price,
        },
        { withCredentials: true }
      );
      setSuccess("Place updated successfully.");
      setError(null);
      navigate(`/places/${id}`);
    } catch (error) {
      console.error("Update place", error);
      setError("An error occurred while updating the place.");
      setSuccess(null);
    }
  };

  if (
    !isAuthenticated ||
    !roles.includes("Owner") ||
    !roles.includes("Admin")
  ) {
    return <div>You are not authorized to edit this place.</div>;
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="max-w-lg mx-auto p-4 bg-white shadow-md rounded-md"
    >
      <div className="mb-4">
        <label className="block text-gray-700 font-bold mb-2">Address:</label>
        <input
          type="text"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          required
          className="w-full px-3 py-2 border rounded-md"
        />
      </div>
      <div className="mb-4">
        <label className="block text-gray-700 font-bold mb-2">
          Description:
        </label>
        <textarea
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full px-3 py-2 border rounded-md resize"
        />
      </div>
      <div className="mb-4">
        <label className="block text-gray-700 font-bold mb-2">
          Rooms Count:
        </label>
        <input
          type="number"
          value={roomsCount}
          onChange={(e) => setRoomsCount(Number(e.target.value))}
          required
          className="w-full px-3 py-2 border rounded-md"
        />
      </div>
      <div className="mb-4">
        <label className="block text-gray-700 font-bold mb-2">Size:</label>
        <input
          type="number"
          value={size}
          onChange={(e) => setSize(Number(e.target.value))}
          required
          className="w-full px-3 py-2 border rounded-md"
        />
      </div>
      <div className="mb-4">
        <label className="block text-gray-700 font-bold mb-2">Price:</label>
        <input
          type="number"
          value={price}
          onChange={(e) => setPrice(Number(e.target.value))}
          required
          className="w-full px-3 py-2 border rounded-md"
        />
      </div>
      <button
        type="submit"
        className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600"
      >
        Update Place
      </button>
      {error && <p className="mt-4 text-red-600">{error}</p>}
      {success && <p className="mt-4 text-green-600">{success}</p>}
    </form>
  );
};

export default EditPlaceForm;
