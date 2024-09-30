import React, { useState, useEffect } from "react";
import OrderList from "./OrderList";

const OrderManagement = () => {
  const [orders, setOrders] = useState([]);
  const [order, setOrder] = useState({ status: "Processing", notes: "" }); // Default status
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      const response = await fetch("http://localhost:5266/api/orders"); // Adjust to your backend API endpoint
      if (!response.ok) {
        throw new Error("Failed to fetch orders");
      }
      const data = await response.json();
      setOrders(data);
    } catch (err) {
      setError(err.message);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setOrder({ ...order, [name]: value });
  };

  const createOrder = async () => {
    try {
      const response = await fetch("http://localhost:5266/api/orders", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(order),
      });
      if (!response.ok) {
        throw new Error("Failed to create order");
      }
      setOrder({ status: "Processing", notes: "" }); // Reset form to default values
      fetchOrders(); // Refresh orders list
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div>
      <h2>Create Order</h2>
      {error && <div style={{ color: "red" }}>Error: {error}</div>}
      <select name="status" value={order.status} onChange={handleInputChange}>
        <option value="Processing">Processing</option>
        <option value="Shipped">Shipped</option>
        <option value="Delivered">Delivered</option>
        <option value="Cancelled">Cancelled</option>
      </select>
      <input
        type="text"
        name="notes"
        placeholder="Order Notes"
        value={order.notes}
        onChange={handleInputChange}
      />
      <button onClick={createOrder}>Submit Order</button>

      <h2>Order List</h2>
      <OrderList orders={orders} />
    </div>
  );
};

export default OrderManagement;
