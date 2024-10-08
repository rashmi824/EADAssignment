import React, { useState, useEffect } from "react";
import axios from "axios";
import swal from "sweetalert";
import RegisterModal from "../components/RegisterModal";
import "../css/userTable.css";

function VendorManagement() {
  const [userId, setUserId] = useState("");
  const [showRegisterUserModal, setShowRegisterUserModal] = useState(false);
  const [users, setUsers] = useState([]);
  const [isUserRegistered, setIsUserRegistered] = useState(false);
  const [search, setSearch] = useState("");
  const [activeTab, setActiveTab] = useState("new");

  // Fetch Users when the component mounts or when a user is registered
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get("http://localhost:5266/api/users");
        setUsers(response.data); // Set the fetched users in state
      } catch (error) {
        console.error("Error fetching users", error.response.data);
        swal("Error fetching users", "Please try again later.", "error");
      }
    };
    fetchUsers();
  }, [isUserRegistered]); // Dependency array ensures fetch is triggered on user registration

  // Close the registration modal and reset registration status
  const handleRegisterUserModalClose = () => {
    setShowRegisterUserModal(false);
    setIsUserRegistered(false);
  };

  // Show the registration modal
  const handleRegisterUserModalShow = () => {
    setShowRegisterUserModal(true);
  };

  // Delete a user with confirmation
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
            setIsUserRegistered(true); // Update registration status to fetch users again
            swal("User deleted successfully!", "", "success");
            window.location.reload(); // Reload the page to see updated users
          })
          .catch((err) => {
            console.error(err);
            swal(
              "Error deleting user",
              "Please try again later.",
              err.response.data
            );
          });
      } else {
        swal("Delete cancelled successfully!"); // Inform user of cancellation
      }
    });
  };

  // Approve a user and activate them
  const Approve = (id) => {
    axios
      .put(`http://localhost:5266/api/users/approve-customer/${id}`, true, {
        headers: {
          "Content-Type": "application/json",
        },
      })
      .then(() => {
        Activate(id); // Activate the user after approving
        window.location.reload(); // Reload to see changes
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Approving user",
          "Please try again later.",
          err.response.data
        );
      });
  };

  // Disapprove a user and deactivate them
  const disApprove = (id) => {
    axios
      .put(`http://localhost:5266/api/users/approve-customer/${id}`, false, {
        headers: {
          "Content-Type": "application/json",
        },
      })
      .then(() => {
        Deactivate(id); // Deactivate the user after disapproving
        window.location.reload(); // Reload to see changes
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Disapproving user or User Already Disapproved",
          "Please try again later.",
          err.response.data
        );
      });
  };

  // Activate a user
  const Activate = (id) => {
    axios
      .put(`http://localhost:5266/api/users/activate/${id}`)
      .then(() => {
        swal("User Activated successfully!", "", "success");
        window.location.reload(); // Reload to see changes
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Activating user",
          "Please try again later.",
          err.response.data
        );
      });
  };

  // Deactivate a user
  const Deactivate = (id) => {
    axios
      .put(`http://localhost:5266/api/users/deactivate/${id}`)
      .then(() => {
        swal("User Deactivated successfully!", "", "success");
        window.location.reload(); // Reload to see changes
      })
      .catch((err) => {
        console.error(err);
        swal(
          "Error Deactivating user",
          "Please try again later.",
          err.response.data
        );
      });
  };

  // Handle search input changes
  const handleSearch = (e) => {
    setSearch(e.target.value); // Update search state
  };

  // Filter users based on active tab and search input
  const filteredUsers = users.filter(
    (user) =>
      user.role === "Vendor" && // Filter by role
      user.username?.toLowerCase().includes(search.toLowerCase()) // Filter by username
  );

  return (
    <div className="unique-content">
      <RegisterModal
        show={showRegisterUserModal} // Show or hide modal based on state
        handleClose={handleRegisterUserModalClose}
        setIsUserRegistered={setIsUserRegistered} // Pass function to update registration state
      />
      <div className="unique-search-bar">
        <input
          type="text"
          className="unique-search-input"
          placeholder="Search Users..."
          value={search} // Controlled input for search
          onChange={handleSearch} // Update search state on change
        />
        <button
          className="unique-btn unique-btn-primary"
          onClick={handleRegisterUserModalShow} // Show register modal on click
        >
          Register New Vendor
        </button>
      </div>

      <div className="user-header">
        <h2 className="unique-content-title">Vendor Management</h2>
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
                  <button
                    className="unique-btn unique-btn-1"
                    onClick={() => Activate(user.id)} // Activate user
                  >
                    Activate
                  </button>

                  <button
                    className="unique-btn unique-btn-2"
                    onClick={() => Deactivate(user.id)} // Deactivate user
                  >
                    Deactivate
                  </button>
                  <button
                    className="unique-btn unique-btn-success"
                    onClick={() => handleRegisterUserModalShow(user.id)} // Show edit modal
                  >
                    Edit
                  </button>
                  <button
                    className="unique-btn unique-btn-danger"
                    onClick={() => deleteUser(user.id)} // Delete user
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default VendorManagement;
