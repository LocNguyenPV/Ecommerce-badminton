import { useQuery } from "@tanstack/react-query";
import { productApi } from "@/lib/services/product.service";
import { ProductDetailDto } from "@/types";

export const productDetailQueryKey = "productDetail";

export const useProductDetail = (id: string | undefined) => {
  return useQuery<ProductDetailDto, Error>({
    // Key bao gồm cả ID để cache riêng cho từng sản phẩm
    queryKey: [productDetailQueryKey, id],

    // Chỉ fetch data khi `id` tồn tại
    queryFn: () => productApi.getProductById(id!),
    enabled: !!id,

    // Chi tiết sản phẩm ít thay đổi, có thể cache lâu hơn
    staleTime: 10 * 60 * 1000,
  });
};
