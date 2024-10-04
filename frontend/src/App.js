import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css"; // Importing Bootstrap
import OrderDetails from "./components/OrderDetails";
import OrderList from "./components/OrderList";
import OrderManagement from "./components/OrderManagement";
import OrderForm from "./components/OrderForm";

function App() {
  return (
    <Router>
      <div className="container">
        {" "}
        {/* Bootstrap container */}
        <Routes>
          <Route path="/orders/:id" element={<OrderDetails />} />
          <Route path="/orders" element={<OrderList />} />
          <Route path="/OrderHomePage" element={<OrderManagement />} />
          <Route path="/create-order" element={<OrderForm />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
