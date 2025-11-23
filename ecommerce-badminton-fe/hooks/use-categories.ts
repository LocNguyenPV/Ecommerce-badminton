import { useQuery } from "@tanstack/react-query";
import { productApi } from "@/lib/services/product.service";
import { CategoryDto } from "@/types";

export const categoriesQueryKey = "categories";

export const useCategories = () => {
  return useQuery<CategoryDto[], Error>({
    queryKey: [categoriesQueryKey],
    queryFn: productApi.getCategories,

    // Danh mục rất hiếm khi thay đổi, có thể cache rất lâu
    staleTime: 30 * 60 * 1000, // 30 phút
  });
};

// Hook cho danh mục phân cấp
export const useCategoriesHierarchy = () => {
  return useQuery<CategoryDto[], Error>({
    queryKey: [categoriesQueryKey, "hierarchy"],
    queryFn: productApi.getCategoriesHierarchy,
    staleTime: 30 * 60 * 1000,
  });
};
