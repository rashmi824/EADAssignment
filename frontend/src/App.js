import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import "./App.css";
import OrderManagement from "./components/OrderManagement";
import AuthPage from "./Pages/AuthPage";
import Dashboard from "./Pages/Dashboard";
import CustomerManagement from "./Pages/CustomerManagement";
import VendorManagement from "./Pages/VendorManagement";
import VendorProfile from "./Pages/VendorProfile";
//import InventoryManagement from "./Pages/InventoryManagement"; // Assuming this component exists
//import ProductCRUD from "./Pages/ProductCRUD"; // Assuming this component exists
//import OrderStatus from "./Pages/OrderStatus"; // Assuming this component exists

function App() {
  const retrievedUser = JSON.parse(localStorage.getItem("user"));
  const role = retrievedUser?.role; // Use optional chaining to avoid errors if user is null

  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<AuthPage />} />
          <Route path="/profile" element={<VendorProfile userId={retrievedUser.id} />} />

          {/* Dashboard route with nested routes */}
          <Route path="/dashboard" element={<Dashboard />}>
            {/* Conditionally render the nested routes based on the user's role */}
            {role === "Administrator" || role === "CSR" ? (
              <>
                 {/*path ="/dashboard/customer-management"}*/}
                <Route path="customer-management" element={<CustomerManagement />} />
                <Route path="vendor-management" element={<VendorManagement />} />
                <Route path="order" element={<OrderManagement />} />
              </>
            ) : null}

            {/* Admin-specific routes */}
            {role === "Administrator" && (
              <>
               {/*<Route path="inventory-management" element={<InventoryManagement />} />*/}
              </>
            )}

            {/* Vendor-specific routes */}
            {role === "Vendor" && (
              <>
                {/*<Route path="product" element={<ProductCRUD />} />*/}
                 {/*<Route path="order-status" element={<OrderStatus />} />*/}
                <Route path="profile" element={<VendorProfile userId={retrievedUser.id} />} />
              </>
            )}

            {/* CSR-specific routes */}
            {role === "CSR" && (
              <>
                {/*<Route path="order-status" element={<OrderStatus />} />*/}
                
              </>
            )}

            
          </Route>
        </Routes>
      </div>
    </Router>
  );
}

export default App;
