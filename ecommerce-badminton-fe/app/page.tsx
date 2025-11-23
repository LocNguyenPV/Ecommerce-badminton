"use client";
import { Hero } from "@/components/home/hero";
import { FeaturedProducts } from "@/components/home/featured-products";
import { ProductCategories } from "@/components/home/product-categories";
import { FeaturedBrands } from "@/components/home/featured-brands";

export default function HomePage() {
  return (
    <main>
      <Hero />
      <FeaturedProducts />
      <ProductCategories />
      <FeaturedBrands />
    </main>
  );
}
