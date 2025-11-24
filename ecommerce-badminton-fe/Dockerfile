FROM node:22-alpine AS base

# Cài đặt các dependency chỉ cho production
FROM base AS deps
RUN apk add --no-cache libc6-compat
WORKDIR /app

# Copy các file package.json và package-lock.json
COPY package.json package-lock.json* ./
RUN npm ci

# Giai đoạn builder
FROM base AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .

# Lệnh build sẽ tạo ra thư mục .next/standalone
RUN npm run build

# Giai đoạn runner - Image sản phẩm cuối cùng
FROM base AS runner
WORKDIR /app
ARG NEXT_PUBLIC_API_BASE_URL

ENV NODE_ENV=production
ENV NEXT_PUBLIC_API_URL=${NEXT_PUBLIC_API_BASE_URL}
# Tạo một user không phải root để tăng cường bảo mật
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

# Copy thư mục public từ builder
COPY --from=builder /app/public ./public

# Tự động tận dụng output memory của Next.js để giảm dung lượng image
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 3000

ENV PORT 3000

# Lệnh để khởi động server sản phẩm
CMD ["node", "server.js"]