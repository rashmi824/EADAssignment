import React, { Component } from "react";
import axios from "axios";
import swal from "sweetalert";
import { Link } from "react-router-dom";
import inventory from "../../images/Inventory.jpg";

export default class AddInventory extends Component {
  constructor(props) {
    super(props);
    this.state = {
      productID: "",
      productName: "",
      stockLevel: 0,
      stockStatus: "", // Automatically set based on stockLevel
      products: [], // Array to hold fetched products
    };
  }

  // Fetch products when component mounts
  componentDidMount() {
    axios
      .get("/api/product/getProducts")
      .then((res) => {
        this.setState({
          products: res.data,
        });
      })
      .catch((error) => {
        console.error("Error fetching products:", error.response);
        swal("Error", "Unable to fetch products.", "error");
      });
  }

  handleInputChange = (e) => {
    const { name, value } = e.target;
    const { products } = this.state;
  
    // Set the state for the input fields
    this.setState({
      [name]: value,
    });
  
    // Sync productName with productID and vice versa
    if (name === "productID") {
      const selectedProduct = products.find((p) => p.productID === value);
      if (selectedProduct) {
        this.setState({
          productName: selectedProduct.name,
        });
      }
    }
  
    if (name === "productName") {
      const selectedProduct = products.find((p) => p.name === value);
      if (selectedProduct) {
        this.setState({
          productID: selectedProduct.productID,
        });
      }
    }
  
    // Auto-generate stockStatus based on stockLevel
    if (name === "stockLevel") {
      const stockLevel = parseInt(value, 10); // Parse value as integer
  
      let stockStatus = "";
      if (stockLevel > 100) {
        stockStatus = "In Stock";
      } else if (stockLevel >= 1 && stockLevel <= 100) {
        stockStatus = "Low Stock";
      } else if (stockLevel < 1) {
        stockStatus = "Out of Stock";
      }
  
      this.setState({
        stockStatus, // Automatically set the stock status based on the level
      });
    }
  };
  

  onSubmit = (e) => {
    e.preventDefault();
    const { productID, productName, stockLevel, stockStatus } = this.state;
  
    // Validation
    if (!this.validateForm()) {
      return;
    }
  
    // Create a JSON object instead of FormData
    const inventoryData = {
      productID: productID,
      productName: productName,
      stockLevel: stockLevel,
      stockStatus: stockStatus,
    };
  
    // Send the request as JSON
    axios
      .post("/api/inventory/add", inventoryData, {
        headers: {
          "Content-Type": "application/json", // Ensure the content type is set to JSON
        },
      })
      .then((res) => {
        console.log("Response from POST request:", res);
        this.setState({
          productID: "",
          productName: "",
          stockLevel: 0,
          stockStatus: "",
        });
        swal("Success", "Inventory Added Successfully", "success");
        window.location.href = "/allInventories";
      })
      .catch((error) => {
        console.error("Error occurred:", error.response);
        swal("Error", "An error occurred.", "error");
      });
  };
  

  validateForm = () => {
    const { productID, productName, stockLevel, stockStatus } = this.state;

    if (!productID || !productName || !stockLevel || !stockStatus) {
      swal(
        "Validation Error",
        "All fields are required and stock must be positive.",
        "error"
      );
      return false;
    }

    return true;
  };

  render() {
    const { products, productID, productName, stockLevel, stockStatus } = this.state;

    return (
      <div className="container">
        <br />
        <div className="back-button-container">
          <Link to="#" onClick={() => window.history.back()} className="back-button">
            &lt; Back
          </Link>
        </div>
        <div className="row">
          <div className="col-md-6">
            <div className="col-md-6">
              <img
                src={inventory}
                alt="Item Image"
                className="img-fluid"
                style={{ maxWidth: "550px", height: "550px" ,marginLeft: "-150px"}}
              />
            </div>
          </div>

          <div className="col-md-6">
            <div className="card" style={{ marginTop: "20px", width: "100%", marginBottom: "30px" }}>
              <div className="card-body">
                <h1 className="text-center topic1 text1 " style={{ color: "#9966CC" }}>Add Inventory</h1>
                <form className="needs-validation form" noValidate>
                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Product ID: </label>
                    <select
                      id="productID"
                      name="productID"
                      className="form-control"
                      value={productID}
                      onChange={this.handleInputChange}
                    >
                      <option value="">Select Product ID</option>
                      {products.map((product) => (
                        <option key={product.productID} value={product.productID}>
                          {product.productID}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Product Name: </label>
                    <select
                      id="productName"
                      name="productName"
                      className="form-control"
                      value={productName}
                      onChange={this.handleInputChange}
                    >
                      <option value="">Select Product Name</option>
                      {products.map((product) => (
                        <option key={product.productID} value={product.name}>
                          {product.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Stock Level: </label>
                    <input
                      type="number"
                      id="stockLevel"
                      className="form-control"
                      name="stockLevel"
                      value={stockLevel}
                      onChange={this.handleInputChange}
                      required
                    />
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Stock Status: </label>
                    <input
                      type="text"
                      id="stockStatus"
                      className="form-control"
                      name="stockStatus"
                      value={stockStatus}
                      readOnly
                    />
                  </div>

                  <button
  className="btn btn-primary btn-sm"
  type="submit"
  onClick={this.onSubmit}
  style={{
    backgroundColor: "#9966CC", // Set your desired background color here
    color: "white", 
    borderColor: "#9966CC",             // Set your desired text color here
    marginTop: "15px",
    boxShadow: "0 2px 4px rgba(0, 0, 0, 0.2)",
    borderRadius: "5px",
    fontWeight: "bold",
    padding: "10px 20px",
  }}
>
  Submit
</button>

                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
