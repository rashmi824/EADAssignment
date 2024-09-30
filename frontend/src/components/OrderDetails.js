import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

const OrderDetails = () => {
  const { orderId } = useParams();
  const navigate = useNavigate();
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [status, setStatus] = useState("");

  useEffect(() => {
    fetchOrderDetails();
  }, [orderId]);

  const fetchOrderDetails = async () => {
    try {
      const response = await fetch(
        `http://localhost:5266/api/orders/${orderId}` // Adjusted to your backend API endpoint
      );
      if (response.ok) {
        const data = await response.json();
        setOrder(data);
        setStatus(data.status);
      } else {
        throw new Error("Failed to fetch order details");
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleCancelOrder = async () => {
    try {
      const response = await fetch(
        `http://localhost:5266/api/orders/${orderId}`,
        {
          method: "DELETE",
        }
      );
      if (response.ok) {
        alert("Order cancelled successfully");
        navigate("/"); // Redirect to the order list
      } else {
        throw new Error("Failed to cancel the order");
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleUpdateStatus = async () => {
    try {
      const response = await fetch(
        `http://localhost:5266/api/orders/${orderId}/status`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ status }),
        }
      );
      if (response.ok) {
        alert("Order status updated successfully");
        fetchOrderDetails();
      } else {
        throw new Error("Failed to update order status");
      }
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (!order) {
    return <div>Order not found.</div>;
  }

  return (
    <div>
      <h2>Order Details</h2>
      <p>
        <strong>Order ID:</strong> {order._id}
      </p>
      <p>
        <strong>Status:</strong>
        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="Processing">Processing</option>
          <option value="Shipped">Shipped</option>
          <option value="Delivered">Delivered</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </p>
      <button onClick={handleUpdateStatus}>Update Status</button>
      <p>
        <strong>Notes:</strong> {order.notes || "No notes available"}
      </p>
      <p>
        <strong>Created At:</strong>{" "}
        {new Date(order.createdAt).toLocaleString()}
      </p>
      <p>
        <strong>Items:</strong>
      </p>
      <ul>
        {order.items && order.items.length > 0 ? (
          order.items.map((item, index) => (
            <li key={index}>
              {item.productName} - Quantity: {item.quantity}
            </li>
          ))
        ) : (
          <li>No items in this order.</li>
        )}
      </ul>
      <button onClick={handleCancelOrder}>Cancel Order</button>
    </div>
  );
};

export default OrderDetails;
