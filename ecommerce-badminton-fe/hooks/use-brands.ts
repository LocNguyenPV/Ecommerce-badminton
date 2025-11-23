import { useQuery } from "@tanstack/react-query";
import { productApi } from "@/lib/services/product.service";
import { BrandDto } from "@/types";

export const brandsQueryKey = "brands";

export const useBrands = () => {
  return useQuery<BrandDto[], Error>({
    queryKey: [brandsQueryKey],
    queryFn: productApi.getBrands,

    // Thương hiệu cũng hiếm khi thay đổi, có thể cache lâu
    staleTime: 30 * 60 * 1000, // 30 phút
  });
};
