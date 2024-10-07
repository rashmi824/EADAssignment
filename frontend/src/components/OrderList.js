import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import "../css/Order/OrderList.css";

function OrderList() {
  const [orders, setOrders] = useState([]);
  const navigate = useNavigate(); // Use navigate for redirection

  useEffect(() => {
    axios
      .get("http://localhost:5266/api/orders")
      .then((response) => {
        setOrders(response.data);
      })
      .catch((error) => {
        console.error("There was an error fetching the orders!", error);
      });
  }, []);

  // const updateOrderStatus = async (id, newStatus) => {
  //   try {
  //     await axios.put(`http://localhost:5266/api/orders/${id}`, {
  //       status: newStatus,
  //     });
  //     alert("Order status updated!");
  //     // Refresh orders to get the updated status
  //     const response = await axios.get("http://localhost:5266/api/orders");
  //     setOrders(response.data);
  //   } catch (error) {
  //     console.error("There was an error updating the order status!", error);
  //     alert("Failed to update order status.");
  //   }
  // };

  const cancelOrder = async (id) => {
    try {
      await axios.delete(`http://localhost:5266/api/orders/${id}`, {
        data: { note: "Customer requested cancellation" },
      });
      alert("Order Cancelled!");
      // Refresh orders to get the updated list
      const response = await axios.get("http://localhost:5266/api/orders");
      setOrders(response.data);
    } catch (error) {
      console.error("There was an error cancelling the order!", error);
    }
  };

  const markAsDelivered = async (id) => {
    try {
      await axios.put(`http://localhost:5266/api/orders/mark-delivered/${id}`);
      alert("Order Marked as Delivered!");
      // Refresh orders to get the updated status
      const response = await axios.get("http://localhost:5266/api/orders");
      setOrders(response.data);
    } catch (error) {
      console.error(
        "There was an error marking the order as delivered!",
        error
      );
    }
  };

  const goToUpdateOrder = (id) => {
    navigate(`/dashboard/update-order/${id}`); // Navigate to UpdateOrder page
  };

  return (
    <div className="container">
      <h2 className="mt-4">Orders</h2>
      <table className="table table-striped">
        <thead className="thead-dark">
          <tr>
            <th>Order ID</th>
            <th>Customer</th>
            <th>Status</th>
            {/* <th>Update Status</th> */}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order, index) => (
            <tr key={order.id}>
              <td>{index + 1}</td>
              <td>{order.customerId}</td>
              <td>{order.status}</td>
              {/* <td>
                <select
                  value={order.status}
                  onChange={(e) => updateOrderStatus(order.id, e.target.value)}
                >
                  <option value="Processing">Processing</option>
                  <option value="Delivered">Delivered</option>
                  <option value="Cancelled">Cancelled</option>
                </select>
              </td> */}
              <td>
                <button
                  className="btn btn-warning btn-sm mr-2"
                  onClick={() => goToUpdateOrder(order.id)} // Navigate to update order page
                >
                  Update
                </button>
                <button
                  className="btn btn-danger btn-sm mr-2"
                  onClick={() => cancelOrder(order.id)}
                >
                  Cancel
                </button>
                <button
                  className="btn btn-success btn-sm"
                  onClick={() => markAsDelivered(order.id)}
                >
                  Mark as Delivered
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default OrderList;
