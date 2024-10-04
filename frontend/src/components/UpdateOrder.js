import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import "../css/Order/OrderForm.css"; // Import any specific styles you have

const UpdateOrder = () => {
  const { id } = useParams(); // Get the order ID from the URL
  const [customerId, setCustomerId] = useState("");
  const [customerEmail, setCustomerEmail] = useState(""); // Added field for Customer Email
  const [vendorId, setVendorId] = useState("");
  const [productIds, setProductIds] = useState("");
  const [status, setStatus] = useState("Processing"); // Default status
  const [error, setError] = useState("");
  const navigate = useNavigate(); // Use navigate for redirection

  // Fetch the order details to update
  useEffect(() => {
    const fetchOrder = async () => {
      try {
        const response = await axios.get(
          `http://localhost:5266/api/orders/${id}`
        );
        const order = response.data;
        setCustomerId(order.customerId);
        setCustomerEmail(order.customerEmail); // Set customer email
        setVendorId(order.vendorId);
        setProductIds(order.productIds.join(", ")); // Convert array to string
        setStatus(order.status);
      } catch (err) {
        setError("Error fetching order details.");
        console.error(err);
      }
    };

    fetchOrder();
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(""); // Reset error

    // Prepare the order data
    const orderData = {
      customerId,
      customerEmail, // Include customer email
      vendorId,
      productIds: productIds.split(",").map((id) => id.trim()), // Convert to array
      status,
    };

    try {
      await axios.put(`http://localhost:5266/api/orders/${id}`, orderData); // Update the order
      alert("Order updated successfully!");
      navigate("/orders"); // Redirect to order list after update
    } catch (err) {
      setError("There was an error updating the order. Please try again.");
      console.error(err);
    }
  };

  return (
    <div className="container mt-4">
      <h2 className="text-center mb-4">Update Order</h2>
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
        <button type="submit" className="btn btn-warning">
          Update Order
        </button>
      </form>
    </div>
  );
};

export default UpdateOrder;
