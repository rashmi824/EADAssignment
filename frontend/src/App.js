import './App.css';
import { BrowserRouter, Route, Routes } from "react-router-dom";
import React from "react";
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css"; // Importing Bootstrap
import OrderDetails from "./components/OrderDetails";
import OrderList from "./components/OrderList";
import OrderManagement from "./components/OrderManagement";
import OrderForm from "./components/OrderForm";
import AuthPage from "./Pages/AuthPage";
import Dashboard from "./Pages/Dashboard";
import CustomerManagement from "./Pages/CustomerManagement";
import VendorManagement from "./Pages/VendorManagement";
import VendorProfile from "./Pages/VendorProfile";
import UpdateOrder from "./components/UpdateOrder";
import Header from './shared/Header';
import Footer from './shared/Footer';
import AddProduct from './components/Products/AddProduct';
import AllProducts from './components/Products/AllProducts';
import AdjustInventory from './components/Inventory/AdjustInventory';
import AllInventory from './components/Inventory/AllInventory';
import UpdateProduct from './components/Products/UpdateProduct';
import AddInventory from './components/Inventory/AddInventory';
import "./../node_modules/bootstrap/dist/css/bootstrap.min.css";

function App() {
  const retrievedUser = JSON.parse(localStorage.getItem("user"));
  const role = retrievedUser?.role; // Use optional chaining to avoid errors if user is null

  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<AuthPage />} />

          {/* Redirect if no role is defined */}
          {role ? (
            <>
              <Route path="/orders/:id" element={<OrderDetails />} />
              <Route path="/orders" element={<OrderList />} />
              <Route path="/OrderHomePage" element={<OrderManagement />} />
              <Route path="/create-order" element={<OrderForm />} />
              <Route path="/update-order/:id" element={<UpdateOrder />} />
                
              {/* Product */}
              <Route path="/addproduct" element={ <> <Header /> <AddProduct /> <Footer /> </> } />
              <Route path="/allproducts" element={ <> <Header /> <AllProducts /> <Footer /> </> } />
              <Route path="/updateproduct/:id" element={ <> <Header /> <UpdateProduct /> <Footer /> </> } />

              {/* Inventory */}
              <Route path="/adjustInventory" element={ <> <Header /> <AdjustInventory /> <Footer /> </> } />
              <Route path="/allInventories" element={ <> <Header /> <AllInventory /> <Footer /> </> } />
              <Route path="/addInventory" element={ <> <Header /> <AddInventory /> <Footer /> </> } />


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
                    <Route
                      path="profile"
                      element={<VendorProfile userId={retrievedUser.id} />}
                    />

                    {/* Order-related routes */}
                    <Route path="orders/:id" element={<OrderDetails />} />
                    <Route path="orders" element={<OrderList />} />
                    <Route path="OrderHomePage" element={<OrderManagement />} />
                    <Route path="create-order" element={<OrderForm />} />
                    <Route path="update-order/:id" element={<UpdateOrder />} />
                  </>
                ) : null}

                {/* Admin-specific routes */}
                {role === "Administrator" && (
                  <>{/* Uncomment and add admin-specific routes here */}</>
                )}

                {/* Vendor-specific routes */}
                {role === "Vendor" && (
                  <>
                    <Route
                      path="profile"
                      element={<VendorProfile userId={retrievedUser.id} />}
                    />
                    <Route path="orders/:id" element={<OrderDetails />} />
                    <Route path="orders" element={<OrderList />} />
                    <Route path="OrderHomePage" element={<OrderManagement />} />
                    <Route path="create-order" element={<OrderForm />} />
                    <Route path="update-order/:id" element={<UpdateOrder />} />
                  </>
                )}

                {/* CSR-specific routes */}
                {role === "CSR" && (
                  <>{/* Uncomment and add CSR-specific routes here */}</>
                )}
              </Route>
            </>
          ) : (
            <Route path="*" element={<Navigate to="/" />} />
          )}
        </Routes>
      </div>
    </Router>
  );
}

export default App;


