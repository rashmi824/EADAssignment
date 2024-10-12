import React, { Component } from "react";
import axios from "axios";
import swal from "sweetalert";
import { Link } from "react-router-dom";
import product from "../../images/product.jpg";

export default class AddProduct extends Component {
  constructor(props) {
    super(props);
    this.state = {
      productID: "",
      name: "",
      description: "",
      price: 0,
      category: "",
      stock: 0, // Add stock state
      image: null,
      status: 0,
    };
  }

  handleInputChange = (e) => {
    const { name, value } = e.target;
    this.setState({
      [name]: value,
    });
  };

  handleFileChange = (e) => {
    this.setState({
      image: e.target.files[0],
    });
  };

  onSubmit = (e) => {
    e.preventDefault();
    const {
      productID,
      name,
      description,
      price,
      category,
      stock, // Include stock in form submission
      image,
    } = this.state;

    // Create FormData and log it to check values
    const formData = new FormData();
    formData.append("productID", productID);
    formData.append("name", name);
    formData.append("description", description);
    formData.append("price", price);
    formData.append("category", category);
    formData.append("stock", stock); // Add stock to form data
    formData.append("image", image);

    for (let [key, value] of formData.entries()) {
      console.log(key, value);
    }

    axios
      .post("/api/product/add", formData)
      .then((res) => {
        console.log("Response from POST request:", res);
        this.setState({
          productID: "",
          name: "",
          description: "",
          price: 0,
          category: "",
          stock: 0, // Reset stock field
          image: null,
          status: 0,
        });
        swal("Success", "Product Added Successfully", "success");
        window.location.href = "/allproducts";
      })
      .catch((error) => {
        console.error("Error occurred:", error.response);
        swal("Error", "An error occurred.", "error");
      });
  };

  validateForm = () => {
    const { productID, name, description, price, category, stock, image } = this.state;

    if (!productID || !name || !description || !price || !category || !image || stock <= 0) {
      swal("Validation Error", "All fields are required and stock must be positive.", "error");
      return false;
    }

    return true;
  };

  render() {
    const cate = ["Mens", "Womens", "Kids"];

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
              <img src={product} alt="Item Image" className="img-fluid" style={{ maxWidth: "650px", height: "750px" , marginLeft: "-200px"}} />
            </div>
          </div>

          <div className="col-md-6">
            <div className="card" style={{ marginTop: "20px", width: "100%", marginBottom: "30px" }}>
              <div className="card-body">
                <h1 className="text-center topic1 text1" style={{ color: "#9966CC" }}>Add Product</h1>
                <form className="needs-validation form" noValidate encType="multipart/form-data">
                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Product ID: </label>
                    <input
                      type="text"
                      id="productID"
                      className="form-control"
                      name="productID"
                      value={this.state.productID}
                      onChange={this.handleInputChange}
                      required
                    />
                  </div>
                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Product Name: </label>
                    <input
                      type="text"
                      id="name"
                      className="form-control"
                      name="name"
                      value={this.state.name}
                      onChange={this.handleInputChange}
                      required
                    />
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Description: </label>
                    <textarea
                      id="description"
                      className="form-control"
                      name="description"
                      value={this.state.description}
                      onChange={this.handleInputChange}
                      required
                      rows="3"
                    />
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Price: </label>
                    <input
                      type="number"
                      id="price"
                      className="form-control"
                      name="price"
                      value={this.state.price}
                      onChange={this.handleInputChange}
                      required
                    />
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Category: </label>
                    <select
                      className="form-control"
                      name="category"
                      value={this.state.category}
                      onChange={this.handleInputChange}
                      required
                    >
                      <option value="">Select Category</option>
                      {cate.map((cate, index) => (
                        <option key={index} value={cate}>
                          {cate}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Stock Level: </label>
                    <input
                      type="number"
                      id="stock"
                      className="form-control"
                      name="stock"
                      value={this.state.stock}
                      onChange={this.handleInputChange}
                      required
                    />
                  </div>

                  <div className="form-group" style={{ marginBottom: "15px" }}>
                    <label style={{ marginBottom: "5px" }}>Image: </label>
                    <input
                      type="file"
                      id="image"
                      className="form-control"
                      name="image"
                      onChange={this.handleFileChange}
                      required
                    />
                  </div>

                  <button
                    className="btn btn-primary btn-sm"
                    type="submit"
                    onClick={this.onSubmit}
                    style={{
                      marginTop: "15px",
                      backgroundColor: "#9966CC", // Set your desired background color here
                      color: "white", 
                      borderColor: "#9966CC", 
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
