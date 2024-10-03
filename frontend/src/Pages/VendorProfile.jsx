import React, { useState, useEffect } from "react";
import axios from "axios";

const VendorProfile = ({ userId }) => {
  const [vendor, setVendor] = useState({
    username: "",
    email: "",
    mobileNumber: "",
    address: "",
    averageRating: "",
    comments: [],
  }); // Initially no vendor data
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

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

  if (loading) return <p>Loading vendor details...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div className="vendor-profile">
      <h2>{vendor.username}'s Vendor Profile</h2>
      <p>
        <strong>Email:</strong> {vendor.email}
      </p>
      <p>
        <strong>Mobile Number:</strong> {vendor.mobileNumber}
      </p>
      <p>
        <strong>Address:</strong> {vendor.address}
      </p>
      <p>
        <strong>Average Rating:</strong> {vendor.averageRating}
      </p>
      <div>
        <h3>Comments:</h3>
        {vendor.comments && vendor.comments.length > 0 ? (
          <ul>
            {vendor.comments.map((comment, index) => (
              <li key={index}>{comment}</li>
            ))}
          </ul>
        ) : (
          <p>No comments available.</p>
        )}
      </div>
    </div>
  );
};

export default VendorProfile;
