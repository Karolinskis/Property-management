import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import "./App.css";
import Navbar from "./components/Navbar";
import PlaceDetails from "./components/Place/PlaceDetails";
import Home from "./pages/Home";
import MyPlaces from "./pages/MyPlaces";
import CreatePlaceForm from "./components/Forms/CreatePlaceForm";
import RegisterForm from "./components/Forms/RegisterForm";
import LoginForm from "./components/Forms/LoginForm";
import EditPlaceForm from "./components/Forms/EditPlaceForm";
import MyReservations from "./pages/MyReservations";
import Footer from "./components/Footer";

function App() {
  return (
    <Router>
      <div className="flex flex-col min-h-screen">
        <Navbar />
        <div className="flex-grow container mx-auto p-4 mt-20 max-w-7xl flex justify-center">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/places/mine" Component={MyPlaces} />
            <Route path="/places/:id" Component={PlaceDetails} />
            <Route path="places/:id/edit" element={<EditPlaceForm />} />
            <Route path="/places/new" element={<CreatePlaceForm />} />

            <Route path="/reservations/mine" Component={MyReservations} />

            <Route path="/register" element={<RegisterForm />} />
            <Route path="/login" element={<LoginForm />} />
          </Routes>
        </div>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
