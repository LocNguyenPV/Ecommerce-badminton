import apiClient from "../api-client";
import {
  ProductDto,
  ProductDetailDto,
  PagedResult,
  ProductFilters,
  CategoryDto,
  BrandDto,
} from "../../types";

export const productApi = {
  // Lấy danh sách sản phẩm có phân trang và lọc
  getProducts: (filters: ProductFilters): Promise<PagedResult<ProductDto>> => {
    const params = new URLSearchParams();
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        params.append(key, String(value));
      }
    });
    return apiClient
      .get(`/products?${params.toString()}`)
      .then((res) => res.data);
  },

  // Lấy chi tiết một sản phẩm
  getProductById: (id: string): Promise<ProductDetailDto> => {
    return apiClient.get(`/products/${id}`).then((res) => res.data);
  },

  // Lấy danh sách danh mục
  getCategories: (): Promise<CategoryDto[]> => {
    return apiClient.get("/categories").then((res) => res.data);
  },

  // Lấy danh mục theo cây phân cấp
  getCategoriesHierarchy: (): Promise<CategoryDto[]> => {
    return apiClient.get("/categories/hierarchy").then((res) => res.data);
  },

  // Lấy danh sách thương hiệu
  getBrands: (): Promise<BrandDto[]> => {
    return apiClient.get("/brands").then((res) => res.data);
  },

  // Các hàm cho mutation (POST, PUT, DELETE)
  createProduct: (
    productData: Omit<ProductDetailDto, "productId" | "sku">
  ): Promise<ProductDetailDto> => {
    return apiClient.post("/products", productData).then((res) => res.data);
  },

  updateProduct: (
    id: string,
    productData: Partial<ProductDetailDto>
  ): Promise<void> => {
    return apiClient.put(`/products/${id}`, productData);
  },

  deleteProduct: (id: string): Promise<void> => {
    return apiClient.delete(`/products/${id}`);
  },
};
