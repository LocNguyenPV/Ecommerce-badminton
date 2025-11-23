import { useBrands } from "@/hooks/use-brands";

export const FeaturedBrands = () => {
  const { data: brands, isLoading, isError } = useBrands();

  // Chỉ hiển thị 8 thương hiệu đầu tiên
  const displayBrands = brands?.slice(0, 8);

  return (
    <section className="py-16 bg-white">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <h2 className="text-3xl font-extrabold tracking-tight text-gray-900">
            Thương hiệu nổi bật
          </h2>
        </div>

        {isLoading && (
          <div className="text-center mt-8">Đang tải thương hiệu...</div>
        )}
        {isError && (
          <div className="text-center mt-8 text-red-500">
            Không thể tải thương hiệu.
          </div>
        )}

        {displayBrands && (
          <div className="mt-12 grid grid-cols-2 gap-8 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-8">
            {displayBrands.map((brand) => (
              <div
                key={brand.brandId}
                className="col-span-1 flex justify-center items-center"
              >
                {/* Giả sử BrandDto có logoUrl */}
                <img
                  className="h-12 w-auto object-contain grayscale hover:grayscale-0 transition-all"
                  src={`/brands/${brand.brandId}.png`} // Giả sử có folder chứa logo
                  alt={brand.name}
                />
                {/* Nếu không có ảnh logo, hiển thị text */}
                {/* <span className="text-sm text-gray-600">{brand.name}</span> */}
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  );
};
