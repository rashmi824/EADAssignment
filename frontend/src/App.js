import './App.css';
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Header from './shared/Header';
import Footer from './shared/Footer';
import AddProduct from './components/Products/AddProduct';
import AllProducts from './components/Products/AllProducts';
import AdjustInventory from './components/Inventory/AdjustInventory';
import AllInventory from './components/Inventory/AllInventory';
import UpdateProduct from './components/Products/UpdateProduct';
import AddInventory from './components/Inventory/AddInventory';
import "./../node_modules/bootstrap/dist/css/bootstrap.min.css";

const App = () => {
  return (
    <BrowserRouter>
  <Header />
      <div className="">
        <div>
          <Routes>
            {/* Product */}
            <Route path="/addproduct" element={<AddProduct />} />
            <Route path="/allproducts" element={<AllProducts />} />
            <Route path="/updateproduct/:id" element={<UpdateProduct />} />

            {/* Inventory */}
            <Route path="/adjustInventory" element={<AdjustInventory />} />
            <Route path="/allInventories" element={<AllInventory />} />
            <Route path="/addInventory" element={<AddInventory />} />

          </Routes>
        </div>
      </div>
      <Footer />
    </BrowserRouter>

  );
}

export default App;


