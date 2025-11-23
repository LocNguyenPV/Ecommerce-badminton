import axios from "axios";

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || "http://localhost:5201/api/v1", // Thay bằng URL API của bạn
  headers: {
    "Content-Type": "application/json",
  },
});

// Optional: Interceptor để thêm token xác thực vào mọi request
// apiClient.interceptors.request.use((config) => {
//   const token = getAuthToken(); // Hàm của bạn để lấy token
//   if (token) {
//     config.headers.Authorization = `Bearer ${token}`;
//   }
//   return config;
// });

// Optional: Interceptor để xử lý lỗi toàn cục
// apiClient.interceptors.response.use(
//   (response) => response,
//   (error) => {
//     // Xử lý lỗi 401 (Unauthorized), 403 (Forbidden), v.v.
//     return Promise.reject(error);
//   }
// );

export default apiClient;
