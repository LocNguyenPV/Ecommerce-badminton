// Copy các types bạn đã cung cấp vào đây
export interface ProductDto {
  productId: string;
  sku: string;
  name: string;
  description: string;
  basePrice: number;
  categoryName: string;
  brandName: string;
  isActive: boolean;
  primaryImageUrl: string;
}

export interface ProductDetailDto extends ProductDto {
  costPrice: number;
  category: CategoryDto;
  brand: BrandDto;
  variants: ProductVariantDto[];
  images: ProductImageDto[];
}

export interface ProductVariantDto {
  variantId: string;
  sku: string;
  variantName: string;
  finalPrice: number;
  availableQuantity: number;
  attributes: VariantAttributeDto[];
}

// Thêm các type còn thiếu để đảm bảo code hoàn chỉnh
export interface VariantAttributeDto {
  attributeId: string;
  name: string;
  value: string;
}

export interface ProductImageDto {
  imageId: string;
  imageUrl: string;
  isPrimary: boolean;
}

export interface CategoryDto {
  categoryId: string;
  name: string;
  parentId?: string;
}

export interface BrandDto {
  brandId: string;
  name: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Interface cho các bộ lọc
export interface ProductFilters {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
  brandId?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
