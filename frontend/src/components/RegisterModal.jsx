import React, { useState, useEffect } from "react";
import axios from "axios";
import Modal from "react-bootstrap/Modal";
import Button from "react-bootstrap/Button";
import swal from "sweetalert";
import "../css/RegisterModal.css"; // Import the unique CSS

const RegisterModal = ({ show, handleClose, setIsUserRegistered }) => {
  // State for storing form inputs
  const [inputs, setInputs] = useState({
    email: "",
    username: "",
    password: "",
    role: "Vendor", // Default role
    address: "",
    mobileNumber: "",
  });

  // State for form validation errors
  const [formErrors, setFormErrors] = useState({});
  // State to track form submission
  const [submitted, isSubmitted] = useState(false);

  // Handle input changes
  const handleChange = (e) => {
    setInputs((prev) => ({
      ...prev,
      [e.target.name]: e.target.value, // Update the corresponding input field
    }));
  };

  // Validate input values
  const validate = (values) => {
    const errors = {};
    // Check for required fields and validate their formats
    if (!values.email) {
      errors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(values.email)) {
      errors.email = "Email is invalid";
    }
    if (!values.username) {
      errors.username = "Username is required";
    }
    if (!values.password) {
      errors.password = "Password is required";
    }
    if (!values.role) {
      errors.role = "Role is required";
    }
    if (!values.address) {
      errors.address = "Address is required";
    }
    if (!values.mobileNumber) {
      errors.mobileNumber = "Mobile Number is required";
    } else if (values.mobileNumber.length !== 10) {
      errors.mobileNumber = "Invalid Mobile Number";
    }
    return errors; // Return the validation errors
  };

  // Handle form submission
  const handleSubmit = async (e) => {
    e.preventDefault(); // Prevent default form submission
    const errors = validate(inputs); // Validate inputs
    setFormErrors(errors); // Update the form errors state
    isSubmitted(true); // Mark the form as submitted
  };

  // Effect to handle successful submission and API call
  useEffect(() => {
    // If there are no form errors and the form was submitted
    if (Object.keys(formErrors).length === 0 && submitted) {
      axios
        .post("http://localhost:5266/api/users/register", inputs) // API call to register user
        .then((res) => {
          swal("Vendor Registered Successfully"); // Show success alert
          setIsUserRegistered(true); // Update user registration status
          handleClose(); // Close the modal
        })
        .catch((error) => {
          console.log(error); // Log error for debugging
          swal(error.response.data); // Show error alert
        });
    }
  }, [formErrors, submitted]); // Dependencies for the effect

  return (
    <Modal show={show} onHide={handleClose} centered className="register-modal">
      <Modal.Header className="register-modal-header">
        <Modal.Title>Register User</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {/* Form fields for user registration */}
        <div className="register-form-group">
          <label className="register-form-label">Email:</label>
          <input
            type="email"
            className="register-form-control"
            name="email"
            onChange={handleChange}
            required
          />
          <p className="register-error">{formErrors.email}</p>
        </div>

        <div className="register-form-group">
          <label className="register-form-label">Username:</label>
          <input
            type="text"
            className="register-form-control"
            name="username"
            onChange={handleChange}
            required
          />
          <p className="register-error">{formErrors.username}</p>
        </div>

        <div className="register-form-group">
          <label className="register-form-label">Password:</label>
          <input
            type="password"
            className="register-form-control"
            name="password"
            onChange={handleChange}
            required
          />
          <p className="register-error">{formErrors.password}</p>
        </div>

        <div className="register-form-group">
          <label className="register-form-label">Address:</label>
          <input
            type="text"
            className="register-form-control"
            name="address"
            onChange={handleChange}
            required
          />
          <p className="register-error">{formErrors.address}</p>
        </div>

        <div className="register-form-group">
          <label className="register-form-label">Mobile Number:</label>
          <input
            type="text"
            className="register-form-control"
            name="mobileNumber"
            onChange={handleChange}
            required
          />
          <p className="register-error">{formErrors.mobileNumber}</p>
        </div>
      </Modal.Body>
      <Modal.Footer className="register-modal-footer">
        <Button
          type="submit"
          className="unique-btn unique-btn-success"
          onClick={handleSubmit} // Call handleSubmit on button click
        >
          Register
        </Button>
        <Button className="unique-btn unique-btn-danger" onClick={handleClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default RegisterModal; // Export the component
