import { useQuery } from "@tanstack/react-query";
import { productApi } from "@/lib/services/product.service";
import { ProductFilters, PagedResult, ProductDto } from "@/types";

// Key cho query cache của sản phẩm
export const productsQueryKey = "products";

export const useProducts = (filters: ProductFilters) => {
  return useQuery<PagedResult<ProductDto>, Error>({
    // Query key là một mảng, giúp TanStack Query xác định duy nhất mỗi request.
    // Khi `filters` thay đổi, key sẽ thay đổi và query sẽ tự động fetch lại.
    queryKey: [productsQueryKey, filters],

    // Hàm fetch data
    queryFn: () => productApi.getProducts(filters),

    // Cấu hình cache: dữ liệu được coi là 'stale' sau 5 phút.
    // Trong 5 phút, component sẽ không fetch lại data nếu mount lại.
    staleTime: 5 * 60 * 1000,

    // Thời gian data được giữ trong cache sau khi component unmount
    gcTime: 10 * 60 * 1000,
  });
};
