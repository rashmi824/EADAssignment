import React, { useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import { FaBell } from "react-icons/fa";
import axios from "axios";
import logo from "../images/Logo.jpg";
import "./header.css";

function Header() {
  const location = useLocation();
  const [showDropdown, setShowDropdown] = useState(false); // Dropdown state
  const [lowStockAlerts, setLowStockAlerts] = useState([]); // State for low stock alerts
  const [loading, setLoading] = useState(true); // Loading state

  const handleLogout = () => {
    localStorage.removeItem("userRole");
    window.location.href = "/login";
  };

  const toggleDropdown = () => {
    setShowDropdown(!showDropdown);
  };

  const isLoginPath = location.pathname === "/login";

  useEffect(() => {
    const retrievePosts = async () => {
      try {
        const res = await axios.get("/api/inventory/getLowInv");
        if (res.status === 200) {
          const inventory = res.data;

          const alerts = inventory
            .map((item) => {
              if (item.stockLevel < 1) {
                return `${item.productName} is Out of stock`;
              } else if (item.stockLevel < 100) {
                return `${item.productName} is low on stock`;
              }
              return null;
            })
            .filter(Boolean);

          setLowStockAlerts(alerts);
          setLoading(false);
        } else {
          console.error("API request did not return a successful status.");
          setLoading(false);
        }
      } catch (error) {
        console.error("API request failed:", error);
        setLoading(false);
      }
    };

    retrievePosts();
  }, []);

  return (
    <nav className="navbar navbar-expand-lg navbar-light custom-navbar">
      <div className="container-fluid">
        {/* Navigation items aligned to the left */}
        <div className="collapse navbar-collapse" id="navbarSupportedContent">
          <ul className="navbar-nav me-auto">
            <li
              className={`nav-item ${
                location.pathname === "/" ? "active" : ""
              }`}
            >
              <Link className="nav-link" to="/">
                Home
              </Link>
            </li>
            <li
              className={`nav-item ${
                location.pathname === "/allproducts" ? "active" : ""
              }`}
            >
              <Link className="nav-link" to="/allproducts">
                Product Management
              </Link>
            </li>
            <li
              className={`nav-item ${
                location.pathname === "/allInventories" ? "active" : ""
              }`}
            >
              <Link className="nav-link" to="/allInventories">
                Inventory Management
              </Link>
            </li>
            {!isLoginPath && (
              <li className="nav-item">
                <button
                  className="btn btn-outline-dark btn-sm"
                  onClick={handleLogout}
                >
                  Logout
                </button>
              </li>
            )}
          </ul>
        </div>

        {/* Right-aligned items */}
        <div className="navbar-brand ms-auto d-flex align-items-center">
          <a href="#">
            <img
              src={logo}
              width="80"
              height="80"
              className="d-inline-block align-top"
              alt="Store Logo"
            />
          </a>
          <span className="site-name ms-2 align-middle">Style Heaven</span>

          {/* Notification Bell Icon */}
          <div className="notification-bell ms-3 position-relative">
            <FaBell
              size={24}
              onClick={toggleDropdown}
              style={{ cursor: "pointer" }}
            />
            {/* Show number of alerts if available */}
            {lowStockAlerts.length > 0 && (
              <span className="badge badge-danger">
                {lowStockAlerts.length}
              </span>
            )}

            {/* Dropdown for low stock notifications */}
            {showDropdown && (
              <div className="notification-dropdown">
                <h6>Notifications</h6>
                <ul className="list-unstyled">
                  {lowStockAlerts.length > 0 ? (
                    lowStockAlerts.map((alert, index) => (
                      <li key={index} className="dropdown-item">
                        {alert}
                      </li>
                    ))
                  ) : (
                    <li className="dropdown-item">No low stock alerts</li>
                  )}
                </ul>
              </div>
            )}
          </div>
        </div>

        <button
          className="navbar-toggler"
          type="button"
          data-toggle="collapse"
          data-target="#navbarSupportedContent"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
      </div>
    </nav>
  );
}

export default Header;
