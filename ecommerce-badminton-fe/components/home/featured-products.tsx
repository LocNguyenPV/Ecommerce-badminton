import { useProducts } from "@/hooks/use-products";
import { ProductCard } from "@/components/product/product-card"; // Giả sử bạn có component này
import { ProductDto, ProductFilters } from "@/types";

export const FeaturedProducts = () => {
  // Định nghĩa filter để lấy 8 sản phẩm đầu tiên, sắp xếp theo tên
  const filters: ProductFilters = {
    page: 1,
    pageSize: 8,
    sortBy: "name",
  };

  const { data, isLoading, isError } = useProducts(filters);

  return (
    <section className="py-16 bg-white">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <h2 className="text-3xl font-extrabold tracking-tight text-gray-900">
            Sản phẩm nổi bật
          </h2>
          <p className="mt-4 max-w-2xl mx-auto text-xl text-gray-500">
            Những sản phẩm được yêu thích nhất của chúng tôi
          </p>
        </div>

        {isLoading && (
          <div className="text-center mt-8">Đang tải sản phẩm...</div>
        )}
        {isError && (
          <div className="text-center mt-8 text-red-500">
            Không thể tải sản phẩm.
          </div>
        )}

        {data && (
          <div className="mt-12 grid grid-cols-1 gap-y-10 gap-x-6 sm:grid-cols-2 lg:grid-cols-4 xl:gap-x-8">
            {data.items.map((product: ProductDto) => (
              <ProductCard key={product.productId} product={product} />
            ))}
          </div>
        )}
      </div>
    </section>
  );
};
