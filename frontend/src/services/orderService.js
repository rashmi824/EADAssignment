const apiUrl = "http://localhost:5266/api/orders/"; // Point this to your actual backend API URL

export const fetchOrders = async () => {
  const response = await fetch(apiUrl);
  return await response.json();
};

export const fetchOrderById = async (id) => {
  const response = await fetch(`${apiUrl}/${id}`);
  return await response.json();
};

export const createOrder = async (order) => {
  const response = await fetch(apiUrl, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(order),
  });
  return await response.json();
};

export const updateOrder = async (id, updatedOrder) => {
  const response = await fetch(`${apiUrl}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(updatedOrder),
  });
  return await response.ok;
};

export const deleteOrder = async (id) => {
  const response = await fetch(`${apiUrl}/${id}`, {
    method: "DELETE",
  });
  return await response.ok;
};
