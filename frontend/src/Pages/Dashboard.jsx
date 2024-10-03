// src/components/Dashboard.js
import { useLocation, Outlet } from "react-router-dom";
import Sidebar from "../components/Sidebar";
import "../css/dashboard.css";
import React, { useState, useEffect } from "react";

const Dashboard = () => {
  const [userRole, setUserRole] = useState("");
  const location = useLocation(); // Get the current location object

  useEffect(() => {
    // Get user object from localStorage
    const storedUser = localStorage.getItem("user");

    if (storedUser) {
      const user = JSON.parse(storedUser); // Parse the user object
      setUserRole(user.role); // Set the role in state
    }
  }, []);

  const onLogout = () => {
    // Logout logic here
  };

  return (
    <div className="dashboard">
      <Sidebar role={userRole} onLogout={onLogout} />
      <div className="dashboard-content">
        {location.pathname === "/dashboard" && ( // Check if the current path is '/dashboard'
          <h1>Welcome to the {userRole} dashboard</h1>
        )}
        <Outlet /> {/* Render child routes here */}
      </div>
    </div>
  );
};

export default Dashboard;
