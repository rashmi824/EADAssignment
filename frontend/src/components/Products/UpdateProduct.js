import React, { useState, useEffect } from "react";
import { useParams, Link } from "react-router-dom";
import axios from "axios";
import swal from "sweetalert";

function UpdateProduct() {
  const { id } = useParams();

  const cate = ["Mens", "Womens", "Kids"]; // Category options

  const [state, setState] = useState({
    id: id,
    productID: "",
    name: "",
    description: "",
    price: 0,
    category: "",
    image: null, // image will be stored as base64
    status: 0,
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setState((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  };

  const onSubmit = (e) => {
    e.preventDefault();

    if (validateForm()) {
      setState((prevState) => ({
        ...prevState,
      }));

      const { productID, name, description, price, category, image, status } =
        state;

      const data = {
        productID,
        name,
        description,
        price,
        category,
        image,
        status,
      };

      axios
        .put(`/api/product/update/${id}`, data)
        .then((res) => {
          console.log("Response from PUT request:", res);

          swal("Success", "Product Updated Successfully", "success");
          window.location.href = `/allproducts`;
        })
        .catch((error) => {
          swal("Error", "An error occurred.", "error");
        });
    }
  };

  const validateForm = () => {
    const { productID, name, description, price, category, image, status } =
      state;

    if (
      !productID ||
      !name ||
      !description ||
      !price ||
      !category ||
      !image ||
      !status
    ) {
      swal("Validation Error", "All fields are required.", "error");
      return false;
    }

    return true;
  };

  useEffect(() => {
    retrieveProductDetails();
  }, [id]);

  const retrieveProductDetails = () => {
    axios
      .get(`/api/product/${id}`)
      .then((res) => {
        if (res.status === 200) {
          const productData = res.data[0];
          setState((prevState) => ({
            ...prevState,
            productID: productData.productID,
            name: productData.name,
            description: productData.description,
            price: productData.price,
            category: productData.category,
            image: productData.image, // Assign base64 image
            status: productData.status,
          }));
        } else {
          console.error("API request did not return a successful status.");
        }
      })
      .catch((error) => {
        console.error("API request failed:", error);
      });
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];

    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setState((prevState) => ({
          ...prevState,
          image: reader.result.split(",")[1],
        }));
      };
      reader.readAsDataURL(file);
    }
  };

  return (
    <div className="container">
      <br />
      <div className="back-button-container">
        <Link
          to="#"
          onClick={() => window.history.back()}
          className="back-button"
        >
          &lt; Back
        </Link>
      </div>
      <div className="row">
        <div className="col-md-6">
          <div
            className="col-md-6"
            style={{ marginTop: "30px", marginLeft: "50px" }}
          >
            <img
              src={`data:image/jpeg;base64,${state.image}`}
              className="img-fluid"
              style={{
                maxWidth: "550px",
                height: "550px",
                marginBottom: "0px",
                marginLeft: "-200px"
              }}
            />
          </div>
        </div>
        <div className="col-md-6">
          <div
            className="card"
            style={{ marginTop: "20px", width: "105%", marginBottom: "30px" }}
          >
            <div className="card-body">
              <h1 className="text-center topic1 text1" style={{ color: "#9966CC" }}>Update Product</h1>
              <form
                className="needs-validation form"
                noValidate
                encType="multipart/form-data"
              >
                <div className="form-group" style={{ marginBottom: "15px" }}>
                  <label style={{ marginBottom: "5px" }}>Product ID: </label>
                  <input
                    type="text"
                    className="form-control"
                    name="productID"
                    value={state.productID}
                    style={{ width: "100%" }}
                    onChange={handleInputChange}
                    required
                  />
                </div>
                <div className="form-group" style={{ marginBottom: "15px" }}>
                  <label style={{ marginBottom: "5px" }}>Name: </label>
                  <input
                    type="text"
                    className="form-control"
                    name="name"
                    value={state.name}
                    onChange={handleInputChange}
                    required
                  />
                </div>
                <div className="form-group" style={{ marginBottom: "15px" }}>
                  <label style={{ marginBottom: "5px" }}>Vendor ID:</label>
                  <input
                    type="text"
                    className="form-control"
                    name="description"
                    value={state.description}
                    onChange={handleInputChange}
                    required
                  />
                </div>
                <div className="form-group" style={{ marginBottom: "15px" }}>
                  <label style={{ marginBottom: "5px" }}>Price:</label>
                  <input
                    type="Number"
                    className="form-control"
                    name="price"
                    value={state.price}
                    onChange={handleInputChange}
                    required
                  />
                </div>
                <div className="form-group" style={{ marginBottom: "15px" }}>
                  <label style={{ marginBottom: "5px" }}>Category:</label>
                  <select
                    className="form-control"
                    name="category"
                    value={state.category}
                    onChange={handleInputChange}
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
                  <label style={{ marginBottom: "5px" }}>Status: </label>
                  <select
                    className="form-control"
                    name="status"
                    value={state.status}
                    style={{ width: "100%" }}
                    onChange={handleInputChange}
                    required
                  >
                    <option value="">Select Status</option>
                    <option value="1">Active</option>
                    <option value="2">Inactive</option>
                  </select>
                </div>
                <div className="form-group" style={{ marginBottom: "15px" }}>
                  <label style={{ marginBottom: "5px" }}>Image: </label>
                  <input
                    type="file"
                    id="image"
                    className="form-control"
                    name="image"
                    accept="image/*"
                    onChange={handleFileChange}
                  />
                </div>

                <button
                  className="btn btn-primary btn-lg"
                  type="button"
                  onClick={onSubmit}
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
                  Update
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default UpdateProduct;
