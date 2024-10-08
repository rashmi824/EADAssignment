import React, { useState, useEffect } from "react";
import axios from "axios";
import swal from "sweetalert";
import RegisterModal from "../components/RegisterModal";
import "../css/userTable.css";

function CustomerManagement() {
  const [userId, setUserId] = useState("");
  const [showRegisterUserModal, setShowRegisterUserModal] = useState(false);
  const [users, setUsers] = useState([]);
  const [isUserRegistered, setIsUserRegistered] = useState(false);
  const [search, setSearch] = useState("");
  const [activeTab, setActiveTab] = useState("new");

  // Fetch Users when component mounts or when isUserRegistered state changes
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get("http://localhost:5266/api/users");
        setUsers(response.data); // Set the users state with the fetched data
      } catch (error) {
        console.error("Error fetching users", error.response.data);
        swal("Error fetching users", "Please try again later.", "error"); // Show error alert
      }
    };
    fetchUsers();
  }, [isUserRegistered]);

  // Close the registration modal and reset registration state
  const handleRegisterUserModalClose = () => {
    setShowRegisterUserModal(false);
    setIsUserRegistered(false);
  };

  // Show the registration modal
  const handleRegisterUserModalShow = () => {
    setShowRegisterUserModal(true);
  };

  // Delete a user by their ID with a confirmation alert
  const deleteUser = (id) => {
    swal({
      title: "Are you sure?",
      text: "Once deleted, you will not be able to recover this record!",
      icon: "warning",
      buttons: true,
      dangerMode: true,
    }).then((willDelete) => {
      if (willDelete) {
        axios
          .delete(`http://localhost:5266/api/users/${id}`)
          .then(() => {
            setIsUserRegistered(true); // Trigger user re-fetch
            swal("User deleted successfully!", "", "success"); // Show success alert
            window.location.reload(); // Reload the page to update the user list
          })
          .catch((err) => {
            console.error(err);
            swal(
              "Error deleting user",
              "Please try again later.",
              err.response.data
            ); // Show error alert
          });
      } else {
        swal("Delete cancelled successfully!"); // Show cancellation alert
      }
    });
  };

  // Approve a customer by their ID and activate them
  const Approve = (id) => {
    axios
      .put(`http://localhost:5266/api/users/approve-customer/${id}`, true, {
        headers: {
          "Content-Type": "application/json",
        },
      })
      .then(() => {
        Activate(id); // Activate the user after approval
        window.location.reload(); // Reload the page to update the user list
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Approving user",
          "Please try again later.",
          err.response.data
        ); // Show error alert
      });
  };

  // Disapprove a customer by their ID and deactivate them
  const disApprove = (id) => {
    axios
      .put(`http://localhost:5266/api/users/approve-customer/${id}`, false, {
        headers: {
          "Content-Type": "application/json",
        },
      })
      .then(() => {
        Deactivate(id); // Deactivate the user after disapproval
        window.location.reload(); // Reload the page to update the user list
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Disapproving user or User Already Disapproved",
          "Please try again later.",
          err.response.data
        ); // Show error alert
      });
  };

  // Activate a user by their ID
  const Activate = (id) => {
    axios
      .put(`http://localhost:5266/api/users/activate/${id}`)
      .then(() => {
        swal("User Activated successfully!", "", "success"); // Show success alert
        window.location.reload(); // Reload the page to update the user list
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Activating user",
          "Please try again later.",
          err.response.data
        ); // Show error alert
      });
  };

  // Deactivate a user by their ID
  const Deactivate = (id) => {
    axios
      .put(`http://localhost:5266/api/users/deactivate/${id}`)
      .then(() => {
        swal("User Deactivated successfully!", "", "success"); // Show success alert
        window.location.reload(); // Reload the page to update the user list
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Deactivating user",
          "Please try again later.",
          err.response.data
        ); // Show error alert
      });
  };

  // Handle search input change
  const handleSearch = (e) => {
    setSearch(e.target.value); // Update search state
  };

  // Filter users based on active tab, role, and search input
  const filteredUsers = users.filter(
    (user) =>
      user.role === "Customer" && // Filter by role
      user.username?.toLowerCase().includes(search.toLowerCase()) && // Filter by search input
      (activeTab === "new"
        ? !user.isApproved && user.status // New customers
        : activeTab === "active"
        ? user.isApproved && user.status // Active customers
        : activeTab === "Reject"
        ? !user.isApproved && !user.status // Rejected customers
        : user.isApproved && !user.status) // Inactive customers
  );

  return (
    <div className="unique-content">
      {/* Register Modal */}
      <RegisterModal
        show={showRegisterUserModal}
        handleClose={handleRegisterUserModalClose}
        setIsUserRegistered={setIsUserRegistered}
      />
      <div className="unique-search-bar">
        <input
          type="text"
          className="unique-search-input"
          placeholder="Search Users..."
          value={search}
          onChange={handleSearch} // Update search state on input change
        />
      </div>

      <div className="user-header">
        <h2 className="unique-content-title">Customer Management</h2>

        <div className="tabs-container">
          {/* Tab buttons for different customer categories */}
          <button
            className={`tab-btn ${activeTab === "new" ? "active" : ""}`}
            onClick={() => setActiveTab("new")}
          >
            New Customers
          </button>
          <button
            className={`tab-btn ${activeTab === "active" ? "active" : ""}`}
            onClick={() => setActiveTab("active")}
          >
            Active Customers
          </button>
          <button
            className={`tab-btn ${activeTab === "Reject" ? "active" : ""}`}
            onClick={() => setActiveTab("Reject")}
          >
            Rejected Customers
          </button>
          <button
            className={`tab-btn ${activeTab === "Inactive" ? "active" : ""}`}
            onClick={() => setActiveTab("Inactive")}
          >
            Inactive Customers
          </button>
        </div>

        {/* Display the title based on the active tab */}
        <h2 className="unique-content-title">
          {activeTab === "new"
            ? "New Customers"
            : activeTab === "active"
            ? "Active Customers"
            : activeTab === "Reject"
            ? "Rejected Customers"
            : "Inactive Customers"}
        </h2>
      </div>
      <div className="unique-table-container">
        <table className="unique-user-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Username</th>
              <th>Email</th>
              <th>Address</th>
              <th>Mobile Number</th>
              <th>Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {filteredUsers.map((user, index) => (
              <tr key={user._id}>
                <td>{index + 1}</td>
                <td>{user.username}</td>
                <td>{user.email}</td>
                <td>{user.address}</td>
                <td>{user.mobileNumber}</td>

                <td>{user.status === true ? "Active" : "Deactive"}</td>

                <td>
                  {activeTab === "new" || activeTab === "Reject" ? (
                    <>
                      <button
                        className="unique-btn unique-btn-success"
                        onClick={() => Approve(user.id)}
                      >
                        Approve
                      </button>
                      <button
                        className="unique-btn unique-btn-danger"
                        onClick={() => disApprove(user.id)}
                      >
                        Reject
                      </button>
                    </>
                  ) : activeTab === "Inactive" ? (
                    <>
                      <button
                        className="unique-btn unique-btn-1"
                        onClick={() => Activate(user.id)}
                      >
                        Activate
                      </button>
                    </>
                  ) : (
                    <>
                      <button
                        className="unique-btn unique-btn-danger"
                        onClick={() => deleteUser(user.id)}
                      >
                        Delete
                      </button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

    </div>
  );
}

export default CustomerManagement;
