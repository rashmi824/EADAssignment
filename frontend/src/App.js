import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import "./App.css";
import OrderManagement from "./components/OrderManagement";

function App() {
  return (
    <Router>
      <div className="App">
        <header className="App-header">
          <h1>Order Management System</h1>
        </header>
        <Routes>
          <Route path="/" element={<OrderManagement />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
