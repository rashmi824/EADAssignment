import React, { Component } from "react";
import axios from "axios";
import swal from "sweetalert";

export default class AdjustInventory extends Component {
  constructor(props) {
    super(props);
    this.state = {
      productID: "",
      quantity: 0,
    };
  }

  handleInputChange = (e) => {
    const { name, value } = e.target;
    this.setState({
      [name]: value,
    });
  };

  onSubmit = (e) => {
    e.preventDefault();
    const { productID, quantity } = this.state;

    if (quantity === 0) {
      swal("Validation Error", "Quantity cannot be zero.", "error");
      return;
    }

    const adjustmentData = {
      inventoryId: productID,
      quantity: parseInt(quantity),
    };

    axios
      .post("/api/inventory/adjust", adjustmentData)
      .then((res) => {
        swal("Success", "Inventory adjusted successfully.", "success");
        this.setState({
          productID: "",
          quantity: 0,
        });
      })
      .catch((error) => {
        swal(
          "Error",
          "An error occurred during inventory adjustment.",
          "error"
        );
      });
  };

  render() {
    return (
      <div className="container">
        <br />
        <h2>Adjust Inventory</h2>
        <form onSubmit={this.onSubmit}>
          <div className="form-group" style={{ marginBottom: "15px" }}>
            <label>Product ID:</label>
            <input
              type="text"
              name="productID"
              className="form-control"
              value={this.state.productID}
              onChange={this.handleInputChange}
              required
            />
          </div>

          <div className="form-group" style={{ marginBottom: "15px" }}>
            <label>Quantity (positive to add, negative to deduct):</label>
            <input
              type="number"
              name="quantity"
              className="form-control"
              value={this.state.quantity}
              onChange={this.handleInputChange}
              required
            />
          </div>

          <button
            className="btn btn-primary"
            type="submit"
            style={{
              marginTop: "15px",
              fontWeight: "bold",
              padding: "10px 20px",
            }}
          >
            Adjust Inventory
          </button>
        </form>
      </div>
    );
  }
}
