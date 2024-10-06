import React from "react";
import { Link } from "react-router-dom"; // Link for navigation
import "../css/Order/OrderManagement.css"; // Ensure you have appropriate styles

function OrderManagement() {
  return (
    <div className="container mt-4">
      <h2 className="text-center mb-4">Order Management System</h2>
      <div className="text-center mb-4">
        <Link to="/orders" className="btn btn-secondary mx-2">
          View Orders
        </Link>
        <Link to="/create-order" className="btn btn-primary mx-2">
          Create New Order
        </Link>
        {/* Add more links or buttons for other functionalities as needed */}
      </div>
      <div className="card text-center">
        <div className="card-body">
          <h5 className="card-title">
            Welcome to the Order Management Dashboard
          </h5>
          <p className="card-text">
            Here you can manage your orders efficiently. Use the buttons above
            to navigate to the relevant sections.
          </p>
        </div>
      </div>
    </div>
  );
}

export default OrderManagement;
