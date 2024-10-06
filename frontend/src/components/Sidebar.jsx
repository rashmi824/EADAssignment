// src/components/Sidebar.js
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "../css/Sidebar.css"; // Import the CSS file

const Sidebar = ({ role }) => {
  const [dropdownVisible, setDropdownVisible] = useState(false);
  const navigate = useNavigate();

  const toggleDropdown = () => {
    setDropdownVisible((prev) => !prev);
  };

  const handleOutsideClick = (e) => {
    if (!e.target.closest(".user-profile")) {
      setDropdownVisible(false);
    }
  };

  useEffect(() => {
    if (dropdownVisible) {
      document.addEventListener("click", handleOutsideClick);
    } else {
      document.removeEventListener("click", handleOutsideClick);
    }
    return () => document.removeEventListener("click", handleOutsideClick);
  }, [dropdownVisible]);

  const handleLogout = () => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("user");

    navigate("/");
  };

  return (
    <aside className="dashboard-sidebar" id="sidebar">
      <div className="sidebar-header">
        <img
          src="data:image/png;base64,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"
          alt="Logo Icon"
          className="logo-image"
        />
        <h2 className="app-logo">Style Hevan</h2>
      </div>
      <nav className="sidebar-navigation">
        <ul className="nav-list">
          <li>
            <a href="#" className="nav-link active">
              Dashboard
            </a>
          </li>
          {role === "Administrator" && (
            <>
              <li>
                <a href="/dashboard/product-management" className="nav-link">
                  Product Activation/Deactivation
                </a>
              </li>
              <li>
                <a href="/dashboard/inventory-management" className="nav-link">
                  Inventory Management
                </a>
              </li>
              <li>
                <a href="/dashboard/vendor-management" className="nav-link">
                  Vendor Management
                </a>
              </li>
              <li>
                <a href="/dashboard/order" className="nav-link">
                  Order Status
                </a>
              </li>
              <li>
                <a href="/dashboard/customer-management" className="nav-link">
                  Customer Management
                </a>
              </li>
            </>
          )}
          {role === "Vendor" && (
            <>
              <li>
                <a href="/dashboard/product-management" className="nav-link">
                  Product CRUD
                </a>
              </li>
              <li>
                <a href="/dashboard/order-status" className="nav-link">
                  Order Status
                </a>
              </li>
            </>
          )}
          {role === "CSR" && (
            <>
              <li>
                <a href="/dashboard/order-status" className="nav-link">
                  Order Status
                </a>
              </li>
              <li>
                <a href="/dashboard/customer-management" className="nav-link">
                  Customer Management
                </a>
              </li>
            </>
          )}
        </ul>
      </nav>
      <div className="user-profile" onClick={toggleDropdown}>
        <img
          src="https://th.bing.com/th/id/OIP.k7dE2dftQijg3KbpTyIObAHaHa?w=195&h=195&c=7&r=0&o=5&dpr=1.3&pid=1.7"
          alt="Profile Icon"
          className="prof-image"
        />
        <span className="profile-name">My Profile</span>
        {dropdownVisible && (
          <div className="dropdown-menu">
            <ul>
              <li>
                <a href="/dashboard/profile">Profile</a>
              </li>
              <li>
                <a href="#" onClick={handleLogout}>
                  Log Out
                </a>
              </li>
            </ul>
          </div>
        )}
      </div>
    </aside>
  );
};

export default Sidebar;
