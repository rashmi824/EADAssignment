import React, { Component } from "react";
import axios from "axios";
import swal from "sweetalert";
import "../../css/Product.css";
import { Link } from "react-router-dom";
import loadingGif from "../../images/loading.gif";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPen, faTrashAlt } from "@fortawesome/free-solid-svg-icons";

class AllProducts extends Component {
  constructor(props) {
    super(props);

    this.state = {
      posts: [],
      loading: true,
      statusUpdating: null,
      searchQuery: "",
    };
  }

  componentDidMount() {
    this.retrievePosts();
  }

  retrievePosts() {
    axios
      .get("/api/product/getAll")
      .then((res) => {
        if (res.status === 200) {
          const products = res.data;
          this.setState({
            posts: products,
            loading: false,
          });
        } else {
          console.error("API request did not return a successful status.");
          this.setState({
            loading: false,
          });
        }
      })
      .catch((error) => {
        console.error("API request failed:", error);
        this.setState({
          loading: false,
        });
      });
  }

  onDelete = (id) => {
    axios
      .delete(`/api/product/delete/${id}`)
      .then((res) => {
        swal("Deleted Successful", "Product Removed Successfully", "success");
        this.retrievePosts();
      })
      .catch((error) => {
        console.error("Axios Error:", error);
        swal("Network Error", "Failed to connect to the server", "error");
      });
  };

  handleToggle = (post, event) => {
    const updatedStatus = event.target.checked ? 1 : 0;

    this.setState({ statusUpdating: post.id });

    axios
      .put(`/api/product/updateStatus/${post.id}`, { status: updatedStatus })
      .then((res) => {
        swal(
          "Status Updated",
          "Product status updated successfully",
          "success"
        );

        const updatedPosts = this.state.posts.map((p) =>
          p.id === post.id ? { ...p, status: updatedStatus } : p
        );
        this.setState({ posts: updatedPosts, statusUpdating: null });
      })
      .catch((error) => {
        console.error("Error updating status:", error);
        swal("Error", "Failed to update product status", "error");
        this.setState({ statusUpdating: null });
      });
  };

  handleSearch = (event) => {
    this.setState({ searchQuery: event.target.value });
  };

  render() {
    const filteredPosts = this.state.posts.filter(
      (post) =>
        post.name
          .toLowerCase()
          .includes(this.state.searchQuery.toLowerCase()) ||
        post.productID
          .toLowerCase()
          .includes(this.state.searchQuery.toLowerCase())
    );

    return (
      <div className="d-flex align-items-center justify-content-center h-100">
        <div className="container card  p-5 m-5"style={{ width: "90vw", maxWidth: "1000px" }}>
          <h1
            className="text-center"
            style={{ color: "#9966CC", fontFamily: "Baufra" }}
          >
            <b>Product Management</b>
          </h1>
          <br />
          <div className="container">
            <div className="button-container d-flex justify-content-between align-items-center">
              {/* Search bar */}
              <input
                type="text"
                placeholder="Search by name or product ID"
                className="form-control w-50"
                value={this.state.searchQuery}
                onChange={this.handleSearch}
              />

              {/* Add product button */}
              <Link to="/addProduct" className="btn mb-3" style={{ backgroundColor: '#9966CC', color: 'white' }}>
                Add New Product
              </Link>
            </div>
            <br />
            {this.state.loading ? (
              <div className="text-center">
                <img src={loadingGif} alt="Loading..." />
              </div>
            ) : (
              <table className="table bordered">
               <thead>
  <tr>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>#</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Image</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Product ID</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Name</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Category</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Vendor ID</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Price</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Active</th>
    <th scope="col" style={{ backgroundColor: '#A6959A', color: 'white' }}>Actions</th>
  </tr>
</thead>

                <tbody>
                  {filteredPosts.map((post, index) => (
                    <tr key={index}>
                      <td>{index + 1}</td>
                      <td>
                        <img
                          src={`data:image/jpeg;base64,${post.image}`}
                          style={{ height: "80px", width: "70px"}}
                          alt="Product"
                        />
                      </td>
                      <td style={{ display: "none" }}>{post.id}</td>
                      <td>{post.productID}</td>
                      <td>{post.name}</td>
                      <td>{post.category}</td>
                      <td>{post.description}</td>
                      <td>{post.price}</td>
                      <td>
                        {this.state.statusUpdating === post.id ? (
                          <img
                            src={loadingGif}
                            alt="Updating..."
                            style={{ height: "30px", width: "30px" }}
                          />
                        ) : (
                          <label className="switch">
                            <input
                              type="checkbox"
                              checked={post.status === 1}
                              onChange={(event) =>
                                this.handleToggle(post, event)
                              }
                            />
                            <span className="slider round"></span>
                          </label>
                        )}
                      </td>
                      <td>
                        <div className="d-flex align-items-center justify-content-center">
                          <Link
                            to={`/updateproduct/${post.id}`}
                            className="btn btn-outline-secondary mx-2"
                          >
                            <FontAwesomeIcon icon={faPen} />
                            &nbsp;Update
                          </Link>
                          <button
                            className="btn btn-outline-danger mx-2"
                            onClick={() => this.onDelete(post.id)}
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

export default AllProducts;
