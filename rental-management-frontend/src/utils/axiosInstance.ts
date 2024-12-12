import axios from "axios";

const axiosInstance = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
  withCredentials: true,
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    console.log("Request:", config); // Log the request details
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const { data } = await axiosInstance.post(
          "/Authentication/RefreshToken",
          {},
          { withCredentials: true }
        );
        const { accessToken } = data;

        // Store the new access token in local storage
        localStorage.setItem("accessToken", accessToken);

        // Update the Authorization header for the original request
        originalRequest.headers["Authorization"] = `Bearer ${accessToken}`;

        // Retry the original request with the new token
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        console.error("Refresh token", refreshError);
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default axiosInstance;
