import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import axiosInstance from "../../utils/axiosInstance";
import axios from "axios";

const CreatePlaceForm: React.FC = () => {
  const [roomsCount, setRoomsCount] = useState<number>(0);
  const [size, setSize] = useState<number>(0);
  const [address, setAddress] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [price, setPrice] = useState<number>(0);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    console.log("Creating place", {
      roomsCount,
      size,
      address,
      description,
      price,
    });

    try {
      const response = await axiosInstance.post(
        `${process.env.REACT_APP_API_URL}/Places`,
        {
          roomsCount,
          size,
          address,
          description,
          price,
        }
      );
      setSuccess("Place created successfully.");
      setError(null);
      navigate(`/places/${response.data.id}`);
    } catch (error) {
      console.error("Create place", error);
      if (axios.isAxiosError(error)) {
        setError(
          "An error occurred while creating the place. " + error.message
        );
      } else {
        setError("An error occurred while creating the place.");
      }
      setSuccess(null);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-semibold text-center pb-10">
        Create a new Place
      </h1>
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
          Create Place
        </button>
        {error && <p className="mt-4 text-red-600">{error}</p>}
        {success && <p className="mt-4 text-green-600">{success}</p>}
      </form>
    </div>
  );
};

export default CreatePlaceForm;
