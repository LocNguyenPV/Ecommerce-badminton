import { Providers } from "./providers";
import "./globals.css";
import { PublicEnvScript } from "next-runtime-env";

export const metadata = {
  title: "Badminton E-commerce",
  description: "Best badminton gear",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="vi">
      <head>
        <PublicEnvScript />
      </head>
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
