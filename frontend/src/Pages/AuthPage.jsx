import React, { useState } from "react";
import axios from "axios";
import "../css/Auth.css";
import Logo from "../images/style.jpg";
import fashion from "../images/online-shopping.png";
import swal from "sweetalert"; // Import sweetalert for user notifications
import { Navigate } from "react-router-dom"; // Import Navigate for redirection

const AuthPage = () => {
  const [isSignup, setIsSignup] = useState(false); // Track whether the user is signing up or logging in
  const [formData, setFormData] = useState({ // State for form data
    email: "",
    username: "",
    mobileNumber: "",
    address: "",
    password: "",
    role: "",
  });
  const [userDetails, setUserDetails] = useState(null); // State for storing user details after login

  // Handle changes in form input fields
  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value }); // Update form data based on input field changes
  };

  // Handle form submission for both login and signup
  const handleSubmit = async (e) => {
    e.preventDefault(); // Prevent default form submission behavior
    try {
      if (isSignup) {
        // If the user is signing up, send a registration request
        const response = await axios.post(
          "http://localhost:5266/api/users/register",
          {
            email: formData.email,
            username: formData.username,
            password: formData.password,
            role: formData.role,
            address: formData.address,
            mobileNumber: formData.mobileNumber,
          }
        );
        swal(response.data); // Display success message from the server
        Navigate("/"); // Redirect to the home page after registration
      } else {
        // If the user is logging in, send a login request
        const response = await axios.post(
          "http://localhost:5266/api/users/login",
          {
            email: formData.email,
            password: formData.password,
            role: "Other",
          }
        );
        const { token, refreshToken } = response.data; // Destructure tokens from the response
        console.log(token);
        console.log(refreshToken);

        // Store tokens in localStorage for authentication
        localStorage.setItem("jwtToken", token);
        localStorage.setItem("refreshToken", refreshToken);

        // Fetch user ID using the token
        const idResponse = await axios.get(
          "http://localhost:5266/api/users/user-id",
          {
            headers: {
              Authorization: `Bearer ${token}`, // Include token in the Authorization header
            },
          }
        );

        console.log(idResponse);
        // Fetch user details using the user ID
        const userResponse = await axios.get(
          `http://localhost:5266/api/users/${idResponse.data.userId}`
        );
        setUserDetails(userResponse.data); // Store user details in state
        localStorage.setItem("user", JSON.stringify(userResponse.data)); // Store user details in localStorage

        swal("Login successful! Tokens received."); // Show success alert
        Navigate("/dashboard"); // Redirect to dashboard after successful login
      }
    } catch (error) {
      // Handle any errors that occur during the request
      if (error.response) {
        swal(error.response.data); // Display error message from the server
      } else {
        alert("An error occurred, please try again."); // Display generic error message
      }
    }
  };

  return (
    <div className="page-container">
      <div className="auth-page">
        <div className="auth-illustration">
          <img src={fashion} alt="Illustration" />
        </div>
        <div className="auth-form-container">
          <div className="auth-form">
            <div className="auth-header">
              <img src={Logo} alt="Avatar" className="avatar-icon" />
              <h2>{isSignup ? "Create an Account" : "Welcome"}</h2>
              <h3>{isSignup ? "" : "Back Office - Style Hevan"}</h3>
            </div>
            <form onSubmit={handleSubmit}> {/* Handle form submission */}
              <input
                type="text"
                name="email"
                placeholder="Email"
                value={formData.email} // Bind input value to email
                onChange={handleChange} // Handle input changes
                required
              />
              <input
                type="password"
                name="password"
                placeholder="Password"
                value={formData.password} // Bind input value to password
                onChange={handleChange} // Handle input changes
                required
              />

              {isSignup && ( // Show additional fields only if signing up
                <>
                  <input
                    type="text"
                    name="username"
                    placeholder="Username"
                    value={formData.username} // Bind input value to username
                    onChange={handleChange} // Handle input changes
                    required
                  />
                  <input
                    type="number"
                    name="mobileNumber"
                    placeholder="Mobile Number"
                    value={formData.mobileNumber} // Bind input value to mobile number
                    onChange={handleChange} // Handle input changes
                    required
                  />
                  <input
                    type="text"
                    name="address"
                    placeholder="Address"
                    value={formData.address} // Bind input value to address
                    onChange={handleChange} // Handle input changes
                    required
                  />
                  <select
                    name="role"
                    placeholder="Role"
                    value={formData.role} // Bind input value to role
                    onChange={handleChange} // Handle input changes
                    required
                  >
                    <option value="" disabled>
                      Select Role
                    </option>
                    <option value="Administrator">Administrator</option>
                    <option value="CSR">CSR</option>
                    <option value="Customer">Customer</option>
                    <option value="Vendor">Vendor</option>
                  </select>
                </>
              )}

              <button type="submit" className="auth-btn">
                {isSignup ? "Sign Up" : "Login"} {/* Change button text based on mode */}
              </button>
            </form>

            <div className="auth-footer">
              <a href="#" onClick={() => setIsSignup(!isSignup)}> {/* Toggle signup/login mode */}
                {isSignup
                  ? "Already have an account? Login"
                  : "Don’t have an account? Sign Up"}
              </a>
              <a href="#" className="forgot-password">
                Forgot Password? {/* Link for forgot password */}
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AuthPage;
