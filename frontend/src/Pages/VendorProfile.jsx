import React, { useState, useEffect } from "react";
import axios from "axios";
import "../css/vendor.css"; // Import the CSS file
import profile from "../images/profile.png"; // Import the profile image
import swal from "sweetalert"; // Import the sweetalert library

const VendorProfile = ({ userId }) => {
  const [vendor, setVendor] = useState({
    username: "",
    email: "",
    mobileNumber: "",
    address: "",
    password: "",
    role: "",
  });
  const [loading, setLoading] = useState(true); // State to manage loading state
  const [error, setError] = useState(null); // State to manage error messages
  const [isEditing, setIsEditing] = useState(false); // Track editing state

  useEffect(() => {
    // Function to fetch vendor details
    const fetchVendor = async () => {
      try {
        const response = await axios.get(
          `http://localhost:5266/api/users/${userId}`
        );
        if (response.status === 200) {
          const data = response.data;
          setVendor(data); // Set the vendor details from the response
        } else {
          setError("Failed to fetch vendor details");
        }
      } catch (error) {
        setError("Error: " + error.message); // Set error message if fetching fails
      } finally {
        setLoading(false); // Set loading to false after fetch attempt
      }
    };

    fetchVendor(); // Call the fetchVendor function
  }, [userId]); // Fetch when userId changes

  // Handle input changes in the form
  const handleChange = (e) => {
    const { name, value } = e.target;
    setVendor({ ...vendor, [name]: value }); // Update vendor state with new input values
  };

  // Handle vendor details update
  const handleUpdate = async (e) => {
    e.preventDefault(); // Prevent default form submission
    const userUpdateDto = {
      username: vendor.username,
      email: vendor.email,
      mobileNumber: vendor.mobileNumber,
      address: vendor.address,
      password: vendor.password || '', // Include password only if needed
      role: vendor.role, // Set the role as necessary, or fetch it from the vendor state if available
    };

    try {
      const response = await axios.put(
        `http://localhost:5266/api/users/${userId}`,
        userUpdateDto // Send updated vendor details
      );
      if (response.status === 200) {
        setIsEditing(false); // Exit editing mode on successful update
        swal("Vendor details updated successfully"); // Show success alert
      } else {
        setError("Failed to update vendor details");
      }
    } catch (error) {
      console.log("Error: " + error.response.data); // Set error message if updating fails
    }
  };

  // Loading state rendering
  if (loading) return <p>Loading vendor details...</p>;
  // Error state rendering
  if (error) return <p>{error}</p>;

  return (
    <div className="vendor-profile">
      <h2>My Profile</h2>

      <img
        src={profile}
        alt={`${vendor.username}'s profile`}
        className="profile-image" // Display vendor profile image
      />

      <form onSubmit={handleUpdate} className="vendor-form">
        <div className="vendor-field">
          <strong>Username:</strong>
          {isEditing ? (
            <input
              type="text"
              name="username"
              value={vendor.username} // Bind input value to vendor.username
              onChange={handleChange} // Handle input changes
              required // Make field required
            />
          ) : (
            <p>{vendor.username}</p> // Display username if not editing
          )}
        </div>
        <div className="vendor-field">
          <strong>Email:</strong>
          {isEditing ? (
            <input
              type="email"
              name="email"
              value={vendor.email} // Bind input value to vendor.email
              onChange={handleChange} // Handle input changes
              required // Make field required
            />
          ) : (
            <p>{vendor.email}</p> // Display email if not editing
          )}
        </div>
        <div className="vendor-field">
          <strong>Password:</strong>
          {isEditing ? (
            <input
              type="input"
              name="password"
              value={vendor.password} // Bind input value to vendor.password
              onChange={handleChange} // Handle input changes
              
            />
          ) : (

            <p>********</p> // Display password if not editing
          )}
        </div>


        <div className="vendor-field">
          <strong>Mobile Number:</strong>
          {isEditing ? (
            <input
              type="text"
              name="mobileNumber"
              value={vendor.mobileNumber} // Bind input value to vendor.mobileNumber
              onChange={handleChange} // Handle input changes
              required // Make field required
            />
          ) : (
            <p>{vendor.mobileNumber}</p> // Display mobile number if not editing
          )}
        </div>
        <div className="vendor-field">
          <strong>Address:</strong>
          {isEditing ? (
            <input
              type="text"
              name="address"
              value={vendor.address} // Bind input value to vendor.address
              onChange={handleChange} // Handle input changes
              required // Make field required
            />
          ) : (
            <p>{vendor.address}</p> // Display address if not editing
          )}
        </div>

        <div className="profile-button-group">
          <button
            type="button"
            onClick={() => setIsEditing(!isEditing)} // Toggle edit mode
            className="edit-button"
          >
            {isEditing ? "Cancel" : "Edit"}
          </button>
          {isEditing && (
            <button type="submit" className="update-button">
              Update {/* Show update button when in edit mode */}
            </button>
          )}
        </div>
      </form>
    </div>
  );
};

export default VendorProfile;
