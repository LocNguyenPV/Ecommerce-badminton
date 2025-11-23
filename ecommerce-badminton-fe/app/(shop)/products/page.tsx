"use client";

import { useState } from "react";
import { useProducts } from "@/hooks/use-products";
import { ProductFilters } from "@/types";

export default function ProductsPage() {
  // State để quản lý bộ lọc và phân trang
  const [filters, setFilters] = useState<ProductFilters>({
    page: 1,
    pageSize: 20,
    sortBy: "name",
    sortOrder: "asc",
  });

  // Sử dụng hook để fetch data
  const { data, isLoading, isError, error } = useProducts(filters);

  const handlePageChange = (newPage: number) => {
    setFilters((prev) => ({ ...prev, page: newPage }));
  };

  const handleFilterChange = (newFilters: Partial<ProductFilters>) => {
    setFilters((prev) => ({ ...prev, ...newFilters, page: 1 })); // Reset về trang 1 khi lọc
  };

  if (isLoading) {
    return <div>Đang tải sản phẩm...</div>;
  }

  if (isError) {
    return <div>Lỗi: {error.message}</div>;
  }

  return (
    <div>
      <h1>Danh sách sản phẩm</h1>

      {/* Phần bộ lọc (ví dụ đơn giản) */}
      <div className="filters mb-4">
        <input
          type="text"
          placeholder="Tìm kiếm..."
          onChange={(e) => handleFilterChange({ searchTerm: e.target.value })}
        />
        {/* Thêm các input lọc khác: categoryId, brandId, minPrice, maxPrice */}
      </div>

      <div className="product-grid">
        {data?.items.map((product) => (
          <ProductCard key={product.productId} product={product} />
        ))}
      </div>

      {/* Phân trang */}
      <div className="pagination-controls mt-4">
        <button
          onClick={() => handlePageChange(filters.page! - 1)}
          disabled={!data?.hasPreviousPage}
        >
          Trang trước
        </button>
        <span>
          Trang {filters.page} / {data?.totalPages}
        </span>
        <button
          onClick={() => handlePageChange(filters.page! + 1)}
          disabled={!data?.hasNextPage}
        >
          Trang sau
        </button>
      </div>
    </div>
  );
}
