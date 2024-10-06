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
    averageRating: "",
    comments: [],
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
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
          setVendor(data); // Set the vendor details
        } else {
          setError("Failed to fetch vendor details");
        }
      } catch (error) {
        setError("Error: " + error.message);
      } finally {
        setLoading(false);
      }
    };

    fetchVendor();
  }, [userId]); // Fetch when userId changes

  const handleChange = (e) => {
    const { name, value } = e.target;
    setVendor({ ...vendor, [name]: value });
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.put(
        `http://localhost:5266/api/users/${userId}`,
        vendor
      );
      if (response.status === 200) {
        setIsEditing(false); // Exit editing mode
        swal("Vendor details updated successfully");
      } else {
        setError("Failed to update vendor details");
      }
    } catch (error) {
      setError("Error: " + error.message);
    }
  };

  if (loading) return <p>Loading vendor details...</p>;
  if (error) return <p>{error}</p>;

  return (
 
      <div className="vendor-profile">
        <h2>My Profile</h2>

        <img
          src={profile}
          alt={`${vendor.username}'s profile`}
          className="profile-image"
        />

        <form onSubmit={handleUpdate} className="vendor-form">
          <div className="vendor-field">
            <strong>Username:</strong>
            {isEditing ? (
              <input
                type="text"
                name="username"
                value={vendor.username}
                onChange={handleChange}
                required
              />
            ) : (
              <p>{vendor.username}</p>
            )}
          </div>
          <div className="vendor-field">
            <strong>Email:</strong>
            {isEditing ? (
              <input
                type="email"
                name="email"
                value={vendor.email}
                onChange={handleChange}
                required
                disabled
              />
            ) : (
              <p>{vendor.email}</p>
            )}
          </div>
          <div className="vendor-field">
            <strong>Mobile Number:</strong>
            {isEditing ? (
              <input
                type="text"
                name="mobileNumber"
                value={vendor.mobileNumber}
                onChange={handleChange}
                required
              />
            ) : (
              <p>{vendor.mobileNumber}</p>
            )}
          </div>
          <div className="vendor-field">
            <strong>Address:</strong>
            {isEditing ? (
              <input
                type="text"
                name="address"
                value={vendor.address}
                onChange={handleChange}
                required
              />
            ) : (
              <p>{vendor.address}</p>
            )}
          </div>
          {vendor.averageRating !== null &&
            vendor.averageRating !== undefined && (
              <div className="vendor-field">
                <strong>Average Rating:</strong>
                <p>{vendor.averageRating || 0}</p>
              </div>
            )}
          {vendor.comments ? (
            <div className="vendor-field">
              <h3>Comments:</h3>
              {vendor.comments.length > 0 ?(
              <ul>
                {vendor.comments.map((comment, index) => (
                  <li key={index}>{comment}</li>
                ))}
              </ul>)
              :(<p>No comments yet</p>)}
            </div>
          ) : null}

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
                Update
              </button>
            )}
          </div>
        </form>
      </div>

  );
};

export default VendorProfile;
