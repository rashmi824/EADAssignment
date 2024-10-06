import React from "react";
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import "./App.css";
import OrderManagement from "./components/OrderManagement";
import AuthPage from "./Pages/AuthPage";
import Dashboard from "./Pages/Dashboard";
import CustomerManagement from "./Pages/CustomerManagement";
import VendorManagement from "./Pages/VendorManagement";
import VendorProfile from "./Pages/VendorProfile";

function App() {
  const retrievedUser = JSON.parse(localStorage.getItem("user"));
  const role = retrievedUser?.role; // Use optional chaining to avoid errors if user is null

  // Redirect if no role is defined
  if (!role) {
    return <Navigate to="/" />;
  }

  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<AuthPage />} />
         

          {/* Dashboard route with nested routes */}
          <Route path="/dashboard" element={<Dashboard />}>
            {/* Conditionally render the nested routes based on the user's role */}
            {role === "Administrator" || role === "CSR" ? (
              <>
                <Route
                  path="customer-management"
                  element={<CustomerManagement />}
                />
                <Route
                  path="vendor-management"
                  element={<VendorManagement />}
                />
                <Route path="order" element={<OrderManagement />} />
                <Route
                  path="profile"
                  element={<VendorProfile userId={retrievedUser.id} />}
                />
              </>
            ) : null}

            {/* Admin-specific routes */}
            {role === "Administrator" && (
              <>{/* Uncomment and add your routes here */}</>
            )}

            {/* Vendor-specific routes */}
            {role === "Vendor" && (
              <>
                 <Route
                  path="/profile"
                  element={<VendorProfile userId={retrievedUser.id} />}
                />
              </>
            )}

            {/* CSR-specific routes */}
            {role === "CSR" && <>{/* Uncomment and add your routes here */}</>}
          </Route>
        </Routes>
      </div>
    </Router>
  );
}

export default App;
