import type { NextConfig } from "next";

const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  images: {
    domains: ["localhost", "your-domain.com"],
  },
  env: {
    CUSTOM_KEY: process.env.CUSTOM_KEY,
  },
  output: "standalone",
};

export default nextConfig;
