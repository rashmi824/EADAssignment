import React, { useState, useEffect } from "react";
import axios from "axios";
import "../css/Auth.css";
import Logo from "../images/style.jpg";
import fashion from "../images/online-shopping.png";
import swal from "sweetalert";
import {
 
  Navigate,
} from "react-router-dom";



const AuthPage = () => {
  const [isSignup, setIsSignup] = useState(false);
  const [formData, setFormData] = useState({
    email: "",
    username: "",
    mobileNumber: "",
    address: "",
    password: "",
    role: "",
  });
  const [userDetails, setUserDetails] = useState(null);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (isSignup) {
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
        swal(response.data); // Success message from the server

        Navigate("/");
      } else {
        const response = await axios.post(
          "http://localhost:5266/api/users/login",
          {
            email: formData.email,
            password: formData.password,
            role: "Other",
          }
        );
        const { token, refreshToken } = response.data;
        console.log(token);

        console.log(refreshToken);

        // Store tokens in localStorage
        localStorage.setItem("jwtToken", token);
        localStorage.setItem("refreshToken", refreshToken);

        // Decode the token to get user details
        const idResponse = await axios.get(
          "http://localhost:5266/api/users/user-id",
          {
            headers: {
              Authorization: `Bearer ${token}`, // Include the token in the Authorization header
            },
          }
        );

        console.log(idResponse);
        const userResponse = await axios.get(
          `http://localhost:5266/api/users/${idResponse.data.userId}`
        );
        setUserDetails(userResponse.data);
        localStorage.setItem("user", JSON.stringify(userResponse.data));

        swal("Login successful! Tokens received.");

        // Redirect based on role
        Navigate("/dashboard");
      }
    } catch (error) {
      if (error.response) {
        swal(error.response.data); // Display server error message
      } else {
        alert("An error occurred, please try again.");
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
            <form onSubmit={handleSubmit}>
              <input
                type="text"
                name="email"
                placeholder="Email"
                value={formData.email}
                onChange={handleChange}
                required
              />
              <input
                type="password"
                name="password"
                placeholder="Password"
                value={formData.password}
                onChange={handleChange}
                required
              />

              {isSignup && (
                <>
                  <input
                    type="text"
                    name="username"
                    placeholder="Username"
                    value={formData.username}
                    onChange={handleChange}
                    required
                  />
                  <input
                    type="number"
                    name="mobileNumber"
                    placeholder="Mobile Number"
                    value={formData.mobileNumber}
                    onChange={handleChange}
                    required
                  />
                  <input
                    type="text"
                    name="address"
                    placeholder="Address"
                    value={formData.address}
                    onChange={handleChange}
                    required
                  />
                  <select
                    name="role"
                    placeholder="Role"
                    value={formData.role}
                    onChange={handleChange}
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
                {isSignup ? "Sign Up" : "Login"}
              </button>
            </form>

            <div className="auth-footer">
              <a href="#" onClick={() => setIsSignup(!isSignup)}>
                {isSignup
                  ? "Already have an account? Login"
                  : "Don’t have an account? Sign Up"}
              </a>
              <a href="#" className="forgot-password">
                Forgot Password?
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AuthPage;
