import React, { useState, useEffect } from "react";
import axios from "axios";
import "../css/Order/OrderDetails.css";

function OrderDetails({ match }) {
  const [order, setOrder] = useState(null);

  useEffect(() => {
    const orderId = match.params.id;
    axios
      .get(`/api/orders/${orderId}`)
      .then((response) => {
        setOrder(response.data);
      })
      .catch((error) => {
        console.error("There was an error fetching the order!", error);
      });
  }, [match.params.id]);

  if (!order) {
    return <div className="text-center">Loading...</div>;
  }

  return (
    <div className="container">
      <h2 className="mt-4">Order Details for {order.customer}</h2>
      <p>
        Status: <span className="badge badge-info">{order.status}</span>
      </p>
      <ul className="list-group">
        {order.items.map((item) => (
          <li key={item.productId} className="list-group-item">
            {item.productName} - Quantity: {item.quantity}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default OrderDetails;
