import React from "react";

const OrderList = ({ orders }) => {
  return (
    <div>
      {orders.length > 0 ? (
        <ul>
          {orders.map((order) => (
            <li key={order._id}>
              Order ID: {order._id} | Status: {order.status}
            </li>
          ))}
        </ul>
      ) : (
        <p>No orders found.</p>
      )}
    </div>
  );
};

export default OrderList;
