import Link from "next/link";
import Image from "next/image";
import { useCategoriesHierarchy } from "@/hooks/use-categories";
import { CategoryDto } from "@/types";

export const ProductCategories = () => {
  const { data: categories, isLoading, isError } = useCategoriesHierarchy();

  return (
    <section className="py-16 bg-gray-50">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <h2 className="text-3xl font-extrabold tracking-tight text-gray-900">
            Danh mục sản phẩm
          </h2>
          <p className="mt-4 max-w-2xl mx-auto text-xl text-gray-500">
            Dễ dàng tìm kiếm sản phẩm bạn cần
          </p>
        </div>

        {isLoading && (
          <div className="text-center mt-8">Đang tải danh mục...</div>
        )}
        {isError && (
          <div className="text-center mt-8 text-red-500">
            Không thể tải danh mục.
          </div>
        )}

        {categories && (
          <div className="mt-12 grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
            {categories.map((category) => (
              <Link
                key={category.categoryId}
                href={`/products?categoryId=${category.categoryId}`}
                className="group relative bg-white p-6 block rounded-lg border border-gray-200 hover:border-indigo-500 transition-colors"
              >
                <div className="text-center">
                  {/* Giả sử CategoryDto có imageUrl, nếu không thì hiển thị icon placeholder */}
                  <div className="w-20 h-20 mx-auto bg-gray-200 rounded-full flex items-center justify-center text-gray-400 group-hover:bg-gray-300">
                    <span className="text-2xl">📦</span>
                  </div>
                  <p className="mt-4 text-lg font-medium text-gray-900 group-hover:text-indigo-600">
                    {category.name}
                  </p>
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </section>
  );
};
