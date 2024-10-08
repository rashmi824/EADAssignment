import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import "../css/Order/OrderForm.css"; // Import any specific styles you have

const OrderForm = () => {
  const [customerId, setCustomerId] = useState("");
  const [customerEmail, setCustomerEmail] = useState(""); // New state for CustomerEmail
  const [vendorId, setVendorId] = useState("");
  const [productIds, setProductIds] = useState("");
  const [status, setStatus] = useState("Processing"); // Default status
  const [error, setError] = useState("");
  const navigate = useNavigate(); // Use navigate for redirection

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(""); // Reset error

    // Prepare the order data
    const orderData = {
      customerId,
      customerEmail, // Include CustomerEmail in the payload
      vendorId,
      productIds: productIds.split(",").map((id) => id.trim()), // Convert to array
      status,
      orderDate: new Date(), // Assuming order date is the current date
      notes: [], // Initialize with no notes
    };

    try {
      await axios.post("http://localhost:5266/api/orders", orderData); // Make the API call
      alert("Order created successfully!");
      navigate("/orders"); // Redirect to order list after creation
    } catch (err) {
      setError("There was an error creating the order. Please try again.");
      console.error(err);
    }
  };

  return (
    <div className="container mt-4">
      <h2 className="text-center mb-4">Create Order</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="customerId">Customer ID</label>
          <input
            type="text"
            className="form-control"
            id="customerId"
            value={customerId}
            onChange={(e) => setCustomerId(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="customerEmail">Customer Email</label>
          <input
            type="email"
            className="form-control"
            id="customerEmail"
            value={customerEmail}
            onChange={(e) => setCustomerEmail(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="vendorId">Vendor ID</label>
          <input
            type="text"
            className="form-control"
            id="vendorId"
            value={vendorId}
            onChange={(e) => setVendorId(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="productIds">Product IDs (comma-separated)</label>
          <input
            type="text"
            className="form-control"
            id="productIds"
            value={productIds}
            onChange={(e) => setProductIds(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="status">Status</label>
          <select
            className="form-control"
            id="status"
            value={status}
            onChange={(e) => setStatus(e.target.value)}
          >
            <option value="Processing">Processing</option>
            <option value="Delivered">Delivered</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
        <button type="submit" className="btn btn-primary">
          Create Order
        </button>
      </form>
    </div>
  );
};

export default OrderForm;
