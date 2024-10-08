import React, { Component } from "react";
import axios from "axios";
import { Link } from "react-router-dom";
import swal from "sweetalert";
import "../../css/Product.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faEdit,
  faTrashAlt,
  faFilter,
} from "@fortawesome/free-solid-svg-icons"; // Added faFilter icon
import loadingGif from "../../images/loading.gif";

class AllInventory extends Component {
  constructor(props) {
    super(props);
    this.state = {
      inventory: [],
      searchQuery: "",
      stockFilter: "", // Updated state without 'All' option
      loading: true,
    };
  }

  componentDidMount() {
    this.fetchInventory();
  }

  fetchInventory = () => {
    this.setState({ loading: true });
    axios
      .get("/api/inventory/getAll")
      .then((res) => {
        this.setState({ inventory: res.data, loading: false });
      })
      .catch((error) => {
        console.error("Error fetching inventory:", error);
        this.setState({ loading: false });
      });
  };

  handleSearch = (event) => {
    this.setState({ searchQuery: event.target.value });
  };

  handleStockFilter = (event) => {
    this.setState({ stockFilter: event.target.value });
  };

  onDelete = (id) => {
    swal({
      title: "Are you sure?",
      text: "Once deleted, this inventory item cannot be recovered!",
      icon: "warning",
      buttons: true,
      dangerMode: true,
    }).then((willDelete) => {
      if (willDelete) {
        axios
          .delete(`/api/inventory/delete/${id}`)
          .then(() => {
            swal("Inventory item deleted!", { icon: "success" });
            this.fetchInventory();
          })
          .catch((error) => {
            console.error("Error deleting item:", error);
            swal("Error", "Failed to delete item.", "error");
          });
      }
    });
  };

  onAdjustStock = (inventory) => {
    swal({
      title: "Adjust Stock",
      text: `Enter the amount to adjust for ${inventory.productName} product (positive to add Stock, negative to deduct stock) :`,
      content: "input",
      buttons: true,
    }).then((amount) => {
      const adjustment = parseInt(amount?.trim(), 10);
      if (isNaN(adjustment)) {
        swal("Invalid input", "Please enter a valid number", "error");
        return;
      }
      axios
        .put(
          `/api/inventory/adjust/${inventory.id}`,
          { adjustment },
          {
            headers: {
              "Content-Type": "application/json",
            },
          }
        )
        .then((res) => {
          swal(
            "Stock adjusted!",
            "The stock has been updated successfully.",
            "success"
          );
          this.fetchInventory();
        })
        .catch((error) => {
          console.error("Error adjusting stock:", error);
          const errorMessage =
            error.response?.data?.message ||
            "Failed to adjust stock due to an unknown error.";
          swal("Error", errorMessage, "error");
        });
    });
  };

  render() {
    const { inventory, searchQuery, stockFilter, loading } = this.state;

    const filteredInventory = inventory.filter((item) => {
      const matchesSearchQuery =
        item.productID.toLowerCase().includes(searchQuery.toLowerCase()) ||
        item.productName.toLowerCase().includes(searchQuery.toLowerCase());

      let matchesStockFilter = true;
      if (stockFilter === "In Stock") {
        matchesStockFilter = item.stockLevel > 100;
      } else if (stockFilter === "Out of Stock") {
        matchesStockFilter = item.stockLevel === 0;
      } else if (stockFilter === "Low Stock") {
        matchesStockFilter = item.stockLevel > 0 && item.stockLevel <= 100; // Example condition for low stock
      }

      return matchesSearchQuery && matchesStockFilter;
    });

    return (
      <div className="d-flex align-items-center justify-content-center h-100">
        <div className="container card p-5 m-5">
          <h1
            className="text-center"
            style={{ color: "#0448db", fontFamily: "Baufra" }}
          >
            <b>Inventory Management</b>
          </h1>
          <br />
          <div className="container">
            <div className="button-container d-flex justify-content-between align-items-center">
              <input
                type="text"
                placeholder="Search by Product ID or Name"
                className="form-control w-50"
                value={searchQuery}
                onChange={this.handleSearch}
              />
              <div className="d-flex align-items-center">
                <FontAwesomeIcon
                  icon={faFilter}
                  style={{ marginRight: "10px" }}
                />
                <select
                  className="form-control"
                  value={stockFilter}
                  onChange={this.handleStockFilter}
                >
                  <option value="">Filter by Stock</option>
                  <option value="In Stock">In Stock</option>
                  <option value="Out of Stock">Out of Stock</option>
                  <option value="Low Stock">Low Stock</option>
                </select>
              </div>
              
            </div>
            <br/>
              <Link to="/addInventory" className="btn btn-success mb-3">
                Add New Inventory
              </Link>
            <br />
            {loading ? (
              <div className="text-center">
                <img src={loadingGif} alt="Loading..." />
              </div>
            ) : (
              <table className="table table-bordered">
                <thead>
                  <tr>
                    <th scope="col">#</th>
                    <th scope="col">Product ID</th>
                    <th scope="col">Product Name</th>
                    <th scope="col">Stock Level</th>
                    <th scope="col">Stock Status</th>
                    <th scope="col">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredInventory.map((item, index) => (
                    <tr key={index}>
                      <td>{index + 1}</td>
                      <td>{item.productID}</td>
                      <td>{item.productName}</td>
                      <td>{item.stockLevel}</td>
                      <td>
                        {item.stockLevel > 100 ? (
                          <span className="text-success">In Stock</span>
                        ) : item.stockLevel > 0 && item.stockLevel <= 100 ? (
                          <span className="text-warning">Low Stock</span>
                        ) : (
                          <span className="text-danger">Out of Stock</span>
                        )}
                      </td>

                      <td>
                        <div className="d-flex align-items-center justify-content-center">
                          <button
                            className="btn btn-outline-info mx-2"
                            onClick={() => this.onAdjustStock(item)}
                          >
                            <FontAwesomeIcon icon={faEdit} />
                            &nbsp;Adjust
                          </button>
                          <button
                            className="btn btn-outline-danger mx-2"
                            onClick={() => this.onDelete(item.id)}
                          >
                            <FontAwesomeIcon icon={faTrashAlt} />
                            &nbsp;Delete
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      </div>
    );
  }
}

export default AllInventory;
