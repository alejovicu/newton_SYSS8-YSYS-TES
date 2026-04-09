import React, { useState, useEffect } from "react";
import Signup from "./components/Signup.jsx";
import Login from "./components/Login.jsx";
import { api } from "./api.js";

export default function App() {
  const [token, setToken] = useState(() => localStorage.getItem("token") || null);
  const [user, setUser] = useState(null);
  const [screen, setScreen] = useState("login"); // "login" or "signup"
  const [newProductName, setNewProductName] = useState("");
  const [allProducts, setAllProducts] = useState([]); // For admin
  const [showModal, setShowModal] = useState(false);
  const [availableProducts, setAvailableProducts] = useState([]);

  // Fetch current user after login
  useEffect(() => {
    if (token) {
      localStorage.setItem("token", token);

      const fetchUser = async () => {
        try {
          const data = await api("/user", "GET", null, token);
          setUser(data);

          // If admin, fetch all products
          if (data.id === 1) {
            const products = await api("/products", "GET", null, token);
            setAllProducts(products);
          }
        } catch (err) {
          console.error("Failed to fetch user:", err.message);
          handleLogout();
        }
      };

      fetchUser();
    } else {
      localStorage.removeItem("token");
      setUser(null);
      setAllProducts([]);
    }
  }, [token]);

  // Logout function
  const handleLogout = () => {
    setToken(null);
    setUser(null);
    setAllProducts([]);
    setScreen("login");
  };

  // Admin-only: create new product
  const handleCreateProduct = async () => {
    if (!newProductName) return;
    try {
      const product = await api("/product", "POST", { name: newProductName }, token);
      setAllProducts((prev) => [...prev, product]);
      setNewProductName("");
    } catch (err) {
      alert("Failed to create product: " + err.message);
    }
  };

  // Admin-only: delete product
  const handleDeleteProduct = async (id) => {
    try {
      await api(`/product/${id}`, "DELETE", null, token);
      setAllProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      alert("Failed to delete product: " + err.message);
    }
  };


  // Open modal and fetch available products
  const openAddProductModal = async () => {
    try {
      const allProducts = await api("/products", "GET", null, token);
      const userProductIds = user.products.map((p) => p.id);
      const filtered = allProducts.filter((p) => !userProductIds.includes(p.id));
      setAvailableProducts(filtered);
      setShowModal(true);
    } catch (err) {
      alert("Failed to load products: " + err.message);
    }
  };


  const handleAddUserProduct = async (product) => {
    try {
      await api(`/user/product/${product.id}`, "POST", null, token);

      // Update user state with the newly added product
      setUser((prev) => ({
        ...prev,
        products: [...prev.products, product],
      }));

      // Remove from modal list
      setAvailableProducts((prev) => prev.filter((p) => p.id !== product.id));
    } catch (err) {
      alert("Failed to add product: " + err.message);
    }
  };

  const handleDeleteUserProduct = async (id) => {
    try {
      await api(`/user/product/${id}`, "DELETE", null, token);

      // Remove the deleted product from user's products
      setUser((prev) => ({
        ...prev,
        products: prev.products.filter((p) => p.id !== id),
      }));

      // Optionally, also add it back to availableProducts so it can be re-added
      setAvailableProducts((prev) => [
        ...prev,
        user.products.find((p) => p.id === id),
      ]);
    } catch (err) {
      alert("Failed to delete user product: " + err.message);
    }
  };

  // Before login: show Login or Signup screen
  if (!token) {
    return (
      <div>
        <h1>Shopping Cart App</h1>
        {screen === "login" ? (
          <>
            <Login setToken={setToken} />
            <p>
              Don't have an account?{" "}
              <button id='signup' onClick={() => setScreen("signup")}>Sign Up</button>
            </p>
          </>
        ) : (
          <>
            <Signup />
            <p>
              Already have an account?{" "}
              <button className="btn btn-blue" onClick={() => setScreen("login")}>Login</button>
            </p>
          </>
        )}
      </div>
    );
  }

  // After login but user info still loading
  if (!user) return <p>Loading user info...</p>;

  // After login and user loaded
  return (
    <div>
      <h1>Shopping Cart App</h1>

      <h2>Welcome, {user.username}!</h2>

      {user.id === 1 ? (
        <div>
          <h3>Product Catalog:</h3>
          {allProducts.length === 0 ? (
            <p>No products available.</p>
          ) : (
            <div
              className="product-grid"
            >
              {allProducts.map((product) => (
                <div
                  key={product.id}
                  className="product-item"
                >
                  <span>{product.name}</span>
                  <button
                    onClick={() => handleDeleteProduct(product.id)}
                    className="product-item-button"
                  >
                    Delete
                  </button>
                </div>
              ))}
            </div>
          )}

          <div style={{ marginTop: "20px" }}>
            <h3>Add new product:</h3>
            <input
              type="text"
              placeholder="Product Name"
              value={newProductName}
              onChange={(e) => setNewProductName(e.target.value)}
            />
            <button onClick={handleCreateProduct}>Create Product</button>
          </div>
        </div>
      ) : (
        // Normal user view: assigned products only
        <div>
          <h3>Your Products:</h3>
          {user.products.length === 0 ? (
            <p>No products assigned.</p>
          ) : (
            <div
              style={{
                display: "grid",
                gridTemplateColumns: "repeat(auto-fill, minmax(150px, 1fr))",
                gap: "10px",
              }}
            >
              {user.products.map((product) => (
                <div
                  key={product.id}
                  style={{
                    border: "1px solid #ccc",
                    padding: "10px",
                    borderRadius: "5px",
                  }}
                >
                  {product.name}
                  <button
                    onClick={() => handleDeleteUserProduct(product.id)}
                    className="product-item-button"
                  >
                    Delete
                  </button>
                </div>
              ))}
            </div>
          )}

          <button onClick={openAddProductModal} style={{ marginTop: "20px" }}>
            Add Product
          </button>
        </div>
      )}

{/* Modal */}
      {showModal && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            background: "rgba(0,0,0,0.5)",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <div
            style={{
              background: "#fff",
              padding: "20px",
              borderRadius: "8px",
              minWidth: "300px",
            }}
          >
            <h3>Select Products</h3>
            {availableProducts.length === 0 ? (
              <p>No more products available.</p>
            ) : (
              <ul>
                {availableProducts.map((p) => (
                  <li key={p.id} style={{ marginBottom: "10px" }}>
                    {p.name}
                    <button
                      style={{ marginLeft: "10px" }}
                      onClick={() => handleAddUserProduct(p)}
                    >
                      Add
                    </button>
                  </li>
                ))}
              </ul>
            )}

            <button onClick={() => setShowModal(false)} style={{ marginTop: "10px" }}>
              Close
            </button>
          </div>
        </div>
      )}

      <button onClick={handleLogout} style={{ marginBottom: "10px" }}>
        Logout
      </button>
    </div>
  );
}
